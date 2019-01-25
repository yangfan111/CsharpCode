using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace com.wd.free.condition
{
    [Serializable]
    public class OrParaCondition : IParaCondition
    {
        public List<IParaCondition> conditions;

        public bool Meet(IEventArgs args)
        {
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i].Meet(args))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
