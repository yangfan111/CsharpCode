using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class StartUnitTestAction : AbstractGameAction, IRule
    {
        private int action;

        public override void DoAction(IEventArgs args)
        {
            if (action == 1)
            {
                args.FreeContext.TestCase.Start(args);
            }
            if (action == 2)
            {
                args.FreeContext.TestCase.Pause(args);
            }
            if (action == 3)
            {
                args.FreeContext.TestCase.Resume(args);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.StartUnitTestAction;
        }
    }
}
