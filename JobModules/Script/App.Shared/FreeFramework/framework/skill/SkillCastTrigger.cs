using Sharpen;
using com.wd.free.action;
using com.wd.free.para.exp;
using com.wd.free.util;
using UnityEngine;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillCastTrigger : ISkillTrigger
	{
		private const long serialVersionUID = 6618913049715717146L;

		private string time;

		private int key;

		private ISkillInterrupter interrupter;

		private IGameAction castAction;

		private IGameAction interAction;

		private IParaCondition condition;

		[System.NonSerialized]
		private bool release;

		[System.NonSerialized]
		private long startTime;

		[System.NonSerialized]
		private int realTime;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			if (realTime == 0)
			{
				realTime = FreeUtil.ReplaceInt(time, args);
			}
			if (args.GetInput().IsPressed(key) && startTime == 0 && condition != null && !condition.Meet(args))
			{
				return ISkillTrigger.TriggerStatus.Failed;
			}
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
			if (args.GetInput().IsPressed(key))
			{
				if (startTime == 0 && release)
				{
					startTime = Runtime.CurrentTimeMillis();
					if (castAction != null)
					{
						castAction.Act(args);
					}
				}
				release = false;
			}
			else
			{
				release = true;
			}
            
			if (startTime > 0 && Runtime.CurrentTimeMillis() - startTime >= realTime)
			{
				startTime = 0;
				return ISkillTrigger.TriggerStatus.Success;
			}
			return ISkillTrigger.TriggerStatus.Failed;
		}

		public virtual int GetKey()
		{
			return key;
		}

		public virtual void SetKey(int key)
		{
			this.key = key;
		}

		public virtual string GetTime()
		{
			return time;
		}

		public virtual void SetTime(string time)
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
	}
}
