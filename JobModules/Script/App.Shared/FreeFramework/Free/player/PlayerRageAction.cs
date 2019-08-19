using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerRageAction : AbstractPlayerAction, IRule
    {
        private bool rage;
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity entity = GetPlayerEntity(args);
            if (entity != null && entity.hasStateInterface) {
                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.PlayerRageStart;
                if (rage) {
                    entity.stateInterface.State.RageStart();
                    sp.Bs.Add(true);
                } else {
                    entity.stateInterface.State.RageEnd();
                    sp.Bs.Add(false);
                }
                FreeMessageSender.SendMessage(entity, sp);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerRageAction;
        }
    }
}
