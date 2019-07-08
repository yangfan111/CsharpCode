using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace com.wd.free.condition
{
    [Serializable]
    public class NotParaCondition : IParaCondition, IRule
    {
        private IParaCondition condition;

        public int GetRuleID()
        {
            return (int)ERuleIds.NotParaCondition;
        }

        public bool Meet(IEventArgs args)
        {
            return !condition.Meet(args);
        }
    }
}
