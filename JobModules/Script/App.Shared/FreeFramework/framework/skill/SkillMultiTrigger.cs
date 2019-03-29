using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillMultiTrigger : ISkillTrigger
	{
		private const long serialVersionUID = -6693234022996398380L;

		private IList<ISkillTrigger> triggers;

		private bool or;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			if (!or)
			{
                for(int i = 0; i < triggers.Count; i++)
                {
                    ISkillTrigger t = triggers[i];
					ISkillTrigger.TriggerStatus ts = t.Triggered(args);
					if (ts == ISkillTrigger.TriggerStatus.Failed || ts == ISkillTrigger.TriggerStatus.Interrupted)
					{
						return ts;
					}
				}
				return ISkillTrigger.TriggerStatus.Success;
			}
			else
			{
                for (int i = 0; i < triggers.Count; i++)
                {
                    ISkillTrigger t = triggers[i];
                    ISkillTrigger.TriggerStatus ts = t.Triggered(args);
					if (ts == ISkillTrigger.TriggerStatus.Success)
					{
						return ts;
					}
				}
				return ISkillTrigger.TriggerStatus.Failed;
			}
		}
	}
}
