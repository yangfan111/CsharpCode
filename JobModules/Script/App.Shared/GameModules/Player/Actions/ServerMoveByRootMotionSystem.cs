using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions
{
    class ServerMoveByRootMotionSystem: IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity)owner.OwnerEntity;
            var clientData = player.playerMoveByAnimUpdate;
            
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead) || !clientData.NeedUpdate)
            {
                return;
            }

            player.RootGo().transform.position = player.position.Value = clientData.Position;
            player.orientation.ModelPitch = clientData.ModelPitch;
            player.orientation.ModelYaw = clientData.ModelYaw;
            
            if(player.hasPlayerMove)
                player.playerMove.Velocity = Vector3.zero;
            if(player.hasMoveUpdate)
                player.moveUpdate.Velocity = Vector3.zero;
        }
    }
}
