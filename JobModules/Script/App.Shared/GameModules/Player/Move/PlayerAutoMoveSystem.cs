using App.Shared.Components.Player;
using App.Shared.Player;
using Core.Compare;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;

namespace App.Shared.GameModules.Player.Move
{
    public class PlayerAutoMoveSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity) owner.OwnerEntity;
            
            CheckPlayerLifeState(player);
            InterruptAutoRun(player, cmd);

            if (cmd.IsSwitchAutoRun)
            {
                player.playerMove.IsAutoRun = !player.playerMove.IsAutoRun;
                if (player.playerMove.IsAutoRun)
                {
                    PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, player.gamePlay);
                }
            }
        }

        private void InterruptAutoRun(PlayerEntity player, IUserCmd cmd)
        {
            var actionState = player.stateInterface.State.GetActionState();
            var curPostureState = player.stateInterface.State.GetCurrentPostureState();
            var nextPostureState = player.stateInterface.State.GetNextPostureState();

            if (!CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0) ||
                !CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0))
                player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
            else if ((cmd.IsPeekRight || cmd.IsPeekLeft) &&
                     curPostureState != PostureInConfig.Prone &&
                     curPostureState != PostureInConfig.Swim &&
                     curPostureState != PostureInConfig.Dive)
                player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
            else if (curPostureState != nextPostureState &&
                     nextPostureState != PostureInConfig.Dive &&
                     nextPostureState != PostureInConfig.Swim &&
                     nextPostureState != PostureInConfig.Land &&
                     nextPostureState != PostureInConfig.Jump &&
                     nextPostureState != PostureInConfig.Stand)
                player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
            else
            {
                switch (actionState)
                {
                    case ActionInConfig.MeleeAttack:
                    case ActionInConfig.SwitchWeapon:
                    case ActionInConfig.PickUp:
                    case ActionInConfig.Reload:
                    case ActionInConfig.SpecialReload:
                        player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                        break;
                }
            }
        }
        
        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.playerGameState) return;
            var gameState = player.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }

        private static void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var autoMove = player.autoMoveInterface.PlayerAutoMove;
            if (null == autoMove) return;
            autoMove.StopAutoMove();
        }
        
        private static void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var autoMove = player.autoMoveInterface.PlayerAutoMove;
            if (null == autoMove) return;
            autoMove.StopAutoMove();
        }

        #endregion
    }
}