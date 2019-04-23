using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;
using UnityEngine;
using WindowsInput.Native;

namespace App.Shared.FreeFramework.framework.ai
{
    [Serializable]
    public class PlayerPressKeyAction : AbstractPlayerAction
    {
        private string key;
        private string time;
        
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            SimpleProto msg = FreePool.Allocate();
            msg.Key = FreeMessageConstant.PlayerPressKey;
            msg.Ins.Add(args.GetInt(key));
            msg.Ins.Add(args.GetInt(time));
            FreeMessageSender.SendMessage(player, msg);
        }
    }
}
