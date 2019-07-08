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
    public class FinishStepAiAction : AbstractGameAction , IRule
    {
        public override void DoAction(IEventArgs args)
        {
            args.FreeContext.AiSuccess = true;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.FinishStepAiAction;
        }
    }
}
