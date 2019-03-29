using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public abstract class ISkillTrigger
	{
		public enum TriggerStatus
		{
			Success,
			Failed,
			Interrupted
		}

		public abstract ISkillTrigger.TriggerStatus Triggered(ISkillArgs args);
	}

	public static class ISkillTriggerConstants
	{
	}
}
