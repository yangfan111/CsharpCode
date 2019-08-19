using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StatsMonitor;

namespace Utils.StatsMonitor.Scripts.GPU
{
    public class GPUProfiler : IProfiler
    {
        public GPUProfiler(string name)
        {
            Name = name;
            IsRegistered = false;
        }

        public bool IsRegistered;

        public string Name { get; set; }

        private float _value = 0.0f;
        public float SampleValue()
        {
            return _value;
        }

        public void SetData(float val)
        {
            _value = val * 0.9f + _value * 0.1f;
        }
    }
}
