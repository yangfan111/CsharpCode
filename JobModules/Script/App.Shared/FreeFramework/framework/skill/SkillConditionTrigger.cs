using com.wd.free.para;
using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Free;

namespace com.wd.free.skill
{
    [Serializable]
    public class SkillConditionTrigger : AbstractSkillTrigger, IRule
    {
        private IParaCondition condition;

        public int GetRuleID()
        {
            return (int)ERuleIds.SkillConditionTrigger;
        }

        public override TriggerStatus Triggered(ISkillArgs args)
        {
            if (condition.Meet(args))
            {
                return IsInter(args);
            }
            else
            {
                return TriggerStatus.Failed;
            }
        }
    }
}
