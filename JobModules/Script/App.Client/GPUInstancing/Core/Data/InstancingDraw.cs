using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;
using UnityEngine.Rendering;

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
        
        private ComputeShader _visShader;

        private ComputeBuffer _transformData;
        private readonly ComputeBuffer[] _drawInstanceData;
        private readonly ComputeBuffer[] _argsData;

        public ComputeBuffer TransformData { get { return _transformData; } }

        private int _blockCount;
        private int _blockSize;
        private int[] _realCountInBlockArray;
        private int _totalCountInBlock;
        private ComputeBuffer _realCountInBlockData;

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
        private const int ArgCountPerDraw = 5;

        internal InstancingDraw(InstancingRenderer renderer, ComputeShader visShader)
        {
            _renderer = renderer;
            _visShader = visShader;

            SetShadow(ShadowCastingMode.Off, false);
            _drawInstanceData = new ComputeBuffer[MaxLodLevelCount];

            // index count per instance, instance count, start index location, base vertex location, start instance location.
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
                    for (int k = 0; k < lodRenderer.Renderers[j].SubMeshCount; ++k)
                    {
                        argsArray[drawCallIndex] = lodRenderer.Renderers[j].IndexCount[k];
                        drawCallIndex += ArgCountPerDraw;
                    }
                }
                _argsData[i] = new ComputeBuffer(1, drawCallIndex * sizeof(uint), ComputeBufferType.IndirectArguments);
                _argsData[i].SetData(argsArray);
            }

            if (_lodRatios != null)
            {
                _lodSize = renderer.LodSize;
                _lodRatios = renderer.LodRatios;
            }
        }

        public void SetShadow(ShadowCastingMode castShadow, bool receiveShadow)
        {
            _castShadow = castShadow;
            _receiveShadow = receiveShadow;
        }

        public void SetInstancingCount(int blockCount, int blockSize)
        {
            if (blockSize == 0)
            {
                State = InstancingDrawState.Disable;
                return;
            }

            State = InstancingDrawState.Enable;

            _blockCount = blockCount;
            _blockSize = blockSize;

            if (_transformData != null && _transformData.count != blockCount)
                ReleaseBuffer();

            if (_transformData == null && blockCount != 0)
                BuildBuffer(blockCount, blockSize);
        }

        public void ClearRealBlockCount()
        {
            _totalCountInBlock = 0;

            for (int i = 0; i < _realCountInBlockArray.Length; ++i)
                _realCountInBlockArray[i] = 0;
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

            for (int i = 0; i < _drawInstanceData.Length; ++i)
            {
                _drawInstanceData[i].Release();
                _drawInstanceData[i] = null;
            }

            _realCountInBlockData.Release();
            _realCountInBlockData = null;
        }

        protected virtual void BuildBuffer(int blockCount, int blockSize)
        {
            _transformData = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeMatrix4x4);
            _drawInstanceData[0] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);
            _drawInstanceData[1] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);
            _drawInstanceData[2] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);
            _drawInstanceData[3] = new ComputeBuffer(blockCount * blockSize, Constants.StrideSizeInt, ComputeBufferType.Append);

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
            _drawInstanceData[0].SetCounterValue(0);
            _drawInstanceData[1].SetCounterValue(0);
            _drawInstanceData[2].SetCounterValue(0);
            _drawInstanceData[3].SetCounterValue(0);

            _realCountInBlockData.SetData(_realCountInBlockArray);

            var kernel = _visShader.FindKernel(Constants.CsKernel.Common);

            _visShader.SetBuffer(kernel, Constants.ShaderVariable.InputData, _transformData);
            _visShader.SetBuffer(kernel, Constants.ShaderVariable.RealCountInBlockData, _realCountInBlockData);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod0, _drawInstanceData[0]);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod1, _drawInstanceData[1]);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod2, _drawInstanceData[2]);
            _visShader.SetBuffer(kernel, Constants.MeshVariable.DrawInstanceDataLod3, _drawInstanceData[3]);
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
        }

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

                        for (int k = 0; k < renderer.SubMeshCount; ++k)
                        {
                            ComputeBuffer.CopyCount(_drawInstanceData[i], _argsData[i], drawCallOffset + Constants.StrideSizeUint);

                            _mbp.SetBuffer(Constants.MeshVariable.DrawInstanceData, _drawInstanceData[i]);
                            SetMaterialPropertyBlock();

                            Graphics.DrawMeshInstancedIndirect(
                                renderer.Mesh,
                                k,
                                renderer.Materials[k],
                                testBounds,
                                _argsData[0],
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

        protected virtual void SetMaterialPropertyBlock() { }
    }
}
