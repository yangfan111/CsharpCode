using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using Core.EntityComponent;
using Core.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Shared.Util
{
    public static class SceneObjectUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SceneObjectUtility));

        public static bool IsCanPickUpByPlayer(this SceneObjectEntity sceneObjectEntity, PlayerEntity playerEntity)
        {
            if (!sceneObjectEntity.hasCastFlag)
            {
                return true;
            }
            return PlayerStateUtil.HasCastState((EPlayerCastState)sceneObjectEntity.castFlag.Flag, playerEntity.gamePlay); 
        }
        
    }
}
