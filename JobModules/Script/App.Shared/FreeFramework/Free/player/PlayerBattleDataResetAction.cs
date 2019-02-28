using System;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerBattleDataResetAction : AbstractPlayerAction
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
    }
}
