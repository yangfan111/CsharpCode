using System;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Spatial;
using App.Client.GPUInstancing.Core.Terrain;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core
{
    public class GpuInstancingDetailOnTerrain : GpuInstancingPipeline
    {
        private readonly GpuInstancingSpatialPartition<GpuInstancingDetailNode> _data =
            new GpuInstancingSpatialPartition<GpuInstancingDetailNode>();

        private readonly ComputeShader _instantiationShader;
        private readonly ComputeBuffer _perlinNoiseBuffer;

        private int _detailNodeSize = Constants.DefaultDetailNodeSize;

        private DetailInstancingDraw[] _instancingDraw;

        private TerrainProperty _terrainProperty;

        private bool _gotSthToDraw;

        public GpuInstancingDetailOnTerrain(ComputeShader instantiation, ComputeShader merge,
            ComputeShader visibility)
            : base(visibility)
        {
            _instantiationShader = instantiation;

            _perlinNoiseBuffer = Constants.Random.GetPerlinNoiseBuffer();
            var kernel = _instantiationShader.FindKernel(Constants.CsKernel.Common);
            _instantiationShader.SetBuffer(kernel, Constants.RandVariable.PerlinNoiseConstants,
                _perlinNoiseBuffer);

            _data.SetMergeShader(merge);
        }

        public bool TerrainLoaded(UnityEngine.Terrain setting, float detailDistance)
        {
            var data = setting.terrainData;

            var detailResolution = data.detailResolution;
            var heightMapResolution = data.heightmapResolution;
            var prototypes = data.detailPrototypes;
            var detailCount = prototypes.Length;

            var division = Mathf.CeilToInt(detailResolution / (float) _detailNodeSize);
            if (!Helper.AlmostEqual(detailResolution, division * _detailNodeSize))
                return false;

            var detailData = new List<int[,]>();

            if (_instancingDraw == null)
                _instancingDraw = new DetailInstancingDraw[detailCount];

            List<DividedDetailProperty> detailProps = new List<DividedDetailProperty>();

            for (int i = 0; i < detailCount; ++i)
            {
                detailData.Add(data.GetDetailLayer(0, 0, detailResolution, detailResolution, i));
                detailProps.Add(new DividedDetailProperty(data, prototypes[i], _detailNodeSize, i));

                if (_instancingDraw[i] == null)
                {
                    var renderer = new InstancingRenderer(prototypes[i]);
                    _instancingDraw[i] = new DetailInstancingDraw(renderer, VisibilityShader, setting, data);
                }
            }

            var nodeSize = new Vector2(data.size.x / division, data.size.z / division);

            var cluster = new GpuInstancingNodeCluster<GpuInstancingDetailNode>();
            cluster.InitDivision(setting.transform.position, nodeSize, division, division, detailDistance);
            cluster.InitHeightMap(data.GetHeights(0, 0, heightMapResolution, heightMapResolution), data.size.y);

            cluster.TerrainName = setting.name;

            _data.SetGridParam(data.size.x, detailDistance, nodeSize);

            _terrainProperty = new TerrainProperty(setting, data, detailDistance);

            for (int x = 0; x < division; ++x)
            {
                for (int z = 0; z < division; ++z)
                {
                    var node = new GpuInstancingDetailNode();

                    node.TerrainName = setting.name;
                    node.X = x;
                    node.Z = z;

                    node.SetInstantiationShader(_instantiationShader);
                    node.SetShaderProperty(_terrainProperty, detailProps);
                    node.InitCountInUnit(detailData, x, z, _detailNodeSize);

                    cluster.AddNode(x, z, node);
                    cluster.UpdateMaxCountInLayer(node);
                }
            }

            _data.AddCluster(cluster);
            return true;
        }

        public bool TerrainLoaded(TextAsset asset, UnityEngine.Terrain setting, float detailDistance)
        {
            var data = setting.terrainData;
            // ignore version
            int dataIndex = 2;

            int heightMapResolution = 0;
            int detailResolution = 0;
            int detailCount = 0;
            List<int> maxCountInLayer = new List<int>();

            unsafe
            {
                var head = new IntPtr(asset.GetBytesIntPtr().ToInt64() + dataIndex);
                ushort* flowData = (ushort*) head.ToPointer();

                heightMapResolution = *flowData++;
                detailResolution = *flowData++;
                _detailNodeSize = *flowData++;
                detailCount = *flowData++;
                for (int i = 0; i < detailCount; ++i)
                {
                    maxCountInLayer.Add(*flowData++);
                }

                dataIndex += 2 * (4 + detailCount);
            }

            var division = detailResolution / _detailNodeSize;
            var prototypes = data.detailPrototypes;
            if (heightMapResolution != data.heightmapResolution || detailResolution != data.detailResolution ||
                detailCount != prototypes.Length)
                return false;

            if (_instancingDraw == null)
                _instancingDraw = new DetailInstancingDraw[detailCount];

            List<DividedDetailProperty> detailProps = new List<DividedDetailProperty>();

            for (int i = 0; i < detailCount; ++i)
            {
                detailProps.Add(new DividedDetailProperty(data, prototypes[i], _detailNodeSize, i));

                if (_instancingDraw[i] == null)
                {
                    var renderer = new InstancingRenderer(prototypes[i]);
                    _instancingDraw[i] = new DetailInstancingDraw(renderer, VisibilityShader, setting, data);
                }
            }

            var nodeSize = new Vector2(data.size.x / division, data.size.z / division);
            
            var cluster = new GpuInstancingNodeCluster<GpuInstancingDetailNode>();
            cluster.InitDivision(setting.transform.position, nodeSize, division, division, detailDistance);
            cluster.InitHeightMap(asset, dataIndex, data.size.y, heightMapResolution);

            cluster.TerrainName = setting.name;

            dataIndex += heightMapResolution * heightMapResolution * 2;

            _data.SetGridParam(data.size.x, detailDistance, nodeSize);

            _terrainProperty = new TerrainProperty(setting, data, detailDistance);
            var layerLength = (detailResolution * detailResolution + division * division) * 2;

            for (int z = 0; z < division; ++z)
            {
                for (int x = 0; x < division; ++x)
                {
                    var node = new GpuInstancingDetailNode();

                    node.TerrainName = setting.name;
                    node.X = x;
                    node.Z = z;

                    node.SetInstantiationShader(_instantiationShader);
                    node.SetShaderProperty(_terrainProperty, detailProps);
                    node.InitCountInUnit(x, z, _detailNodeSize, asset, maxCountInLayer, dataIndex, layerLength, division);

                    cluster.AddNode(x, z, node);
                }
            }

            cluster.SetMaxCountInLayer(maxCountInLayer);

            _data.AddCluster(cluster);
            return true;
        }

        public void TerrainUnloaded(Vector3 minPos)
        {
            _data.RemoveCluster(minPos);
        }

        protected override void FrustumCulling()
        {
            _data.DistanceAndFrustumCulling(GetFrustum());
        }

        protected override void CreateInstance()
        {
            _data.CreateUnitedInstance(_instancingDraw);

            _gotSthToDraw = false;

            foreach (var instance in _instancingDraw)
            {
                if (instance.CanDraw())
                {
                    _gotSthToDraw = true;
                    _terrainProperty.SetDetailCullingProperty(VisibilityShader);
                    VisibilityDetermination(instance);
                }
            }
        }

        protected override void InstancingDraw(Camera camera)
        {
            if (_gotSthToDraw)
            {
                foreach (var instance in _instancingDraw)
                {
                    if (instance.CanDraw())
                    {
                        instance.Update(GetCameraPosition());
                        instance.Draw(camera);
                    }
                }
            }
        }
    }
}