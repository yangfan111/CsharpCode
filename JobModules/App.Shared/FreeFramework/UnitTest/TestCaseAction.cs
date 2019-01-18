using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Utils;
using com.wd.free.trigger;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class TestCaseAction : AbstractTestCaseAction
    {
        static LoggerAdapter logger = new LoggerAdapter("UnitTest");

        private List<ITestValue> values;

        public override void DoAction(IEventArgs args)
        {
            if (values != null)
            {
                args.TempUse(UnitTestConstant.Tester, GetPlayer(args));

                GameTrigger trigger = FreeLog.GetTrigger();

                foreach (var value in values)
                {
                    TestValue tv = value.GetCaseValue(args);
                    tv.Name = value.Name;
                    RecordResult(args, trigger, new TestValue[] { tv });
                }

                args.Resume(UnitTestConstant.Tester);
            }
        }
    }
}
