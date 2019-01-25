using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;

namespace App.Shared.FreeFramework.framework.para.exp
{
    [Serializable]
    public class ActionCondition : IParaCondition
    {
        private const string Success = "success";

        private IGameAction action;

        private BoolPara condition = new BoolPara(Success, false);

        public bool Meet(IEventArgs args)
        {
            condition.SetValue(false);
            args.TempUsePara(condition);

            if (action != null)
            {
                action.Act(args);
            }

            args.ResumePara(Success);

            return (bool)condition.GetValue();
        }
    }
}
