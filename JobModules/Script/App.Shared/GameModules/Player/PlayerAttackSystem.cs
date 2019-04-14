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
using XmlConfig;
using Random = System.Random;
namespace App.Shared.GameModules.Player
{
    public class PlayerAttackSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAttackSystem));

       
        public PlayerAttackSystem()
        {
            
        }     


        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;
            if (playerEntity.hasWeaponLogic)
            {
                PrepareAttackState(playerEntity);
                var comp = playerEntity.weaponLogic;
                comp.Weapon.Update(comp.State, cmd);
            }
        }

        private void PrepareAttackState(PlayerEntity player)
        {
            player.playerWeaponState.Reloading = 
                player.stateInterface.State.GetActionState() == ActionInConfig.Reload ||
                player.stateInterface.State.GetActionState() == ActionInConfig.SpecialReload;
        }
    }
}