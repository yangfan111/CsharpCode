using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Data
{
    class TerrainProperty
    {
        private readonly float _heightMapResolution;

        private readonly float _detailResolution;
        private readonly float _detailResolutionPerPatch;

        private readonly float _detailDensity;
        private readonly float _detailDistance;

        private readonly float[] _terrainSize;
        private readonly float[] _basePosition;
        // for world shifting
        // public float[] Offset { get; private set; }

        public TerrainProperty(UnityEngine.Terrain terrain, TerrainData data)
        {
            _heightMapResolution = data.heightmapResolution;

            _detailDensity = terrain.detailObjectDensity;
            _detailDistance = terrain.detailObjectDistance;

            var terrainSize = data.size;
            _terrainSize = new float[3];
            _terrainSize[0] = terrainSize.x;
            _terrainSize[1] = terrainSize.y;
            _terrainSize[2] = terrainSize.z;

            var position = terrain.transform.position;
            _basePosition = new float[3];
            _basePosition[0] = position.x;
            _basePosition[1] = position.y;
            _basePosition[2] = position.z;

            _detailResolution = data.detailResolution;
            _detailResolutionPerPatch = data.detailResolutionPerPatch;
            _detailResolutionPerPatch = 8;
        }

        public void SetDetailInstantiationProperty(ComputeShader shader)
        {
            shader.SetFloat(Constants.TerrainVariable.HeightMapResolution, _heightMapResolution);
            shader.SetFloat(Constants.DetailVariable.DetailDensity, _detailDensity);
            shader.SetFloat(Constants.DetailVariable.DetailResolution, _detailResolution);
            shader.SetFloat(Constants.DetailVariable.DetailResolutionPerPatch, _detailResolutionPerPatch);
            shader.SetFloats(Constants.TerrainVariable.TerrainSize, _terrainSize);
            shader.SetFloats(Constants.DetailVariable.BasePosition, _basePosition);
        }

        public void SetDetailCullingProperty(ComputeShader shader)
        {
            shader.SetFloat(Constants.ShaderVariable.CullingDistance, _detailDistance);
        }
    }

    class DividedDetailProperty
    {
        // minWidth | maxWidth | minHeight | maxHeight
        private readonly float[] _scale;
        private readonly float _resolution;
        private readonly float _noiseSpread;
        private readonly float[] _healthyColor;
        private readonly float[] _dryColor;
        private readonly int _layer;

        public DividedDetailProperty(TerrainData data, DetailPrototype prototype, float resolution, int layer)
        {
            _scale = new float[4];
            _scale[0] = prototype.minWidth;
            _scale[1] = prototype.maxWidth;
            _scale[2] = prototype.minHeight;
            _scale[3] = prototype.maxHeight;

            _resolution = resolution;
            _noiseSpread = prototype.noiseSpread;

            _healthyColor = new float[3];
            _healthyColor[0] = prototype.healthyColor.r;
            _healthyColor[1] = prototype.healthyColor.g;
            _healthyColor[2] = prototype.healthyColor.b;

            _dryColor = new float[3];
            _dryColor[0] = prototype.dryColor.r;
            _dryColor[1] = prototype.dryColor.g;
            _dryColor[2] = prototype.dryColor.b;

            _layer = layer;
        }

        public void SetDetailInstantiationProperty(ComputeShader shader, int[] index)
        {
            shader.SetFloat(Constants.DetailVariable.DividedDetailResolution, _resolution);
            shader.SetFloats(Constants.DetailVariable.DetailScale, _scale);
            shader.SetInts(Constants.DetailVariable.DividedDetailIndex, index);
            shader.SetInt(Constants.DetailVariable.DetailLayer, _layer);
            shader.SetFloat(Constants.DetailVariable.NoiseSpread, _noiseSpread);
            shader.SetFloats(Constants.DetailVariable.HealthyColor, _healthyColor);
            shader.SetFloats(Constants.DetailVariable.DryColor, _dryColor);
        }
    }
}
