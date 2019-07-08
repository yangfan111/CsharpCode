using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.para;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class GameOverAction : SendMessageAction, IRule
    {
        public int GetRuleID()
        {
            return (int)ERuleIds.GameOverAction;
        }

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.GameOver;
        }
    }
}
