using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.SceneManagement;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.SessionState;
using UnityEngine;

namespace App.Client.Tools
{
    public class AutoTerrainNavigatorSystem : AbstractStepExecuteSystem
    {
        
        
        public AutoTerrainNavigatorSystem(Contexts contexts)
        {
            if (App.Shared.SharedConfig.InSamplingMode || App.Shared.SharedConfig.InLegacySampleingMode)                                         // 仅在开启SampleFPS选项时挂载脚本
            {
                GameObject gameController = GameObject.Find("GameController");
                TerrainSampler sampler = gameController.AddComponent<TerrainSampler>();
                sampler.PlayerContext = contexts.player;
                sampler.LevelManager = contexts.session.commonSession.LevelManager;
            }
        }


        protected override void InternalExecute()
        {
            
        }
    }
}
