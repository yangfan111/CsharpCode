using UnityEngine;

namespace App.Client.GPUInstancing.Core.Terrain
{
    class DetailInstancingPrototype
    {
        // minWidth | maxWidth | minHeight | maxHeight
        private float[] _scale = new float[4];
        private float _noiseSpread;
        private float[] _dryColor = new float[3];
        private float[] _healthyColor = new float[3];

        public float[] Scale { get { return _scale; } }
        public float NoiseSpread { get { return _noiseSpread; } }
        public float[] DryColor { get { return _dryColor; } }
        public float[] HealthyColor { get { return _healthyColor; } }

        public void Init(DetailPrototype prototype)
        {
            _scale[0] = prototype.minWidth;
            _scale[1] = prototype.maxWidth;
            _scale[2] = prototype.minHeight;
            _scale[3] = prototype.maxHeight;

            _noiseSpread = prototype.noiseSpread;

            _dryColor[0] = prototype.dryColor.r;
            _dryColor[1] = prototype.dryColor.g;
            _dryColor[2] = prototype.dryColor.b;

            _healthyColor[0] = prototype.healthyColor.r;
            _healthyColor[1] = prototype.healthyColor.g;
            _healthyColor[2] = prototype.healthyColor.b;
        }
    }
}
