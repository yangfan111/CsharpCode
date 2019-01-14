using Sharpen;
using com.wd.free.action;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SimpleSkillEffect : AbstractSkillEffect
	{
		private const long serialVersionUID = -5148825136040458649L;

		protected internal IGameAction effect;

		protected internal IGameAction resume;

		public SimpleSkillEffect()
			: base()
		{
			this.key = string.Empty;
			this.source = 0;
			this.time = 0;
		}

		public override void Effect(ISkillArgs args)
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
	}
}
