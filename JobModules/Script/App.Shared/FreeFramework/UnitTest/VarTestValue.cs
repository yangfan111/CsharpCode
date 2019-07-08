using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class VarTestValue : AbstractTestValue, IRule
    {
        public string exp;
        public bool number;

        public override TestValue GetCaseValue(IEventArgs args)
        {
            TestValue tv = new TestValue();

            if (number)
            {
                tv.AddField(exp, args.getDouble(exp));
            }
            else
            {
                tv.AddField(exp, args.GetString(exp));
            }

            return tv;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.VarTestValue;
        }
    }
}
