using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;

namespace App.Shared.FreeFramework.framework.ai
{
    [Serializable]
    public class PlayerPressKeyAction : AbstractPlayerAction
    {
        private string press;
        private string key;
        private string time;
        
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            SimpleProto msg = FreePool.Allocate();
            msg.Key = FreeMessageConstant.PlayerPressKey;
            msg.Bs.Add(args.GetBool(press));
            msg.Ss.Add(key);
            msg.Ins.Add(args.GetInt(time));
            FreeMessageSender.SendMessage(player, msg);
        }
    }
}
