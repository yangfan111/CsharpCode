using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Configuration;
using Core.OC;
using UnityEngine;

namespace App.Client.GameModules.OC
{

    public class DummyOCController : IOcclusionCullingController
    {
        public DummyOCController()
        {
            OcclusionRunningState.HasPVSData = false;
            OcclusionRunningState.OcclusionEnabled = SharedConfig.EnableCustomOC;
        }

        public void DoCulling(Vector3 position)
        {
            
        }

        public void Dispose()
        {

        }
    }

    public static class OcclisionCullingControllerFactory
    {
        public static IOcclusionCullingController CreateController(LevelType levelType, OCParam param)
        {
            if (param.OCData != null)
            {
                switch (levelType)
                {
                    case LevelType.SmallMap:
                        return new FixedSceneOCController(param);
                    case LevelType.BigMap:
                        return  new StreamSceneOCController(param);
                }
            }

            return new DummyOCController();
        }
    }
}
