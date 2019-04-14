using System;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.EntityFactory;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.WeaponLogic;
using UnityEngine;
using Random = System.Random;
namespace App.Shared.GameModules.Player
{
    public class PlayerClientTimeSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAttackSystem));

       
        public PlayerClientTimeSystem()
        {
            
        }

      
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;
            playerEntity.time.ClientTime += cmd.FrameInterval;
          
        }
    }
}