using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.para;
using com.wd.free.skill;

namespace com.wd.free.skill
{
    [Serializable]
    public class PlayerActionSkill : ActionSkill
    {
        private bool active;

        [NonSerialized]
        private BoolPara aPara;

        private const string Active = "active";

        public override void Effet(ISkillArgs args)
        {
            if (effect != null)
            {
                if (aPara == null)
                {
                    aPara = new BoolPara(Active);
                }

                aPara.SetValue(active);

                args.GetDefault().GetParameters().TempUse(aPara);

                effect.Act(args);

                args.GetDefault().GetParameters().Resume(Active);

                active = !active;
            }
        }

        public override void Resume(ISkillArgs args)
        {
            if (resume != null)
            {
                resume.Act(args);
            }
        }
    }
}
