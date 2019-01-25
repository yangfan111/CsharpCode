using Sharpen;
using com.wd.free.skill;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentTrigger : ISkillTrigger, IComponentable
	{
		private const long serialVersionUID = 5028515268481970234L;

		private string name;

		private ISkillTrigger defaultTrigger;

		private string title;

		private string desc;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			ISkillTrigger trigger = args.ComponentMap.GetSkillTrigger(name);
			if (trigger != null)
			{
				return trigger.Triggered(args);
			}
			else
			{
				if (defaultTrigger != null)
				{
					return defaultTrigger.Triggered(args);
				}
			}
			return ISkillTrigger.TriggerStatus.Failed;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual ISkillTrigger GetDefaultTrigger()
		{
			return defaultTrigger;
		}

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}
	}
}
