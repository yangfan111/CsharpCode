using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.skill;
using com.wd.free.unit;

namespace com.wd.free.action
{
    [Serializable]
    public class AddSkillAction : AbstractPlayerAction
    {
        private List<ISkill> skills;

        public AddSkillAction()
        {
            this.skills = new List<ISkill>();
        }

        public void AddSkill(ISkill skill)
        {
            this.skills.Add(skill);
        }

        public override void DoAction(IEventArgs args)
        {
            if (string.IsNullOrEmpty(this.player))
            {
                this.player = "current";
            }
            var unit = GetPlayer(args);
            if (skills != null && unit != null)
            {
                foreach (var skill in skills)
                {
                    unit.GetUnitSkill().AddSkill((ISkill)SerializeUtil.Clone(skill));
                }
            }
        }
    }
}
