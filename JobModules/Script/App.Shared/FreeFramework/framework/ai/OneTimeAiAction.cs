using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace com.wd.free.ai
{
    [Serializable]
    public class OneTimeAiAction :AbstractGameAction, IRule
    {
        public IGameAction action;

        public override void DoAction(IEventArgs args)
        {
            args.FreeContext.AiSuccess = true;

            if(action != null)
            {
                action.Act(args);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.OneTimeAiAction;
        }
    }
}
