using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Components;
using Core.GameModule.Interface;
using Core.OC;
using Core.Utils;
using UnityEngine;
using UnityEngine.Profiling;
using Utils.Singleton;

namespace App.Client.GameModules.OC
{
    public class OcclusionCullingSystem : IRenderSystem
    {
        private IOcclusionCullingController _ocController;
        private Sampler _dynamicObjectsUmbraSampler;
        private Sampler _objectsWithoutUmbraSampler;
        private CustomProfileInfo _dynamicObjectsUmbraProfileInfo;
        private CustomProfileInfo _objectsWithoutUmbraProfileInfo;
        public OcclusionCullingSystem(Contexts contexts)
        {
            _ocController = contexts.session.clientSessionObjects.OCController;

            _dynamicObjectsUmbraSampler = Sampler.Get("CullDynamicObjectsWithUmbra");
            _objectsWithoutUmbraSampler = Sampler.Get("CullObjectsWithoutUmbra");
            _dynamicObjectsUmbraProfileInfo = SingletonManager.Get<DurationHelp>().GetProfileInfo(CustomProfilerStep.CullDynamicObjectsWithUmbra);
            _objectsWithoutUmbraProfileInfo = SingletonManager.Get<DurationHelp>().GetProfileInfo(CustomProfilerStep.CullObjectsWithoutUmbra);
        }

        public void OnRender()
        {
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.OC);
            _ocController.DoCulling(Camera.main.transform.position.WorldPosition());
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.OC);

            var umbraTime1 = _dynamicObjectsUmbraSampler.GetRecorder().elapsedNanoseconds * 10e-6f;
            _dynamicObjectsUmbraProfileInfo.Total += umbraTime1;
            _dynamicObjectsUmbraProfileInfo.Times += 1;

            var umbraTime2 = _objectsWithoutUmbraSampler.GetRecorder().elapsedNanoseconds * 10e-6f;
            _objectsWithoutUmbraProfileInfo.Total += umbraTime2;
            _objectsWithoutUmbraProfileInfo.Times += 1;
        }
    }
}
