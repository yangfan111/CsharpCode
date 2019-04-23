using App.Client.GPUInstancing.Core.Terrain;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Utils
{
    class MergeUnit
    {
        public int Kernel;
        public int Input;
        public int Output;
    }

    static class MergeKernel
    {
        private static MergeUnit[] _detailMergeKernel;
        public static MergeUnit[] GetDetailMergeKernel(ComputeShader shader)
        {
            if (_detailMergeKernel == null)
            {
                _detailMergeKernel = new MergeUnit[(int) DetailBufferType.Length];
                _detailMergeKernel[(int) DetailBufferType.Transform] = new MergeUnit
                {
                    Kernel = shader.FindKernel(Constants.MergeVariable.MergeFloat4x4),
                    Input = Constants.MergeVariable.InputFloat4x4,
                    Output = Constants.MergeVariable.OutputFloat4x4
                };
                _detailMergeKernel[(int) DetailBufferType.Color] = new MergeUnit
                {
                    Kernel = shader.FindKernel(Constants.MergeVariable.MergeFloat3),
                    Input = Constants.MergeVariable.InputFloat3,
                    Output = Constants.MergeVariable.OutputFloat3
                };
                _detailMergeKernel[(int) DetailBufferType.Normal] = new MergeUnit
                {
                    Kernel = shader.FindKernel(Constants.MergeVariable.MergeFloat3),
                    Input = Constants.MergeVariable.InputFloat3,
                    Output = Constants.MergeVariable.OutputFloat3
                };
            }

            return _detailMergeKernel;
        }
    }
}
