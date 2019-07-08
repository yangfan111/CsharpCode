using App.Client.GPUInstancing.Core.Utils;
using App.Shared;
using Core.Components;
using UnityEngine;
using UnityEngine.Rendering;
using App.Shared;

namespace App.Client.GPUInstancing.Core.Data
{
    enum InstancingDrawState
    {
        NotInitialized,
        Disable,
        Enable
    }

    public class InstancingDraw
    {
        protected readonly InstancingRenderer _renderer;
        
        internal InstancingDrawState State { get; private set; }
        
        private readonly ComputeShader _visShader;
        private readonly ComputeShader _sortShader;

        private ComputeBuffer _transformData;
        private readonly ComputeBuffer[] _drawInstanceDataForLod;
        private readonly ComputeBuffer[] _argsData;

        public ComputeBuffer TransformData { get { return _transformData; } }

        private int _blockCount;
        private int _blockSize;
        private int[] _realCountInBlockArray;
        private int _totalCountInBlock;
        private ComputeBuffer _realCountInBlockData;

        public float RendererSphereRadius { get { return _renderer.SphereRadius; } }

        #region Instancing Draw
        
        private readonly MaterialPropertyBlock _mbp = new MaterialPropertyBlock();
        public int Layer { get; set; }
        protected MaterialPropertyBlock Mbp { get { return _mbp; } }
        private ShadowCastingMode _castShadow;
        private bool _receiveShadow;

        private readonly float[] _lodRatios = { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue };
        private readonly float _lodSize = 0;
        
        #endregion

        private const int MaxLodLevelCount = 4;
        // index count per instance, instance count, start index location, base vertex location, start instance location.
        private const int ArgCountPerDraw = 5;

        internal InstancingDraw(InstancingRenderer renderer, ComputeShader visShader, ComputeShader sortShader)
        {
            _renderer = renderer;
            _visShader = visShader;
            _sortShader = sortShader;

            SetShadow(renderer.CastShadow, renderer.ReceiveShadow);
            _drawInstanceDataForLod = new ComputeBuffer[MaxLodLevelCount];

            // assume 20 drawcalls at most for each lod level
            var argsArray = new int[ArgCountPerDraw * 20];
            for (int i = 0; i < argsArray.Length; ++i)
                argsArray[i] = 0;

            var lodLevelCount = renderer.LodLevelCount;
            _argsData = new ComputeBuffer[lodLevelCount];

            for (int i = 0; i < lodLevelCount; ++i)
            {
                var lodRenderer = renderer.GetLodRenderer(i);
                var count = lodRenderer.Renderers.Length;

                int drawCallIndex = 0;
                for (int j = 0; j < count; ++j)
                {
                    if (lodRenderer.Renderers[j] == null)
                        continue;

                    for (int k = 0; k < lodRenderer.Renderers[j].SubMeshCount; ++k)
                    {
                        argsArray[drawCallIndex] = lodRenderer.Renderers[j].IndexCount[k];
                        drawCallIndex += ArgCountPerDraw;
                    }
                }

                if (drawCallIndex != 0)
                {
                    _argsData[i] = new ComputeBuffer(1, drawCallIndex * sizeof(uint), ComputeBufferType.IndirectArguments);
                    _argsData[i].SetData(argsArray);
                }
            }

            if (renderer.LodRatios != null)
            {
                _lodSize = renderer.LodSize;
                _lodRatios = renderer.LodRatios;
            }
        }

        private void SetShadow(ShadowCastingMode castShadow, bool receiveShadow)
        {
            _castShadow = castShadow;
            _receiveShadow = receiveShadow;
        }

        public void SetInstancingFullSizeParam(int blockCount, int blockSize)
        {
            if (blockSize == 0)
            {
                State = InstancingDrawState.Disable;
                return;
            }

            State = InstancingDrawState.Enable;

            _blockCount = blockCount;
            _blockSize = blockSize;

            if (_transformData != null && _transformData.count != blockCount * blockSize)
                ReleaseBuffer();

            if (_transformData == null && blockCount != 0)
                BuildBuffer(blockCount, blockSize);
        }

        public void ClearRealBlockCount()
        {
            _totalCountInBlock = 0;

            if (_realCountInBlockArray != null)
            {
                for (int i = 0; i < _realCountInBlockArray.Length; ++i)
                    _realCountInBlockArray[i] = 0;
            }
        }

