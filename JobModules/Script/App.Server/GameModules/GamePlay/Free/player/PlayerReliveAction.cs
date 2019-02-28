using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using App.Shared.Components.Player;
using Core.Components;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerReliveAction : AbstractPlayerAction
    {
        private string resetWeapon;
        private string resetPosition;

        public override void DoAction(IEventArgs args)
        {
            bool rw = FreeUtil.ReplaceBool(resetWeapon, args);
            bool rp = FreeUtil.ReplaceBool(resetPosition, args);

            PlayerEntity p = GetPlayerEntity(args);
            if (p != null)
            {
                CalculateDeadTime(p);
                p.gamePlay.LifeState = (int)EPlayerLifeState.Alive;
                p.gamePlay.CurHp = p.gamePlay.MaxHp;
                p.isFlagCompensation = true;
                p.gamePlay.CurArmor = p.gamePlay.MaxArmor;
                p.gamePlay.CurHelmet = p.gamePlay.MaxHelmet;
                //p.stateInterface.State.PlayerReborn();
                p.position.InterpolateType = (int)PositionInterpolateMode.Discrete;
                p.position.ServerTime = args.GameContext.session.currentTimeObject.CurrentTime;
                p.gamePlay.InHurtedCount = 0;
                p.statisticsData.Statistics.EvenKillCount = 0;
                if (FreeUtil.ReplaceBool(resetWeapon, args))
                {
                    p.modeLogic.ModeLogic.InitDefaultWeapon(p, p.weaponState.BagIndex);
                }
                PlayerAnimationAction.DoAnimation(args.GameContext, PlayerAnimationAction.PlayerReborn, p);
            }
        }

        private void CalculateDeadTime(PlayerEntity playerEntity)
        {
            if (playerEntity.statisticsData.Statistics.LastDeadTime == 0)
            {
                playerEntity.statisticsData.Statistics.DeadTime = 0;
            }
            else if (playerEntity.gamePlay.LifeState != 1)
            {
                playerEntity.statisticsData.Statistics.DeadTime += (int)System.DateTime.Now.Ticks / 10000 - playerEntity.statisticsData.Statistics.LastDeadTime;
            }
        }
    }
}
