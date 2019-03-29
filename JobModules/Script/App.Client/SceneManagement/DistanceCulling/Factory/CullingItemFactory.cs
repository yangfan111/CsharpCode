using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Utils.AssetManager;

namespace App.Client.SceneManagement.DistanceCulling.Factory
{
    class CullingItemFactory
    {
        private IGenericPool<ShadowCullingHandler> _shadowPool;
        private IGenericPool<ProbeCullingHandler> _probePool;

        private CullingHandler _head;

        public CullingItemFactory()
        {
            var shadowPool = new GenericPool<ShadowCullingHandler>();
            shadowPool.SetMeta(new ShadowCullingHandler(shadowPool.Reuse));
            _shadowPool = shadowPool;

            var probePool = new GenericPool<ProbeCullingHandler>();
            probePool.SetMeta(new ProbeCullingHandler(probePool.Reuse));
            _probePool = probePool;
        }
            
        private List<MeshRenderer> _meshRenderTempList = new List<MeshRenderer>();
        public CullingHandler CreateCullingHandlers(UnityObject unityObj, int tag)
        {
            _head = null;
            
            var realGo = unityObj.AsGameObject;

            
            realGo.GetComponentsInChildren<MeshRenderer>(_meshRenderTempList);
            if (_meshRenderTempList.Count > 0)
            {
                CreateShadowHandler(_meshRenderTempList, tag);
                CreateProbeHandler(_meshRenderTempList, tag);
            }
            _meshRenderTempList.Clear();
            return _head;
        }

        private void CreateShadowHandler(List<MeshRenderer> renderers, int tag)
        {
            bool inNeed = false;
            var count = renderers.Count;
            var handler = _shadowPool.Get();
            
            for (int i = 0; i < count; i++)
            {
                var val = renderers[i].shadowCastingMode == ShadowCastingMode.On;
                inNeed = inNeed || val;

                if (val)
                    handler.Add(renderers[i]);
            }

            if (inNeed)
            {
                OverwriteHead(handler);

                if (MultiTagHelper.InDoor(tag))
                    handler.Category = DistCullingCat.Detail;
                else
                    handler.Category = DistCullingCat.Median;
            }
            else
                handler.Free();
        }

        private void CreateProbeHandler(List<MeshRenderer> renderers, int tag)
        {
            bool inNeed = false;
            var count = renderers.Count;
            var handler = _probePool.Get();

            for (int i = 0; i < count; i++)
            {
                var val = renderers[i].lightProbeUsage == LightProbeUsage.BlendProbes;
                inNeed = inNeed || val;
                
                if (val)
                    handler.AddLightprobe(renderers[i]);

                val = renderers[i].reflectionProbeUsage == ReflectionProbeUsage.BlendProbes;
                inNeed = inNeed || val;
                
                if (val)
                    handler.AddReflectionProbe(renderers[i]);
            }

            if (inNeed)
            {
                OverwriteHead(handler);

                if (MultiTagHelper.InDoor(tag))
                    handler.Category = DistCullingCat.Detail;
                else
                    handler.Category = DistCullingCat.Near;
            }
            else
                handler.Free();
        }

        private void OverwriteHead(CullingHandler handler)
        {
            if (_head != null)
                handler.Sibling = _head;

            _head = handler;
        }
    }
}