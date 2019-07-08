using System;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;
using gameplay.gamerule.free.ui;
using com.wd.free.para;

namespace App.Server.GameModules.GamePlay.Free.action.ui
{
    [Serializable]
    public class LockMouseAction : SendMessageAction, IRule
    {
        public int GetRuleID()
        {
            return (int)ERuleIds.LockMouseAction;
        }

        protected override void BuildMessage(IEventArgs args)
        {
            this.scope = "4";

            builder.Key = FreeMessageConstant.LockMouse;

            //True:Lock  False:Unlock
            builder.Bs.Add(FreeUtil.ReplaceBool("{lock}", args));
        }
    }
}
