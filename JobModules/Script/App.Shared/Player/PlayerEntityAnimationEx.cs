using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Core.Utils;

namespace App.Shared.Player
{
    /// <summary>
    /// 处理玩家动作，有些动作需要有组合逻辑，比如要先收枪，只在客户端执行
    /// </summary>
    public static class PlayerEntityAnimationEx
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityAnimationEx));
        public static void StopAnimation(this PlayerEntity playerEntity)
        {
            playerEntity.stateInterface.State.InterruptAction();
            if(playerEntity.hasAnimationExData)
            {
                if(null != playerEntity.animationExData.ActionAfterAllAnimation)
                {
                    playerEntity.animationExData.ActionAfterAllAnimation();
                }
                ResetAnimData(playerEntity);
            }
            else
            {
                Logger.Error("player has no animationex component");
            }
        }

        public static void DefuseBomb(this PlayerEntity playerEntity, Contexts contexts)
        {
            if(!playerEntity.hasAnimationExData)
            {
                playerEntity.stateInterface.State.DismantleBomb(null);
                Logger.Error("player has no animationex component");
                return;
            }
            ResetAnimData(playerEntity);
            playerEntity.animationExData.AnimationWaitToBePlayed = () => playerEntity.stateInterface.State.DismantleBomb(playerEntity.animationExData.ActionAfterAllAnimation);
            UndrawWeaponThenPlayAnim(playerEntity, contexts);
        }

        private static void ResetAnimData(PlayerEntity playerEntity)
        {
            playerEntity.animationExData.AnimationWaitToBePlayed = null;
            playerEntity.animationExData.ActionAfterAllAnimation = null;
        }

        private static void UndrawWeaponThenPlayAnim(this PlayerEntity playerEntity, Contexts contexts)
        {
            if(!playerEntity.WeaponController().IsHeldSlotEmpty)
            {
                playerEntity.animationExData.ActionAfterAllAnimation = () =>
                {
                    playerEntity.freeUserCmd.MountWeapon = true;
                    playerEntity.animationExData.ActionAfterAllAnimation = null;
                };

                //不需要收枪动作
                playerEntity.freeUserCmd.ForceUnmountWeapon = true;

                if(null != playerEntity.animationExData.AnimationWaitToBePlayed)
                {
                    playerEntity.animationExData.AnimationWaitToBePlayed();
                    playerEntity.animationExData.AnimationWaitToBePlayed = null;
                }
            }
            else
            {
                if(null != playerEntity.animationExData.AnimationWaitToBePlayed)
                {
                    playerEntity.animationExData.AnimationWaitToBePlayed();
                    playerEntity.animationExData.AnimationWaitToBePlayed = null;
                }
            }
        }
    }
}