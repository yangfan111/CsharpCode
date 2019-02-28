using System;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;
using gameplay.gamerule.free.ui;

namespace App.Server.GameModules.GamePlay.Free.action.ui
{
    [Serializable]
    public class LockMouseAction : SendMessageAction
    {
        protected override void BuildMessage(IEventArgs args)
        {
            this.scope = "4";

            builder.Key = FreeMessageConstant.LockMouse;

            //True:Lock  False:Unlock
            builder.Bs.Add(FreeUtil.ReplaceBool("{lock}", args));
        }
    }
}
