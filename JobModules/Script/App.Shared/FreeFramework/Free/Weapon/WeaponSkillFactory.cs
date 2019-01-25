using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.skill;
using com.wd.free.trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.Free.Weapon
{
    public class WeaponSkillFactory
    {
        private static Dictionary<int, List<ISkill>> skillDic = new Dictionary<int, List<ISkill>>();

        public static void RegisterSkill(IEventArgs args, GameTrigger trigger)
        {
            trigger.Trigger(args);
        }

        public static void RegisterSkill(int weaponId, ISkill skill)
        {
            if (!skillDic.ContainsKey(weaponId))
            {
                skillDic[weaponId] = new List<ISkill>();
            }

            skillDic[weaponId].Add(skill);
        }

        public static UnitSkill GetSkill(int weaponId)
        {
            UnitSkill unitSkill = new UnitSkill(new WeaponFreeData(weaponId));

            if (skillDic.ContainsKey(weaponId))
            {

                List<ISkill> skills = (List<ISkill>)SerializeUtil.Clone(skillDic[weaponId]);
                for (int i = 0; i < skills.Count; i++)
                {
                    unitSkill.AddSkill(skills[i]);
                }
            }

            return unitSkill;
        }
    }
}
