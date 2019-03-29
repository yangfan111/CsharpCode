using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace App.Client.SceneManagement.DistanceCulling.Factory
{
    class ProbeCullingHandler : CullingHandler
    {
        private readonly Action<ProbeCullingHandler> _reuseHandler;
        private readonly List<MeshRenderer> _lightprobeRenderers = new List<MeshRenderer>();
        private readonly List<MeshRenderer> _reflectionprobeRenderers = new List<MeshRenderer>();

        public ProbeCullingHandler(Action<ProbeCullingHandler> reuseHandler)
        {
            _reuseHandler = reuseHandler;
        }

        public override void StateChanged(bool value)
        {
            var count = _lightprobeRenderers.Count;
            for (int i = 0; i < count; i++)
            {
                _lightprobeRenderers[i].lightProbeUsage = value ? LightProbeUsage.BlendProbes : LightProbeUsage.Off;
            }

            count = _reflectionprobeRenderers.Count;
            for (int i = 0; i < count; i++)
            {
                _reflectionprobeRenderers[i].reflectionProbeUsage =
                    value ? ReflectionProbeUsage.BlendProbes : ReflectionProbeUsage.Off;
            }
        }

        public override object Clone()
        {
            return new ProbeCullingHandler(_reuseHandler);
        }

        public override void Reset()
        {
            base.Reset();
            _lightprobeRenderers.Clear();
            _reflectionprobeRenderers.Clear();
        }

        public override void Free()
        {
            _reuseHandler(this);
        }

        public void AddLightprobe(MeshRenderer renderer)
        {
            _lightprobeRenderers.Add(renderer);
        }

        public void AddReflectionProbe(MeshRenderer renderer)
        {
            _reflectionprobeRenderers.Add(renderer);
        }
    }
}