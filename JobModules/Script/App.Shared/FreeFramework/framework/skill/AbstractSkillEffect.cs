using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public abstract class AbstractSkillEffect : ISkillEffect
	{
		private const long serialVersionUID = 849509849452925460L;

		protected internal long source;

		protected internal int time;

		protected internal string key;

		protected internal string name;

		protected internal int level;

		public virtual long GetSource()
		{
			return source;
		}

		public virtual void SetSource(long source)
		{
			this.source = source;
		}

		public virtual int GetTime()
		{
			return time;
		}

		public virtual void SetTime(int time)
		{
			this.time = time;
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual int GetLevel()
		{
			return level;
		}

		public virtual void SetLevel(int level)
		{
			this.level = level;
		}

		public virtual bool IsTimeOut(long time)
		{
			return time > this.time && time < 1000000;
		}

		public abstract void Effect(ISkillArgs arg1);

		public abstract void Resume(ISkillArgs arg1);
	}
}
