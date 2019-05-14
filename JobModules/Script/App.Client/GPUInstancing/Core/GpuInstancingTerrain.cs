using UnityEngine;

namespace App.Client.GPUInstancing.Core
{
    public class GpuInstancingTerrain : GpuInstancingPipeline
    {
        public GpuInstancingTerrain(ComputeShader visibilityShader) : base(visibilityShader)
        {
        }
    }
}
