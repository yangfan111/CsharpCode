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
    public class PlayerClimbActionSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerClimbActionSystem));
        private IGenericAction _genericAction;
        public PlayerClimbActionSystem()
        {
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity)owner.OwnerEntity;
            
            CheckPlayerLifeState(player);
            
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead) ||
                !player.hasGenericActionInterface ||
                player.IsOnVehicle())
                return;

            _genericAction = player.genericActionInterface.GenericAction;
            _genericAction.Update(player);
            
            if (cmd.IsJump && CanClimb(player))
                TriggerActionInput(player);
        }

        private void TriggerActionInput(PlayerEntity player)
        {
            _genericAction.ActionInput(player);
        }

        private static bool CanClimb(PlayerEntity player)
        {
            var postureState = player.stateInterface.State.GetCurrentPostureState();
            return PostureInConfig.Jump != postureState && PostureInConfig.Climb != postureState;
        }
        
        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.gamePlay) return;

            var gamePlay = player.gamePlay;
            if (!gamePlay.HasLifeStateChangedFlag()) return;
            if(CreatePlayerGameStateData(player)) return;

            if (gamePlay.IsLifeState(EPlayerLifeState.Alive) &&
                gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
                Reborn(player);

            if (gamePlay.IsLifeState(EPlayerLifeState.Dead))
                Dead(player);
        }
        
        private static bool CreatePlayerGameStateData(PlayerEntity player)
        {
            var gamePlay = player.gamePlay;
            var playerGameState = player.playerGameState;
            if(null == playerGameState || null == gamePlay) return true;
            
            if (PlayerSystemEnum.PlayerClimbUpdate == playerGameState.CurrentPlayerSystemState)
            {
                _logger.InfoFormat("ChangeClearInSystem:  {0}", playerGameState.CurrentPlayerSystemState);
                gamePlay.ClearLifeStateChangedFlag();
                playerGameState.CurrentPlayerSystemState = PlayerSystemEnum.NullState;
                return true;
            }
            
            if (PlayerSystemEnum.NullState == playerGameState.CurrentPlayerSystemState)
                playerGameState.CurrentPlayerSystemState = PlayerSystemEnum.PlayerClimbUpdate;

            return false;
        }

        private void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var genericAction = player.genericActionInterface.GenericAction;
            if (null == genericAction) return;
            genericAction.PlayerReborn(player);
        }
        
        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var genericAction = player.genericActionInterface.GenericAction;
            if (null == genericAction) return;
            genericAction.PlayerDead(player);
            _logger.InfoFormat("PlayerClimbDead");
        }

        #endregion
    }
}
