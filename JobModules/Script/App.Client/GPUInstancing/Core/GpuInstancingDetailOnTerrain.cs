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

        public void Init(UnityEngine.Terrain setting, TerrainData data)
        {
            var detailResolution = data.detailResolution;
            var division = Mathf.CeilToInt(detailResolution / (float) _detailNodeSize);

            if (!Helper.AlmostEqual(detailResolution, division * _detailNodeSize))
                return;

            var prototypes = data.detailPrototypes;
            var detailCount = prototypes.Length;
            var detailData = new List<int[,]>();

            _instancingDraw = new DetailInstancingDraw[detailCount];
            List<DividedDetailProperty> detailProps = new List<DividedDetailProperty>();

            for (int i = 0; i < detailCount; ++i)
            {
                detailData.Add(data.GetDetailLayer(0, 0, detailResolution, detailResolution, i));
                detailProps.Add(new DividedDetailProperty(data, prototypes[i], _detailNodeSize, i));

                var renderer = new InstancingRenderer(prototypes[i]);
                _instancingDraw[i] = new DetailInstancingDraw(renderer, VisibilityShader, setting, data);
            }

            var nodeSize = new Vector2(data.size.x / division, data.size.z / division);

            var heightMapResolution = data.heightmapResolution;
            _data.InitDivision(setting.transform.position, nodeSize, division, division, setting.detailObjectDistance);
            _data.InitHeightMap(data.GetHeights(0, 0, heightMapResolution, heightMapResolution), data.size.y);

            _terrainProperty = new TerrainProperty(setting, data);

            for (int i = 0; i < division; ++i)
            {
                for (int j = 0; j < division; ++j)
                {
                    var node = new GpuInstancingDetailNode();
                    node.SetInstantiationShader(_instantiationShader);
                    node.SetShaderProperty(_terrainProperty, detailProps);
                    node.InitCountInUnit(detailData, i, j, _detailNodeSize);

                    _data.AddNode(i, j, node);
                }
            }
        }

        protected override void FrustumCulling()
        {
            _data.FrustumCullingByGrid(GetFrustum());
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
