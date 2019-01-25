using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace com.wd.free.condition
{
    [Serializable]
    public class NotParaCondition : IParaCondition
    {
        private IParaCondition condition;

        public bool Meet(IEventArgs args)
        {
            return !condition.Meet(args);
        }
    }
}
