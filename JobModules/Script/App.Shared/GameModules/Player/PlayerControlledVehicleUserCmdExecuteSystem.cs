using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using EVP;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace App.Shared.GameModules.Player
{
    //hack code: delete after we have a general camera 
    public class PlayerControlledVehicleUserCmdExecuteSystem : IUserCmdExecuteSystem
    {
      
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity) owner.OwnerEntity;
            if (player.IsOnVehicle())
            {
                player.position.AlwaysEqual = true;
                player.orientation.AlwaysEqual = true;

            }
            else
            {
                player.position.AlwaysEqual = false;
                player.orientation.AlwaysEqual = false;

            }
        }
    }
}
