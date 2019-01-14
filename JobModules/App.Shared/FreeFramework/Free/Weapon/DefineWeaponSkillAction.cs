using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.skill;

namespace App.Shared.FreeFramework.Free.Weapon
{
    [Serializable]
    public class DefineWeaponSkillAction : AbstractGameAction
    {
        private int id;
        private List<ISkill> skills;

        public override void DoAction(IEventArgs args)
        {
            if (skills != null)
            {
                foreach (ISkill skill in skills)
                {
                    WeaponSkillFactory.RegisterSkill(id, skill);
                }
            }
        }
    }
}