        public void SetRealBlockCount(int index, int realCount)
        {
            _totalCountInBlock += realCount;

            _realCountInBlockArray[index] = realCount;
        }

        protected virtual void ReleaseBuffer()
        {
            _transformData.Release();
            _transformData = null;

            for (int i = 0; i < _drawInstanceDataForLod.Length; ++i)
            {
                _drawInstanceDataForLod[i].Release();
                _drawInstanceDataForLod[i] = null;
            }

            _realCountInBlockData.Release();
            _realCountInBlockData = null;
        }

        protected virtual void BuildBuffer(int blockCount, int blockSize)
        {
            _transformData = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeMatrix4x4);
            _drawInstanceDataForLod[0] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);
            _drawInstanceDataForLod[1] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);
            _drawInstanceDataForLod[2] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);
            _drawInstanceDataForLod[3] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);

            _realCountInBlockData = new ComputeBuffer(blockCount, Constants.StrideSizeInt);
            _realCountInBlockArray = new int[blockCount];
        }

        public bool CanDraw()
        {
            return _totalCountInBlock != 0;
        }

        private readonly float[] _sphereCenter = new float[3];
        public void VisibilityDetermination(float[] cameraPos, float tanHalfFov, float[][] camPlaneNormal, float camFarClipDist)
        {
            if (sorted)
                return;

            _drawInstanceDataForLod[0].SetCounterValue(0);
            _drawInstanceDataForLod[1].SetCounterValue(0);
            _drawInstanceDataForLod[2].SetCounterValue(0);
            _drawInstanceDataForLod[3].SetCounterValue(0);

            _realCountInBlockData.SetData(_realCountInBlockArray);

            var kernel = _visShader.FindKernel(Constants.CsKernel.Common);

            _visShader.SetBuffer(kernel, Constants.ShaderVariable.InputData, _transformData);
            _visShader.SetBuffer(kernel, Constants.ShaderVariable.RealCountInBlockData, _realCountInBlockData);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod0, _drawInstanceDataForLod[0]);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod1, _drawInstanceDataForLod[1]);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod2, _drawInstanceDataForLod[2]);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod3, _drawInstanceDataForLod[3]);
            _visShader.SetInt(Constants.ShaderVariable.InputDataCount, _totalCountInBlock);
            _visShader.SetInt(Constants.ShaderVariable.BlockCount, _blockCount);
            _visShader.SetInt(Constants.ShaderVariable.BlockSize, _blockSize);

            _visShader.SetFloat(Constants.CameraVariable.TanHalfFov, tanHalfFov);
            _visShader.SetFloat(Constants.CameraVariable.CamFarClipDistance, camFarClipDist);
            _visShader.SetFloats(Constants.CameraVariable.CameraWorldPosition, cameraPos);
            _visShader.SetFloats(Constants.CameraVariable.CamLeftPlaneWorld, camPlaneNormal[(int) CameraFrustum.Direction.Left]);
            _visShader.SetFloats(Constants.CameraVariable.CamRightPlaneWorld, camPlaneNormal[(int) CameraFrustum.Direction.Right]);
            _visShader.SetFloats(Constants.CameraVariable.CamTopPlaneWorld, camPlaneNormal[(int) CameraFrustum.Direction.Top]);
            _visShader.SetFloats(Constants.CameraVariable.CamBottomPlaneWorld, camPlaneNormal[(int) CameraFrustum.Direction.Bottom]);
            _visShader.SetFloats(Constants.CameraVariable.CamFarPlaneWorld, camPlaneNormal[(int) CameraFrustum.Direction.Far]);

            _visShader.SetFloats(Constants.MeshVariable.LodRatios, _lodRatios);
            _visShader.SetFloat(Constants.MeshVariable.LodSize, _lodSize);
            _renderer.SphereCenter.ConvertToFloatArray(_sphereCenter);
            _visShader.SetFloats(Constants.MeshVariable.SphereCenter, _sphereCenter);
            _visShader.SetFloat(Constants.MeshVariable.SphereRadius, _renderer.SphereRadius);

            _visShader.Dispatch(kernel, Mathf.CeilToInt(_totalCountInBlock / (float) Constants.VisibilityThreadCount), 1, 1);

            if (SharedConfig.GPUSort)
            {
                if (!sorted)
                {
                    sorted = true;

                    ComputeBuffer countBuffer = new ComputeBuffer(1, Constants.StrideSizeUint, ComputeBufferType.Raw);
                    int[] count = new int[1];

                    var sortKernel = _sortShader.FindKernel(Constants.CsKernel.Common);

                    float[] RawInspector = null;
                    float[] SortedInspector = null;

                    for (int i = 0; i < 4; ++i)
                    {
                        ComputeBuffer.CopyCount(_drawInstanceDataForLod[i], countBuffer, 0);
                        countBuffer.GetData(count);

                        ComputeBuffer rawDist = null;
                        ComputeBuffer sortedDist = null;

                        if (count[0] > 0)
                        {
                            args[0] = 6;
                            args[1] = count[0];
                            rawDist = new ComputeBuffer(count[0], Constants.StrideSizeFloat);
                            sortedDist = new ComputeBuffer(count[0], Constants.StrideSizeFloat);
                            _sortShader.SetBuffer(sortKernel, "RawDistance", rawDist);
                            _sortShader.SetBuffer(sortKernel, "SortedDistance", sortedDist);
                            RawInspector = new float[500];
                            SortedInspector = new float[500];
                        }

                        _sortShader.SetInt("Count", count[0]);
                        _sortShader.SetBuffer(sortKernel, "TransformData", _transformData);
                        _sortShader.SetBuffer(sortKernel, "Index", _drawInstanceDataForLod[i]);
                        _sortShader.SetFloats("CameraPos", cameraPos);

                        for (int j = 0; j < count[0]; ++j)
                        {
                            _sortShader.SetInt("Stage", j % 2);
                            if (j == 0)
                                _sortShader.SetInt("Record", 1);
                            else if (j == count[0] - 1)
                                _sortShader.SetInt("Record", 2);
                            else
                                _sortShader.SetInt("Record", 0);
                            _sortShader.Dispatch(sortKernel, Mathf.CeilToInt(count[0] / (float) 1024), 1, 1);
                        }

                        if (count[0] > 0)
                        {
                            rawDist.GetData(RawInspector);
                            sortedDist.GetData(SortedInspector);
                            int a = 0;
                        }
                    }

                    countBuffer.Release();
                }
            }
        }

        private static bool sorted;
        private int[] args = new int[6];

        private Bounds testBounds = new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000));
        public void Draw(Camera camera)
        {
            if (CanDraw())
            {
                var lodLevelCount = Mathf.Min(MaxLodLevelCount, _renderer.LodLevelCount);
                for (int i = 0; i < lodLevelCount; ++i)
                {
                    var lodRenderer = _renderer.GetLodRenderer(i);
                    var count = lodRenderer.Renderers.Length;
                    
                    int drawCallOffset = 0;
                    for (int j = 0; j < count; ++j)
                    {
                        var renderer = lodRenderer.Renderers[j];
                        if (renderer == null)
                            continue;

                        for (int k = 0; k < renderer.SubMeshCount; ++k)
                        {
                            if (sorted)
                                _argsData[i].SetData(args);
                            else
                            {
                                ComputeBuffer.CopyCount(_drawInstanceDataForLod[i], _argsData[i],
                                    drawCallOffset + Constants.StrideSizeUint);
//                                _argsData[i].GetData(args);
//                                _argsData[i].SetData(args);
                            }

                            _mbp.SetBuffer(Constants.MeshVariable.DrawInstanceData, _drawInstanceDataForLod[i]);
                            SetMaterialPropertyBlock();

                            if (SharedConfig.GrassQueue != -1)
                            {
                                renderer.Materials[k].renderQueue = SharedConfig.GrassQueue;
                            }
                                
                            Graphics.DrawMeshInstancedIndirect(
                                renderer.Mesh,
                                k,
                                renderer.Materials[k],
                                testBounds,
                                _argsData[i],
                                drawCallOffset,
                                _mbp,
                                _castShadow,
                                _receiveShadow,
                                Layer,
                                camera);

                            drawCallOffset += ArgCountPerDraw * sizeof(uint);
                        }
                    }
                }
            }
        }

        public virtual ComputeBuffer GetMergedTargetBuffer(int index)
        {
            return _transformData;
        }

        protected virtual void SetMaterialPropertyBlock()
        {
            _mbp.SetVector(Constants.ShaderVariable.WorldShift, WorldOrigin.Origin);
        }
    }
}
