using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class StringMapAction : AbstractGameAction
	{
		private const long serialVersionUID = -5592323358066894668L;

		private string keys;

		[System.NonSerialized]
		private LinkedHashMap<string, string> cache;

		private string key;

		private bool useKey;

		private IGameAction action;

		public override void DoAction(IEventArgs args)
		{
			if (cache == null || keys.Contains("{") || keys.Contains("}"))
			{
				cache = new LinkedHashMap<string, string>();
				string[] ks = StringUtil.Split(FreeUtil.ReplaceVar(keys, args), new string[] { ",", "，" });
				foreach (string k in ks)
				{
					string[] vs = StringUtil.Split(k, "=");
					if (vs.Length == 2)
					{
						cache[vs[0].Trim()] = vs[1].Trim();
					}
					else
					{
						if (vs.Length == 1)
						{
							cache[vs[0].Trim()] = vs[0].Trim();
						}
					}
				}
			}
			if (StringUtil.IsNullOrEmpty(key))
			{
				int i = 0;
				foreach (string k in cache.Keys)
				{
					HandleOne(cache.Count, i + 1, k, cache[k], args);
					i++;
				}
			}
			else
			{
				string k = FreeUtil.ReplaceVar(key, args);
				if (cache.ContainsKey(k) || useKey)
				{
					string v = cache[k];
					args.GetDefault().GetParameters().TempUse(new BoolPara("hasKey", v != null));
					if (v == null)
					{
						v = key;
					}
					args.GetDefault().GetParameters().TempUse(new StringPara("key", k));
					args.GetDefault().GetParameters().TempUse(new StringPara("value", v));
					if (action != null)
					{
						action.Act(args);
					}
					args.GetDefault().GetParameters().Resume("key");
					args.GetDefault().GetParameters().Resume("value");
					args.GetDefault().GetParameters().Resume("hasKey");
				}
			}
		}

		private void HandleOne(int all, int index, string k, string v, IEventArgs args)
		{
			args.GetDefault().GetParameters().TempUse(new IntPara("index", index));
			args.GetDefault().GetParameters().TempUse(new IntPara("count", all));
			args.GetDefault().GetParameters().TempUse(new StringPara("key", k));
			args.GetDefault().GetParameters().TempUse(new StringPara("value", v));
			if (action != null)
			{
				action.Act(args);
			}
			args.GetDefault().GetParameters().Resume("count");
			args.GetDefault().GetParameters().Resume("index");
			args.GetDefault().GetParameters().Resume("key");
			args.GetDefault().GetParameters().Resume("value");
		}
	}
}
