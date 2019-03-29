using Sharpen;
using com.wd.free.action;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class ActionSkill : AbstractCoolDownSkill
	{
		private const long serialVersionUID = -2098741273322300043L;

		protected IGameAction initial;

		protected IGameAction effect;

		protected IGameAction resume;

		public override void Effet(ISkillArgs args)
		{
			if (effect != null)
			{
				effect.Act(args);
			}
		}

		public override void Resume(ISkillArgs args)
		{
			if (resume != null)
			{
				resume.Act(args);
			}
		}

		public override void Initial(ISkillArgs args)
		{
			if (initial != null)
			{
				initial.Act(args);
			}
		}

		public virtual IGameAction GetEffect()
		{
			return effect;
		}

		public virtual void SetEffect(IGameAction effect)
		{
			this.effect = effect;
		}

		public virtual IGameAction GetResume()
		{
			return resume;
		}

		public virtual void SetResume(IGameAction resume)
		{
			this.resume = resume;
		}
	}
}
