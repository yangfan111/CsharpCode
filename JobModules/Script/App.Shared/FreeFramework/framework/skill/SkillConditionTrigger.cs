using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.wd.free.skill
{
    [Serializable]
    public class SkillConditionTrigger : AbstractSkillTrigger
    {
        private IParaCondition condition;

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
