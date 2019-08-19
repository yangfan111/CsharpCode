using App.Shared;
using App.Shared.Components.Player;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Components;
using Core.EntityComponent;
using Core.Free;
using System;
using Core.EntityComponent;

namespace App.Server.GameModules.GamePlay.Free.player
{
    /// <summary>
    /// Defines the <see cref="PlayerReliveAction" />
    /// </summary>
    [Serializable]
    public class PlayerReliveAction : AbstractPlayerAction, IRule
    {
        private string resetWeapon;

        private string resetPosition;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);
            if (player != null)
            {
                CalculateDeadTime(player, args);
                player.gamePlay.LifeState = (int)EPlayerLifeState.Alive;
                player.gamePlay.IsStandPosture = true;
                player.gamePlay.CurHp = player.gamePlay.MaxHp;
                player.isFlagCompensation = true;
                player.oxygenEnergyInterface.Oxygen.ResetOxygen(true);
                player.position.InterpolateType = (int) PositionInterpolateMode.Discrete;
                player.position.ServerTime = args.GameContext.session.currentTimeObject.CurrentTime;
                player.gamePlay.InHurtedCount = 0;
                player.statisticsData.Statistics.EvenKillCount = 0;
                player.WeaponController().RelatedThrowAction.ThrowingEntityKey = new EntityKey(0, (short) EEntityType.End);
                player.WeaponController().RelatedThrowAction.LastFireWeaponKey = -1;
                PlayerAnimationAction.DoAnimation(args.GameContext, PlayerAnimationAction.PlayerReborn, player);
                if (FreeUtil.ReplaceBool(resetWeapon, args))
                {
                    player.ModeController().RecoverPlayerWeapon(player, player.WeaponController().HeldBagPointer);
                }
            }
        }

        private void CalculateDeadTime(PlayerEntity playerEntity, IEventArgs args)
        {
            if (playerEntity.statisticsData.Statistics.LastDeadTime == 0L)
            {
                if (playerEntity.gamePlay.IsDead())
                {
                    playerEntity.statisticsData.Statistics.GameJoinTime = args.Rule.ServerTime;
                }
                playerEntity.statisticsData.Statistics.DeadTime = 0;
            }
            else if (playerEntity.gamePlay.IsDead())
            {
                playerEntity.statisticsData.Statistics.DeadTime += (int) (args.Rule.ServerTime - playerEntity.statisticsData.Statistics.LastDeadTime);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerReliveAction;
        }
    }
}
