using Sharpen;
using com.wd.free.action;

namespace com.wd.free.skill
{
	[System.Serializable]
	public abstract class AbstractSkillTrigger : ISkillTrigger
	{
		private const long serialVersionUID = -2610466270149079241L;

		private ISkillInterrupter interrupter;

		private IGameAction interAction;

		protected internal virtual ISkillTrigger.TriggerStatus IsInter(ISkillArgs args)
		{
			if (interrupter != null && interrupter.IsInterrupted(args))
			{
				if (interAction != null)
				{
					interAction.Act(args);
				}
				return ISkillTrigger.TriggerStatus.Failed;
			}
			else
			{
				return ISkillTrigger.TriggerStatus.Success;
			}
		}
	}
}
