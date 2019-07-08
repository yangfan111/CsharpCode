using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Terrain;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Spatial
{
    class GpuInstancingTreeNode : GpuInstancingNode
    {
        #region Debug
        
        public string TerrainName;
        public int X;
        public int Z;
        
        #endregion

        private ComputeShader _instantiationShader;
        private ComputeBuffer[] _transform;

        private float[][] _rawTrs;
        private int[] _instanceCount;
        private int _maxCountBetweenInstances = 0;
        // pos.x | pos.y | pos.z | scale.width | scale.height | rotation(radian)
        private const int TreeTrsStride = 6;

        private TerrainProperty _terrainProperty;

        private bool _empty = true;

        public void SetTreeInstances(List<int>[] indices, TreeInstance[] allTrees)
        {
            var prototypeCount = indices.Length;
            _transform = new ComputeBuffer[prototypeCount];
            _rawTrs = new float[prototypeCount][];
            _instanceCount = new int[prototypeCount];

            for (int i = 0; i < prototypeCount; ++i)
            {
                if (indices[i] == null)
                    continue;

                var count = indices[i].Count;

                _instanceCount[i] = count;
                _maxCountBetweenInstances = Mathf.Max(_maxCountBetweenInstances, _instanceCount[i]);
                _rawTrs[i] = new float[count * TreeTrsStride];
                
                int index = 0;
                for (int j = 0; j < count; ++j)
                {
                    var tree = allTrees[indices[i][j]];

                    _rawTrs[i][index++] = tree.position.x;
                    _rawTrs[i][index++] = tree.position.y;
                    _rawTrs[i][index++] = tree.position.z;
                    _rawTrs[i][index++] = tree.widthScale;
                    _rawTrs[i][index++] = tree.heightScale;
                    _rawTrs[i][index++] = tree.rotation;
                }

                _empty = _empty && count == 0;
            }
        }

        public void SetInstantiationShader(ComputeShader shader)
        {
            _instantiationShader = shader;
        }

        public void SetShaderProperty(TerrainProperty property)
        {
            _terrainProperty = property;
        }

        public override int[] MaxInstanceCount
        {
            get { return _instanceCount; }
        }

        public override void BuildBuffer(ComputeBuffer heightBuffer)
        {
            if (_maxCountBetweenInstances == 0)
                return;

            var kernelId = _instantiationShader.FindKernel(Constants.CsKernel.Common);
            ComputeBuffer trs = new ComputeBuffer(_maxCountBetweenInstances * TreeTrsStride, Constants.StrideSizeFloat);
            _terrainProperty.SetTreeInstantiationProperty(_instantiationShader);

            var count = _instanceCount.Length;
            for (int i = 0; i < count; ++i)
            {
                if (_instanceCount[i] == 0)
                    continue;

                if (_transform[i] == null)
                    _transform[i] = new ComputeBuffer(_instanceCount[i], Constants.StrideSizeMatrix4x4);

                trs.SetData(_rawTrs[i]);
                
                _instantiationShader.SetBuffer(kernelId, Constants.TerrainVariable.TransformData, _transform[i]);
                _instantiationShader.SetBuffer(kernelId, Constants.TreeVariable.RawTRSData, trs);
                _instantiationShader.SetInt(Constants.ShaderVariable.InputDataCount, _instanceCount[i]);

                _instantiationShader.Dispatch(kernelId, Mathf.CeilToInt(_instanceCount[i] / (float) Constants.TreeInstantiationTreadCount), 1, 1);
            }
            
            trs.Release();
        }

        public override void ReleaseBuffer()
        {
            var count = _transform.Length;
            for (int i = 0; i < count; ++i)
            {
                if (_transform[i] != null)
                {
                    _transform[i].Release();
                    _transform[i] = null;
                }
            }
        }

        private readonly ComputeBuffer[] _cache = new ComputeBuffer[(int) TreeBufferType.Length];

        public override ComputeBuffer[] GetInstancingData(int index)
        {
            _cache[(int) TreeBufferType.Transform] = _transform[index];
            return _cache;
        }

        public override int GetInstancingDataCount(int index)
        {
            return _instanceCount[index];
        }

        public override MergeUnit[] GetMergeKernels(ComputeShader shader)
        {
            return MergeKernel.GetTreeMergeKernel(shader);
        }

        public override bool Empty
        {
            get { return _empty; }
        }
    }
}