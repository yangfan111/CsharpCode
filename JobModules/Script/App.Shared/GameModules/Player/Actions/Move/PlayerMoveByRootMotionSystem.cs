using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions.Move
{
    public class PlayerMoveByRootMotionSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity)owner.OwnerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                // gamePlay有对应的处理，这里不需要
                return;
            }

            UpdateTransform(player, cmd.FrameInterval / 1000.0f);
        }

        private void UpdateTransform(PlayerEntity player, float deltaTime)
        {
            //rootMotion控制人物移动
            if (!player.thirdPersonAnimator.UnityAnimator.applyRootMotion)
            {
                player.playerMoveByAnimUpdate.NeedUpdate = false;
                return;
            }

            if (UseLadderMove(player))
            {
                LadderMove.UpdateTransform(player, deltaTime);
                return;
            }
            
            GenericMove.UpdateTransform(player, deltaTime);
        }

        private static bool UseLadderMove(PlayerEntity player)
        {
            if (!player.hasAppearanceInterface || !player.hasStateInterface) return false;
            return MovementInConfig.Ladder == player.stateInterface.State.GetCurrentMovementState() &&
                   player.appearanceInterface.Appearance.GetWeaponIdInHand() != UniversalConsts.InvalidIntId;
        }
    }
}
