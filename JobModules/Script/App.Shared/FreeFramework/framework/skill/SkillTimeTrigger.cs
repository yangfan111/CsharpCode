using Sharpen;
using com.wd.free.action;
using UnityEngine;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillTimeTrigger : ISkillTrigger
	{
		private const long serialVersionUID = 6618913049715717146L;

		private int time;

		private ISkillTrigger startTrigger;

		private ISkillInterrupter interrupter;

		private IGameAction castAction;

		public IGameAction interAction;

		[System.NonSerialized]
		private long startTime;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			if (interrupter != null && interrupter.IsInterrupted(args))
			{
				if (startTime > 0)
				{
					startTime = 0;
					if (interAction != null)
					{
						interAction.Act(args);
					}
					return ISkillTrigger.TriggerStatus.Interrupted;
				}
				startTime = 0;
			}
			if (startTrigger == null || startTrigger.Triggered(args) == ISkillTrigger.TriggerStatus.Success)
			{
				if (startTime == 0)
				{
					startTime = Runtime.CurrentTimeMillis();
					if (castAction != null)
					{
						castAction.Act(args);
					}
				}
			}
			if (startTime > 0 && Runtime.CurrentTimeMillis() - startTime >= time)
			{
				startTime = 0;
				return ISkillTrigger.TriggerStatus.Success;
			}
			return ISkillTrigger.TriggerStatus.Failed;
		}

		public virtual int GetTime()
		{
			return time;
		}

		public virtual void SetTime(int time)
		{
			this.time = time;
		}

		public virtual ISkillInterrupter GetInterrupter()
		{
			return interrupter;
		}

		public virtual void SetInterrupter(ISkillInterrupter interrupter)
		{
			this.interrupter = interrupter;
		}


        public void Reset()
        {
            this.startTime = 0;
        }
	}
}
