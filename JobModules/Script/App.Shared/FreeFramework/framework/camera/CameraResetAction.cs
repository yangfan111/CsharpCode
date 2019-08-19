using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;

namespace App.Shared.FreeFramework.framework.camera
{
    [Serializable]
    public class CameraResetAction : AbstractPlayerAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);
            if (player != null && player.gamePlay.CameraEntityId != 0)
            {
                player.gamePlay.CameraEntityId = 0;

                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.PlayerObserveTrigger;
                sp.Bs.Add(false);
                FreeMessageSender.SendMessage(player, sp);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.CameraResetAction;
        }
    }
}
