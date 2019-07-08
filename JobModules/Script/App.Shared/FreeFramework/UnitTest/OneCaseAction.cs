using com.wd.free.action;
using com.wd.free.ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.trigger;
using Core.Free;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class OneCaseAction : AbstractGameAction, IRule
    {
        [NonSerialized]
        public GameTrigger trigger;

        public IGameAction order;
        public IGameAction frame;
        public IGameAction clean;
        public IGameAction prepare;

        private bool initialed;

        public override void DoAction(IEventArgs args)
        {
            if (!initialed)
            {
                if (prepare != null)
                {
                    prepare.Act(args);
                }
                initialed = true;
            }
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

                if (clean != null)
                {
                    clean.Act(args);
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
            initialed = false;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.OneCaseAction;
        }
    }
}
