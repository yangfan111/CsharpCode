using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;

namespace com.wd.free.action.stage
{
    [Serializable]
    public class TimeLastAction : AbstractGameAction
    {
        public string lastTime;

        public IGameAction action;

        [NonSerialized]
        private long startTime;

        [NonSerialized]
        private int realLastTime;

        public override void DoAction(IEventArgs args)
        {
            if (realLastTime == 0)
            {
                realLastTime = FreeUtil.ReplaceInt(lastTime, args);
            }

            if (startTime == 0)
            {
                startTime = args.Rule.ServerTime;
            }

            if (args.Rule.ServerTime - startTime < realLastTime)
            {
                if (action != null)
                {
                    action.Act(args);
                }
            }
        }
    }
}
