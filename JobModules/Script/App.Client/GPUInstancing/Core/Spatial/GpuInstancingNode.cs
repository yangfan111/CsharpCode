using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Spatial
{
    abstract class GpuInstancingNode
    {
        public abstract int[] MaxInstanceCount { get; }

        public abstract void BuildBuffer(ComputeBuffer heightBuffer);
        public abstract void ReleaseBuffer();

        public abstract ComputeBuffer[] GetInstancingData(int index);
        public abstract int GetInstancingDataCount(int index);

        public abstract MergeUnit[] GetMergeKernels(ComputeShader shader);
    }
}
