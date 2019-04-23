using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Terrain
{
    enum DetailBufferType
    {
        Transform,
        Color,
        Normal,
        Length
    }

    class DetailInstancingDraw : InstancingDraw
    {
        private ComputeBuffer _colorData;
        private ComputeBuffer _normalData;

        private readonly Vector3 _basePos;
        private readonly float _wavingGrassStrength;
        private readonly Vector4 _wavingTint;
        private Vector4 _cameraPosition;
        private Vector4 _waveAndDistance;

        internal DetailInstancingDraw(InstancingRenderer renderer, ComputeShader visShader,
            UnityEngine.Terrain setting, TerrainData data)
            : base(renderer, visShader)
        {
            _basePos = setting.transform.position;
            _wavingGrassStrength = data.wavingGrassStrength;

            _wavingTint = data.wavingGrassTint;

            _waveAndDistance.x = 0;
            _waveAndDistance.y = data.wavingGrassSpeed * 0.4f;
            // for mesh grass, should be 0(DetailRenderer.cpp : 288)
            _waveAndDistance.z = data.wavingGrassAmount * 6;
            _waveAndDistance.w = setting.detailObjectDistance * setting.detailObjectDistance;

            _cameraPosition.w = 1 / _waveAndDistance.w;
        }

        public override ComputeBuffer GetMergedTargetBuffer(int index)
        {
            switch ((DetailBufferType) index)
            {
                case DetailBufferType.Transform:
                    return TransformData;
                case DetailBufferType.Normal:
                    return _normalData;
                default:
                    return _colorData;
            }
        }

        public void Update(Vector3 camPos)
        {
            _cameraPosition.x = camPos.x - _basePos.x;
            _cameraPosition.y = camPos.y - _basePos.y;
            _cameraPosition.z = camPos.z - _basePos.z;

            _waveAndDistance.x += Time.deltaTime * _wavingGrassStrength * 0.05f;
        }

        protected override void SetMaterialPropertyBlock()
        {
            Mbp.SetBuffer(Constants.DetailVariable.TransformData, TransformData);
            Mbp.SetBuffer(Constants.DetailVariable.NormalData, _normalData);
            Mbp.SetBuffer(Constants.DetailVariable.ColorData, _colorData);
            Mbp.SetFloat(Constants.DetailVariable.Cutoff, 0.375f);

            Mbp.SetVector(Constants.TerrainVariable.WavingTint, _wavingTint);
            Mbp.SetVector(Constants.TerrainVariable.CameraPosition, _cameraPosition);
            Mbp.SetVector(Constants.TerrainVariable.WaveAndDistance, _waveAndDistance);
        }

        protected override void ReleaseBuffer()
        {
            base.ReleaseBuffer();

            _normalData.Release();
            _normalData = null;

            _colorData.Release();
            _colorData = null;
        }

        protected override void BuildBuffer(int blockCount, int blockSize)
        {
            base.BuildBuffer(blockCount, blockSize);

            var count = blockCount * blockSize;
            _normalData = new ComputeBuffer(count, Constants.StrideSizeFloat3);
            _colorData = new ComputeBuffer(count, Constants.StrideSizeFloat3);
        }
    }
}
