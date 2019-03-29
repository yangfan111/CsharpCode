using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    [Serializable]
    public class ChickenRuleCondition : IParaCondition
    {
        private static IParaCondition[] conditions;

        private int code;

        static ChickenRuleCondition()
        {
            conditions = new IParaCondition[100];

            conditions[1] = new CanDropCondition();
        }

        public bool Meet(IEventArgs args)
        {
            if(code < conditions.Length)
            {
                IParaCondition con = conditions[code];
                if(con != null)
                {
                    return con.Meet(args);
                }
            }

            return false;
        }
    }
}
