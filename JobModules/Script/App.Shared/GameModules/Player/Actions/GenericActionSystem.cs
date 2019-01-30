using App.Shared.Components.GenericActions;
using App.Shared.Components.Player;
using App.Shared.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions
{
    public class GenericActionSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(GenericActionSystem));
        private IGenericAction _genericAction;
        public GenericActionSystem()
        {
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity)owner.OwnerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead) ||
                !player.hasGenericActionInterface ||
                player.IsOnVehicle())
            {
                // gamePlay有对应的处理，这里不需要
                return;
            }

            _genericAction = player.genericActionInterface.GenericAction;

            _genericAction.Update(player);
            if (cmd.IsJump)
                TriggerActionInput(player);
        }

        private void TriggerActionInput(PlayerEntity player)
        {
            _genericAction.ActionInput(player);
        }
    }
}
