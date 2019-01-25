using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.trigger;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class TestCaseMultiAction : AbstractTestCaseAction
    {
        private Dictionary<string, List<TestValue>> dic = new Dictionary<string, List<TestValue>>();

        private List<ITestValue> values;

        public override void DoAction(IEventArgs args)
        {
            if (values != null)
            {
                args.TempUse(UnitTestConstant.Tester, GetPlayer(args));

                GameTrigger trigger = FreeLog.GetTrigger();

                foreach (ITestValue value in values)
                {
                    TestValue tv = value.GetCaseValue(args);
                    tv.Name = value.Name;

                    if (!dic.ContainsKey(tv.Name))
                    {
                        dic.Add(tv.Name, new List<TestValue>());
                        dic[tv.Name].Add(tv);
                    }

                    List<TestValue> list = dic[tv.Name];

                    if (!list[list.Count - 1].IsSame(tv))
                    {
                        list.Add(tv);
                    }
                }

                args.Resume(UnitTestConstant.Tester);
            }
        }

        public void Record(IEventArgs args)
        {
            foreach (var t in dic)
            {
                TestValue[] tvs = t.Value.ToArray();
                RecordResult(args, FreeLog.GetTrigger(), tvs);
            }
        }

        public override void Reset(IEventArgs args)
        {
            dic.Clear();
        }
    }
}
