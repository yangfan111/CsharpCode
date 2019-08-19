using System.Collections.Generic;
using App.Client.Scripts;
using App.Shared;
using Core.SessionState;
using UnityEngine;
using Utils.Singleton;
using XmlConfig.BootConfig;

namespace App.Client.Tools
{
    public class AutoTerrainNavigatorSystem : AbstractStepExecuteSystem
    {

        public AutoTerrainNavigatorSystem(Contexts contexts)
        {
            if (!SharedConfig.InSamplingMode && !SharedConfig.InLegacySampleingMode) return;

            GameObject controller = GameObject.Find("GameController");
            if (controller == null)
            {
                Debug.LogError("AutoTerrainNavigatorSystem error, can't get GameController");
                return;
            }


            TerrainSampler sampler = controller.GetComponent<TerrainSampler>();
            if (sampler == null) sampler = controller.AddComponent<TerrainSampler>();
            sampler.StartSample(contexts);
        }

        protected override void InternalExecute()
        {
        }
    }
}
