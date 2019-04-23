using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Terrain;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Spatial
{
    class GpuInstancingDetailNode : GpuInstancingNode
    {
        private List<int[]> _countInNode;
        private int[] _totalCountInLayer;
        private int _resolution;
        private int[] _index;

        private ComputeShader _instantiationShader;
        private ComputeBuffer[] _transform;
        private ComputeBuffer[] _normal;
        private ComputeBuffer[] _color;
        private ComputeBuffer[] _count;

        private int[] _countInCpu;
        private readonly int[] _countArray = new int[1];

        private TerrainProperty _terrainProperty;
        private List<DividedDetailProperty> _detailProperties;

        public void InitCountInUnit(List<int[,]> detail, int indexX, int indexY, int size)
        {
            var count = detail.Count;

            _countInNode = new List<int[]>(count);
            _totalCountInLayer = new int[count];

            _index = new[] { indexX, indexY };
            _resolution = size;

            _transform = new ComputeBuffer[count];
            _normal = new ComputeBuffer[count];
            _color = new ComputeBuffer[count];
            _count = new ComputeBuffer[count];
            _countInCpu = new int[count];

            int pixelStartX = indexX * size;
            int pixelStartY = indexY * size;

            for (int i = 0; i < count; ++i)
            {
                var data = detail[i];
                var countInLayer = 0;
                int[] layer = new int[size * size];
                for (int j = 0; j < size; ++j)
                {
                    for (int k = 0; k < size; ++k)
                    {
                        layer[k * size + j] = data[pixelStartY + k, pixelStartX + j];
                        countInLayer += data[pixelStartY + k, pixelStartX + j];
                    }
                }

                _countInNode.Add(layer);
                _totalCountInLayer[i] = countInLayer;
                _countInCpu[i] = 0;
            }
        }

        public void SetInstantiationShader(ComputeShader shader)
        {
            _instantiationShader = shader;
        }

        public void SetShaderProperty(TerrainProperty terrain, List<DividedDetailProperty> detailProperties)
        {
            _terrainProperty = terrain;
            _detailProperties = detailProperties;
        }

        public override int[] MaxInstanceCount
        {
            get { return _totalCountInLayer; }
        }

        public override void BuildBuffer(ComputeBuffer heightBuffer)
        {
            var kernelId = _instantiationShader.FindKernel(Constants.CsKernel.Common);

            ComputeBuffer countInUnit = new ComputeBuffer(_resolution * _resolution, Constants.StrideSizeInt);

            int[] initialCounter = { 0 };

            _instantiationShader.SetBuffer(kernelId, Constants.TerrainVariable.HeightMapData, heightBuffer);
            _instantiationShader.SetBuffer(kernelId, Constants.DetailVariable.DividedDetailData, countInUnit);

            _terrainProperty.SetDetailInstantiationProperty(_instantiationShader);

            var count = _countInNode.Count;
            for (int i = 0; i < count; ++i)
            {
                if (_totalCountInLayer[i] == 0)
                    continue;

                if (_transform[i] == null)
                {
                    _transform[i] = new ComputeBuffer(_totalCountInLayer[i], Constants.StrideSizeMatrix4x4);
                    _normal[i] = new ComputeBuffer(_totalCountInLayer[i], Constants.StrideSizeFloat3);
                    _color[i] = new ComputeBuffer(_totalCountInLayer[i], Constants.StrideSizeFloat3);
                    _count[i] = new ComputeBuffer(1, Constants.StrideSizeInt);
                }

                _countInCpu[i] = -1;

                _count[i].SetData(initialCounter);
                _instantiationShader.SetBuffer(kernelId, Constants.DetailVariable.TransformData, _transform[i]);
                _instantiationShader.SetBuffer(kernelId, Constants.DetailVariable.NormalData, _normal[i]);
                _instantiationShader.SetBuffer(kernelId, Constants.DetailVariable.ColorData, _color[i]);
                _instantiationShader.SetBuffer(kernelId, Constants.ShaderVariable.CounterData, _count[i]);

                countInUnit.SetData(_countInNode[i]);

                _detailProperties[i].SetDetailInstantiationProperty(_instantiationShader, _index);

                _instantiationShader.Dispatch(kernelId,
                                              _resolution / Constants.DetailInstantiationThreadCount,
                                              _resolution / Constants.DetailInstantiationThreadCount,
                                              1);
            }

            countInUnit.Release();
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
                    _normal[i].Release();
                    _normal[i] = null;
                    _color[i].Release();
                    _color[i] = null;
                    _count[i].Release();
                    _count[i] = null;
                    _countInCpu[i] = 0;
                }
            }
        }

        private readonly ComputeBuffer[] _cache = new ComputeBuffer[(int) DetailBufferType.Length];
        public override ComputeBuffer[] GetInstancingData(int index)
        {
            _cache[(int) DetailBufferType.Transform] = _transform[index];
            _cache[(int) DetailBufferType.Normal] = _normal[index];
            _cache[(int) DetailBufferType.Color] = _color[index];
            return _cache;
        }

        public override int GetInstancingDataCount(int index)
        {
            if (_countInCpu[index] == -1)
            {
                _count[index].GetData(_countArray);
                _countInCpu[index] = _countArray[0];
            }

            return _countInCpu[index];
        }

        public override MergeUnit[] GetMergeKernels(ComputeShader shader)
        {
            return MergeKernel.GetDetailMergeKernel(shader);
        }

        public override void Debug()
        {
        }
    }
}
