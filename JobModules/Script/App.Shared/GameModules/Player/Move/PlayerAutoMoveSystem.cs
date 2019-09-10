using App.Shared.Components.Player;
using Core.Compare;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;

namespace App.Shared.GameModules.Player.Move
{
    public class PlayerAutoMoveSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var player = (PlayerEntity) getter.OwnerEntity;

            CheckPlayerLifeState(player);
            InterruptAutoRun(player, cmd);
            if(cmd.IsSwitchAutoRun && CanAutoRun(player))
            {
                player.playerMove.IsAutoRun = !player.playerMove.IsAutoRun;
                if (player.playerMove.IsAutoRun)
                {
                    player.stateInterface.State.InterruptAction();
                    PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, player.gamePlay);
                }
            }
        }

        private static bool CanAutoRun(PlayerEntity player)
        {
            return CanAutoRunByActionState(player) &&
                   CanAutoRunByPostureState(player) &&
                   CanAutoRunByMovementState(player);
        }

        private static bool CanAutoRunByActionState(PlayerEntity player)
        {
            var actionState = player.stateInterface.State.GetActionState();
            var keepActionState = player.stateInterface.State.GetActionKeepState();
            return ActionKeepInConfig.Sight != keepActionState &&
                   (ActionInConfig.Injury == actionState ||
                   ActionInConfig.Null == actionState ||
                   ActionInConfig.Props == actionState);
        }

        private static bool CanAutoRunByPostureState(PlayerEntity player)
        {
            var postureState = player.stateInterface.State.GetCurrentPostureState();
            return PostureInConfig.Stand == postureState ||
                   PostureInConfig.Crouch == postureState ||
                   PostureInConfig.Prone == postureState ||
                   PostureInConfig.Swim == postureState ||
                   PostureInConfig.Dive == postureState;
        }

        private static bool CanAutoRunByMovementState(PlayerEntity player)
        {
            var movementState = player.stateInterface.State.GetCurrentMovementState();
            return MovementInConfig.Idle == movementState ||
                   MovementInConfig.Walk == movementState ||
                   MovementInConfig.Run == movementState ||
                   MovementInConfig.Sprint == movementState;
        }

        private static void InterruptAutoRun(PlayerEntity player, IUserCmd cmd)
        {
            if(!player.playerMove.IsAutoRun) return;
            if (!CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0) ||
                !CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0) ||
                NeedInterruptAutoRun(player))
                player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
        }

        private static bool NeedInterruptAutoRun(PlayerEntity player)
        {
            return NeedInterruptAutoRunByActionState(player) ||
                   NeedInterruptAutoRunByKeepActionState(player) ||
                   NeedInterruptAutoRunByPostureState(player) ||
                   NeedInterruptAutoRunByMovementState(player) ||
                   NeedInterruptAutoRunByLeanState(player);
        }

        private static bool NeedInterruptAutoRunByActionState(PlayerEntity player)
        {
            var actionState = player.stateInterface.State.GetActionState();
            return ActionInConfig.Null != actionState;
        }

        private static bool NeedInterruptAutoRunByKeepActionState(PlayerEntity player)
        {
            var keepActionState = player.stateInterface.State.GetActionKeepState();
            return ActionKeepInConfig.Null != keepActionState;
        }

        private static bool NeedInterruptAutoRunByPostureState(PlayerEntity player)
        {
            var postureState = player.stateInterface.State.GetCurrentPostureState();
            var nextPostureState = player.stateInterface.State.GetNextPostureState();

            if (PostureInConfig.Swim == postureState ||
                PostureInConfig.Swim == nextPostureState ||
                PostureInConfig.Dive == postureState ||
                PostureInConfig.Jump == postureState ||
                PostureInConfig.Land == postureState)
                return false;

            return postureState != nextPostureState;
        }

        private static bool NeedInterruptAutoRunByMovementState(PlayerEntity player)
        {
            var movementState = player.stateInterface.State.GetCurrentMovementState();
            return MovementInConfig.Sprint != movementState &&
                   MovementInConfig.Run != movementState &&
                   MovementInConfig.Walk != movementState &&
                   MovementInConfig.Idle != movementState;
        }

        private static bool NeedInterruptAutoRunByLeanState(PlayerEntity player)
        {
            var leanState = player.stateInterface.State.GetCurrentLeanState();
            return LeanInConfig.NoPeek != leanState;
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
