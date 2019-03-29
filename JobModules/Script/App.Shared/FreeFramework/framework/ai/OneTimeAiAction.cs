using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace com.wd.free.ai
{
    [Serializable]
    public class OneTimeAiAction :AbstractGameAction
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
    }
}
