using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.skill
{
	public class EffectSet
	{
		private string key;

		private MyDictionary<long, ISkillEffect> effects;

		private MyDictionary<string, long> timeMap;

		private ISkillEffect current;

		public EffectSet()
		{
			this.effects = new MyDictionary<long, ISkillEffect>();
			this.timeMap = new MyDictionary<string, long>();
		}

		public virtual string GetKey()
		{
			return key;
		}

		private string GetEffectKey(ISkillEffect effct)
		{
			return effct.GetKey() + "_" + effct.GetSource() + "_" + effct.GetLevel();
		}

		public virtual void AddSkillEffect(ISkillEffect effct)
		{
			ISkillEffect old = this.effects[effct.GetSource()];
			if (old == null || old.GetLevel() <= effct.GetLevel())
			{
				this.effects[effct.GetSource()] = effct;
				this.key = effct.GetKey();
				this.timeMap[GetEffectKey(effct)] = Runtime.CurrentTimeMillis();
			}
		}

		public virtual void RemoveTimeOut()
		{
			IList<long> re = new List<long>();
			foreach (ISkillEffect ef in effects.Values)
			{
				if (ef.IsTimeOut(Runtime.CurrentTimeMillis() - timeMap[GetEffectKey(ef)]))
				{
					re.Add(ef.GetSource());
				}
			}
			foreach (long l in re)
			{
				effects.Remove(l);
			}
		}

		public virtual void RemoveEffect(long source)
		{
			this.effects.Remove(source);
		}

		public virtual int Count()
		{
			return this.effects.Count;
		}

		public virtual ISkillEffect GetEffect(long source)
		{
			return effects[source];
		}

		private ISkillEffect SelectHighest()
		{
			int max = 0;
			ISkillEffect maxE = null;
			foreach (ISkillEffect se in effects.Values)
			{
				if (se.GetLevel() > max)
				{
					max = se.GetLevel();
					maxE = se;
				}
			}
			return maxE;
		}

		public virtual void Effect(ISkillArgs args)
		{
			ISkillEffect newE = SelectHighest();
			if (current == null)
			{
				if (newE != null)
				{
					current = newE;
					current.Effect(args);
				}
			}
			else
			{
				if (newE == null)
				{
					current.Resume(args);
					current = newE;
				}
				else
				{
					if (newE.GetLevel() != current.GetLevel())
					{
						current.Resume(args);
						current = newE;
						current.Effect(args);
					}
				}
			}
		}
	}
}
