using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace com.wd.free.ai
{
    [Serializable]
    public class FinishStepAiAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {
            args.FreeContext.AiSuccess = true;
        }
    }
}
