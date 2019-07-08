using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;
using Core.Free;

namespace App.Shared.FreeFramework.framework.para.exp
{
    [Serializable]
    public class ActionCondition : IParaCondition, IRule
    {
        private const string Success = "success";

        private IGameAction action;

        private BoolPara condition = new BoolPara(Success, false);

        public int GetRuleID()
        {
            return (int)ERuleIds.ActionCondition;
        }

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
