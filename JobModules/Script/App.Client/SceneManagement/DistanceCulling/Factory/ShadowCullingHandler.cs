using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace App.Client.SceneManagement.DistanceCulling.Factory
{
    class ShadowCullingHandler : CullingHandler
    {
        private readonly Action<ShadowCullingHandler> _reuseHandler;
        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();

        public ShadowCullingHandler(Action<ShadowCullingHandler> reuseHandler)
        {
            _reuseHandler = reuseHandler;
        }

        public override void StateChanged(bool value)
        {
            var count = _renderers.Count;
            for (int i = 0; i < count; i++)
            {
               
                _renderers[i].shadowCastingMode = value ? ShadowCastingMode.On : ShadowCastingMode.Off;
            }
        }

        public override object Clone()
        {
            return new ShadowCullingHandler(_reuseHandler);
        }

        public override void Reset()
        {
            base.Reset();
            _renderers.Clear();
        }

        public override void Free()
        {
            _reuseHandler(this);
        }

        public void Add(MeshRenderer renderer)
        {
            _renderers.Add(renderer);
        }
    }
}