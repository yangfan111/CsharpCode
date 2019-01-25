using Sharpen;
using com.wd.free.action;

namespace com.wd.free.skill
{
	[System.Serializable]
	public abstract class AbstractCoolDownSkill : AbstractSkill
	{
		private const long serialVersionUID = -7424856868352449412L;

		[System.NonSerialized]
		protected internal SkillTimer timer;

		protected internal ISkillTrigger trigger;

		protected internal bool always;

		protected internal bool interCoolDown;

		protected internal int last;

		protected internal int cooldown;

		protected internal IGameAction notReadyAction;

		protected internal IGameAction alreadyAction;

		protected internal IGameAction cooldownAction;

		protected internal IGameAction lastAction;

		[System.NonSerialized]
		private bool notCooldown;

		protected internal ISkillInterrupter inter;

		protected internal IGameAction interAction;

		public AbstractCoolDownSkill()
		{
		}

		// 技能持续中已冷却,是否可以再次使用技能
		// 技能被打断的情况下是否会进入冷却
		public override void Frame(ISkillArgs args)
		{
			InitialTimer();
			Initial(args);
			if (timer.IsActive() && inter != null && inter.IsInterrupted(args))
			{
				if (interAction != null)
				{
					interAction.Act(args);
				}
				timer.Terminate();
			}
			if (disable)
			{
				Warning(args, disableAction);
			}
			else
			{
				if (timer.IsCooldownOver())
				{
					if (notCooldown)
					{
						notCooldown = false;
						if (cooldownAction != null)
						{
							cooldownAction.Act(args);
						}
					}
					if (always || !timer.IsActive())
					{
						ISkillTrigger.TriggerStatus s = ISkillTrigger.TriggerStatus.Success;
						if (trigger != null)
						{
							s = trigger.Triggered(args);
						}
						if (disable)
						{
							s = ISkillTrigger.TriggerStatus.Failed;
						}
						if (s == ISkillTrigger.TriggerStatus.Success || s == ISkillTrigger.TriggerStatus.Interrupted)
						{
							if (s == ISkillTrigger.TriggerStatus.Success)
							{
								timer.Start();
								Effet(args);
							}
							else
							{
								if (interCoolDown)
								{
									timer.Start();
								}
								timer.SetInterrupt(true);
							}
						}
					}
					else
					{
						Warning(args, alreadyAction);
					}
				}
				else
				{
					notCooldown = true;
					Warning(args, notReadyAction);
				}
			}
			if (timer.IsActive())
			{
				if (timer.IsLastOver())
				{
					timer.Stop();
					if (!timer.IsInterrupt() && !timer.IsActiveInter())
					{
						Resume(args);
					}
					timer.SetInterrupt(false);
				}
				else
				{
					if (lastAction != null)
					{
						lastAction.Act(args);
					}
				}
			}
		}

		private void Warning(ISkillArgs args, IGameAction action)
		{
			if (trigger != null && trigger.Triggered(args) == ISkillTrigger.TriggerStatus.Success)
			{
				if (action != null)
				{
					action.Act(args);
				}
			}
		}

		protected internal virtual void InitialTimer()
		{
			if (timer == null)
			{
				timer = new SkillTimer(cooldown, last);
			}
		}

		public abstract void Initial(ISkillArgs args);

		public abstract void Effet(ISkillArgs args);

		public abstract void Resume(ISkillArgs args);

		public virtual SkillTimer GetTimer()
		{
			return timer;
		}

		public virtual void SetTimer(SkillTimer timer)
		{
			this.timer = timer;
		}

		public virtual ISkillTrigger GetTrigger()
		{
			return trigger;
		}

		public virtual void SetTrigger(ISkillTrigger trigger)
		{
			this.trigger = trigger;
		}

		public virtual bool IsInterCoolDown()
		{
			return interCoolDown;
		}

		public virtual void SetInterCoolDown(bool interCoolDown)
		{
			this.interCoolDown = interCoolDown;
		}

		public virtual IGameAction GetNotReadyAction()
		{
			return notReadyAction;
		}

		public virtual void SetNotReadyAction(IGameAction notReadyAction)
		{
			this.notReadyAction = notReadyAction;
		}

		public virtual IGameAction GetAlreadyAction()
		{
			return alreadyAction;
		}

		public virtual void SetAlreadyAction(IGameAction alreadyAction)
		{
			this.alreadyAction = alreadyAction;
		}

		public virtual IGameAction GetCooldownAction()
		{
			return cooldownAction;
		}

		public virtual void SetCooldownAction(IGameAction cooldownAction)
		{
			this.cooldownAction = cooldownAction;
		}
	}
}
