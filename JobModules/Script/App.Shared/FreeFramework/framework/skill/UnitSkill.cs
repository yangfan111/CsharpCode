using System;
using System.Collections.Generic;
using com.wd.free.action;
using Sharpen;
using com.wd.free.unit;

namespace com.wd.free.skill
{
	public class UnitSkill
	{
		private bool disable;

		private IList<ISkill> skills;

		private IGameUnit unit;

		private EffectManager effects;

		private IList<ISkill> removes;

		private IList<ISkill> adds;

		public UnitSkill(IGameUnit unit)
		{
			this.skills = new List<ISkill>();
			this.effects = new EffectManager();
			this.unit = unit;
			this.removes = new List<ISkill>();
			this.adds = new List<ISkill>();
		}

		public virtual bool IsDisable()
		{
			return disable;
		}

        public bool IsEmtpy()
        {
            return skills.Count == 0;
        }

		public virtual void SetDisable(bool disable)
		{
			this.disable = disable;
		}

		public virtual void AddSkill(ISkill skill)
		{
			skill.SetUnit(unit);
			this.adds.Add(skill);
		}

		public virtual void RemoveSkill(ISkill skill)
		{
			this.removes.Add(skill);
		}

	    public void Clear()
	    {
	        this.skills.Clear();
	    }

		public virtual void RemoveSkill(string key)
		{
			Iterator<ISkill> it = this.skills.Iterator();
			while (it.HasNext())
			{
				ISkill skill = it.Next();
				if (key.Equals(skill.GetKey()))
				{
					this.removes.Add(skill);
				}
			}
		}

		public virtual void AddSkillEffect(ISkillEffect effect)
		{
			this.effects.AddEffect(effect);
		}

		public virtual void RemoveSkillEffect(ISkillEffect effect)
		{
			this.effects.RemoveEffect(effect);
		}

		public virtual void Frame(ISkillArgs args)
		{
            for(int i = 0; i < skills.Count; i++)
            {
                ISkill skill = skills[i];
                skill.SetDisable(disable);
                //FreeLog.ActionMark = unit.GetID() + "'s skill " + skill.ToString();
                FreeLog.SetTrigger(skill);
                skill.Frame(args);
            }

			if (removes.Count > 0)
			{
                for(int i = 0; i < removes.Count; i++)
                {
                    ISkill skill_1 = removes[i];
                    skills.Remove(skill_1);
                }
				removes.Clear();
			}
			if (adds.Count > 0)
			{
                for (int i = 0; i < adds.Count; i++)
                {
                    ISkill skill_1 = adds[i];
                    skills.Add(skill_1);
                }
				adds.Clear();
			}
			effects.Frame(args);
		}
	}
}
