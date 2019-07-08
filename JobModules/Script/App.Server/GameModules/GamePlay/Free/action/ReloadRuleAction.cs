using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class ReloadRuleAction : AbstractGameAction, IRule
    {
        private string rule;

        public override void DoAction(IEventArgs args)
        {
            FreeGameRule free = (FreeGameRule)args.Rule;
            free.Reload(args.GameContext, this.rule);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.ReloadRuleAction;
        }
    }
}
