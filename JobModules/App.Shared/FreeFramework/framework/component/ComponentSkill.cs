using Sharpen;
using com.wd.free.skill;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentSkill : AbstractSkill, IComponentable
	{
		private const long serialVersionUID = 5028515268481970234L;

		private string name;

		private ISkill defaultSkill;

		private string title;

		private string desc;

		public override void Frame(ISkillArgs args)
		{
			ISkill skill = args.ComponentMap.GetSkill(name);
			if (skill != null)
			{
				skill.Frame(args);
			}
			else
			{
				if (defaultSkill != null)
				{
					defaultSkill.Frame(args);
				}
			}
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual ISkill GetDefaultSkill()
		{
			return defaultSkill;
		}

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public override string GetDesc()
		{
			return desc;
		}

		public override void SetDesc(string desc)
		{
			this.desc = desc;
		}
	}
}
