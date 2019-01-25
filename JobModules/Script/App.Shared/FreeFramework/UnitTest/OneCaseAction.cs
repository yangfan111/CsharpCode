using com.wd.free.action;
using com.wd.free.ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.trigger;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class OneCaseAction : AbstractGameAction
    {
        [NonSerialized]
        public GameTrigger trigger;

        public IGameAction order;
        public IGameAction frame;

        public override void DoAction(IEventArgs args)
        {
            FreeLog.SetTrigger(trigger);
            order.Act(args);
            if (frame != null)
            {
                frame.Act(args);
            }
            if (args.FreeContext.AiSuccess)
            {
                if (frame is TestCaseMultiAction)
                {
                    ((TestCaseMultiAction)frame).Record(args);
                }
            }
        }

        public override void Reset(IEventArgs args)
        {
            order.Reset(args);
            if (frame != null)
            {
                frame.Act(args);
            }
        }
    }
}
