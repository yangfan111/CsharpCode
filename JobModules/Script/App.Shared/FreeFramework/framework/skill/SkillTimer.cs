using Sharpen;

namespace com.wd.free.skill
{
	public class SkillTimer
	{
		private int coolDownDuration;

		private int lastDuration;

		private long currentTime;

		private long pause;

		private bool active;

		private bool interrupt;

		private bool activeInter;

		public SkillTimer()
		{
			this.coolDownDuration = 1000;
			this.lastDuration = 1000;
			this.currentTime = 0;
		}

		public SkillTimer(int cooldown, int last)
		{
			this.coolDownDuration = cooldown;
			this.lastDuration = last;
			this.currentTime = 0;
		}

		public virtual void SetCoolDownDuration(int coolDownDuration)
		{
			this.coolDownDuration = coolDownDuration;
		}

		public virtual void SetLastDuration(int lastDuration)
		{
			this.lastDuration = lastDuration;
		}

		public virtual int GetCoolDownDuration()
		{
			return coolDownDuration;
		}

		public virtual int GetLastDuration()
		{
			return lastDuration;
		}

		public virtual void Pause()
		{
			this.pause = Runtime.CurrentTimeMillis();
		}

		public virtual void Resume()
		{
			this.currentTime += Runtime.CurrentTimeMillis() - pause;
			this.pause = 0;
		}

		public virtual void Terminate()
		{
			this.activeInter = true;
		}

		public virtual bool IsLastOver()
		{
			return (pause == 0 && (Runtime.CurrentTimeMillis() - currentTime) >= lastDuration) || activeInter;
		}

		public virtual bool IsCooldownOver()
		{
			return pause == 0 && (Runtime.CurrentTimeMillis() - currentTime) >= coolDownDuration;
		}

		public virtual bool IsActive()
		{
			return active;
		}

		public virtual void Stop()
		{
			this.active = false;
		}

		public virtual void Start()
		{
			this.currentTime = Runtime.CurrentTimeMillis();
			this.active = true;
			this.activeInter = false;
			this.interrupt = false;
		}

		public virtual void SetInterrupt(bool interrupt)
		{
			this.interrupt = interrupt;
		}

		public virtual bool IsInterrupt()
		{
			return interrupt;
		}

		public virtual bool IsActiveInter()
		{
			return activeInter;
		}
	}
}
