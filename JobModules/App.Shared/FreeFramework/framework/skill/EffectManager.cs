using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.skill
{
	public class EffectManager
	{
		private MyDictionary<string, EffectSet> map;

		public EffectManager()
		{
			this.map = new MyDictionary<string, EffectSet>();
		}

		public virtual void AddEffect(ISkillEffect effect)
		{
			if (!map.ContainsKey(effect.GetKey()))
			{
				map[effect.GetKey()] = new EffectSet();
			}
			map[effect.GetKey()].AddSkillEffect(effect);
		}

		public virtual void RemoveEffect(long source, string key)
		{
			if (map.ContainsKey(key))
			{
				map[key].RemoveEffect(source);
				if (map[key].Count() == 0)
				{
					map.Remove(key);
				}
			}
		}

		public virtual void RemoveEffect(ISkillEffect effect)
		{
			long source = effect.GetSource();
			string key = effect.GetKey();
			RemoveEffect(source, key);
		}

		public virtual void Frame(ISkillArgs args)
		{
			foreach (EffectSet ef in map.Values)
			{
				ef.RemoveTimeOut();
				ef.Effect(args);
			}
		}
	}
}
