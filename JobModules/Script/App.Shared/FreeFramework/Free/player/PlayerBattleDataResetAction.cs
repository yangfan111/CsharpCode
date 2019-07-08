using System;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using com.wd.free.para;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerBattleDataResetAction : AbstractPlayerAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (null != playerEntity)
            {
                playerEntity.statisticsData.Battle.Reset();
                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.ResetBattleData;
                FreeMessageSender.SendMessage(playerEntity, sp);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerBattleDataResetAction;
        }
    }
}
