using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.exception;
using com.wd.free.unit;

namespace com.wd.free.config
{
	public class UnitSet
	{
		private MyDictionary<string, IGameUnit> units;

		public UnitSet()
		{
			this.units = new MyDictionary<string, IGameUnit>();
		}

		public virtual IGameUnit GetUnit(string key)
		{
			return units[key];
		}

		public virtual void SetUnits(ConfigGameUnit[] cns)
		{
			units = new MyDictionary<string, IGameUnit>();
			while (cns.Length > units.Count)
			{
				int size = units.Count;
				IList<string> msg = new List<string>();
				foreach (ConfigGameUnit u in cns)
				{
					IGameUnit unit = u.ToGameUnit();
					if (IsNullOrEmpty(u.GetParent()) && IsNullOrEmpty(u.GetTriggerParent()) && IsNullOrEmpty(u.GetParaParent()))
					{
						units[u.GetKey()] = unit;
					}
					else
					{
						bool all = true;
					    MyDictionary<string, int> pMap = new MyDictionary<string, int>();
						if (!IsNullOrEmpty(u.GetParent()))
						{
							string[] ps = u.GetParent().Split(",");
							foreach (string p in ps)
							{
								if (!units.ContainsKey(p.Trim()))
								{
									all = false;
									msg.Add(unit.GetKey() + " is depend on " + p);
									break;
								}
								pMap[p] = 0;
							}
						}
						if (all && !IsNullOrEmpty(u.GetParaParent()))
						{
							string[] ps = u.GetParaParent().Split(",");
							foreach (string p in ps)
							{
								if (!units.ContainsKey(p.Trim()))
								{
									all = false;
									msg.Add(unit.GetKey() + " is depend on " + p);
									break;
								}
								pMap[p] = 1;
							}
						}
						if (all && !IsNullOrEmpty(u.GetTriggerParent()))
						{
							string[] ps = u.GetTriggerParent().Split(",");
							foreach (string p in ps)
							{
								if (!units.ContainsKey(p.Trim()))
								{
									all = false;
									msg.Add(unit.GetKey() + " is depend on " + p);
									break;
								}
								pMap[p] = 2;
							}
						}
						if (all)
						{
							foreach (string p in pMap.Keys)
							{
								MergeParent(unit, p, pMap[p]);
							}
							units[u.GetKey()] = unit;
						}
					}
				}
				if (size == units.Count && cns.Length > units.Count)
				{
					throw new GameConfigExpception("units have dependency issues\n" + StringUtil.GetStringFromStrings(msg, "\n"));
				}
			}
		}

		private void MergeParent(IGameUnit unit, string p, int type)
		{
			if (type == 0 || type == 1)
			{
				unit.GetParameters().Merge(units[p].GetParameters());
			}
		}

		private bool IsNullOrEmpty(string s)
		{
			return s == null || s.Trim().Length == 0;
		}
	}
}
