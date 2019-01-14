using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.para;

namespace com.wd.free.ai
{
    [Serializable]
    public class WaitTimeAiAction : AbstractGameAction
    {
        private string time;

        private IGameAction action;

        private IParaCondition condition;

        private long startTime;

        private int realTime;

        private IntPara timePara;

        private const string Time = "time";

        public override void DoAction(IEventArgs args)
        {
            long serverTime = args.Rule.ServerTime;

            if (!string.IsNullOrEmpty(time) && realTime == 0)
            {
                realTime = args.GetInt(time);
            }

            if (startTime == 0)
            {
                startTime = serverTime;
                timePara = new IntPara(Time, 0);
            }

            timePara.SetValue(serverTime - startTime);
            args.TempUsePara(timePara);

            if (action != null)
            {
                action.Act(args);
            }

            if (realTime > 0 && serverTime - startTime >= realTime)
            {
                args.FreeContext.AiSuccess = true;
                startTime = 0;
            }

            if (condition != null && condition.Meet(args))
            {
                args.FreeContext.AiSuccess = true;
                startTime = 0;
            }

            args.ResumePara(Time);
        }

        public override void Reset(IEventArgs args)
        {
            startTime = 0;
        }
    }
}
