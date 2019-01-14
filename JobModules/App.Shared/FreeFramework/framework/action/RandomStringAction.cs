using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class RandomStringAction : AbstractGameAction
	{
		private const long serialVersionUID = -5592323358066894668L;

		private string keys;

		private IGameAction action;

		private bool all;

		private string count;

		private bool repeat;

		private static MyDictionary<string, RandomStringAction.KeyProperty> cache = new MyDictionary<string, RandomStringAction.KeyProperty>();

		public override void DoAction(IEventArgs args)
		{
			string realKeys = FreeUtil.ReplaceVar(keys, args);
			if (!cache.ContainsKey(realKeys))
			{
				string[] ks = StringUtil.Split(realKeys, new string[] { ",", "，" });
				ks = GetSpecialString(ks);
				int[] priority = new int[ks.Length];
				for (int i = 0; i < ks.Length; i++)
				{
					string k = ks[i];
					string[] vs = StringUtil.Split(k, "=");
					if (vs.Length == 2 && StringUtil.IsNumberString(vs[1]))
					{
						ks[i] = vs[0].Trim();
						priority[i] = NumberUtil.GetInt(vs[1]);
					}
					else
					{
						priority[i] = 1;
					}
				}
				RandomStringAction.KeyProperty kp = new RandomStringAction.KeyProperty();
				kp.v = realKeys;
				kp.ks = ks;
				kp.priority = priority;
				cache[realKeys] = kp;
			}
			string[] ks_1 = cache[realKeys].ks;
			int[] proAcc = cache[realKeys].priority;
			if (all)
			{
				for (int i = 0; i < ks_1.Length; i++)
				{
					HandleOne(ks_1.Length, i + 1, ks_1[i], args);
				}
			}
			else
			{
				if (StringUtil.IsNullOrEmpty(count))
				{
					count = "1";
				}
				int c = FreeUtil.ReplaceInt(count, args);
				int[] ins = null;
				if (repeat)
				{
					ins = RandomUtil.RandomWithProRepeat(0, proAcc, c);
				}
				else
				{
					ins = RandomUtil.RandomWithPro(0, proAcc, c);
				}
				for (int i = 0; i < ins.Length; i++)
				{
					HandleOne(ins.Length, i + 1, ks_1[ins[i]], args);
				}
			}
		}

		public static string[] GetSpecialString(string[] ss)
		{
			IList<string> list = new List<string>();
			foreach (string s in ss)
			{
				bool added = false;
				if (s.Contains("-") && s.StartsWith("$"))
				{
					string[] ns = StringUtil.Split(Sharpen.Runtime.Substring(s, 1), "-");
					if (ns.Length == 2)
					{
						if (StringUtil.IsNumberString(ns[0]) && StringUtil.IsNumberString(ns[1]))
						{
							int start = NumberUtil.GetInt(ns[0]);
							int end = NumberUtil.GetInt(ns[1]);
							for (int i = start; i <= end; i++)
							{
								list.Add(i.ToString());
							}
							added = true;
						}
					}
				}
				if (!added && !StringUtil.IsNullOrEmpty(s))
				{
					list.Add(s);
				}
			}
			return Sharpen.Collections.ToArray(list, new string[0]);
		}

		private void HandleOne(int all, int index, string k, IEventArgs args)
		{
			args.GetDefault().GetParameters().TempUse(new IntPara("index", index));
			args.GetDefault().GetParameters().TempUse(new IntPara("count", all));
			args.GetDefault().GetParameters().TempUse(new StringPara("random", k.Trim()));
			if (action != null)
			{
				action.Act(args);
			}
			args.GetDefault().GetParameters().Resume("count");
			args.GetDefault().GetParameters().Resume("index");
			args.GetDefault().GetParameters().Resume("random");
		}

		public virtual string GetKeys()
		{
			return keys;
		}

		public virtual void SetKeys(string keys)
		{
			this.keys = keys;
		}

		public virtual bool IsAll()
		{
			return all;
		}

		public virtual void SetAll(bool all)
		{
			this.all = all;
		}

		public virtual string GetCount()
		{
			return count;
		}

		public virtual void SetCount(string count)
		{
			this.count = count;
		}

		public virtual bool IsRepeat()
		{
			return repeat;
		}

		public virtual void SetRepeat(bool repeat)
		{
			this.repeat = repeat;
		}

		public override string ToMessage(IEventArgs args)
		{
			return FreeUtil.ReplaceVar(keys, args);
		}

		internal class KeyProperty
		{
			internal string v;

			internal string[] ks;

			internal int[] priority;
		}
	}
}
