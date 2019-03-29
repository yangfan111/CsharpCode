using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public abstract class InstantSkill : AbstractSkill
	{
		private const long serialVersionUID = -3972083391818600734L;

		public override void Frame(ISkillArgs args)
		{
			if (IsTriggered(args))
			{
				Effet(args);
			}
		}

		public abstract bool IsTriggered(ISkillArgs args);

		public abstract void Effet(ISkillArgs args);
	}
}
