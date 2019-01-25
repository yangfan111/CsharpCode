using System.Collections.Generic;
using System.Linq;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeUIListValue : AbstractFreeUIValue
	{
		private const long serialVersionUID = 5852438718279157187L;

		private string sorts;

		private string order;

		private int capacity;

		private string fields;

		private string range;

		private string autoCondition;

		[System.NonSerialized]
		private SelectMethod select;

		public override int GetType()
		{
			return COMPLEX;
		}

		public override object GetValue(IEventArgs args)
		{
			if (!StringUtil.IsNullOrEmpty(autoCondition))
			{
				select = new SelectMethod(FreeUtil.ReplaceVar(autoCondition, args));
			}
			IList<ParaList> list = new List<ParaList>();
			ParaListSet pls = null;
			foreach (UnitPara sort in UnitPara.Parse(sorts))
			{
				IParable pa = args.GetUnit(sort.GetUnit());
				if (pa == null)
				{
					continue;
				}
				IPara para = pa.GetParameters().Get(sort.GetPara());
				if (para == null)
				{
					continue;
				}
				if (para != null && para is ParaListSet)
				{
					ParaListSet ps = (ParaListSet)para;
					pls = ps;
					foreach (ParaList pl in ps)
					{
						list.Add(pl);
					}
				}
			}
			IList<SimpleProto> results = new List<SimpleProto>();
			if (pls != null && list.Count > 0)
			{
				IList<ParaList> r = pls.Select(list, FreeUtil.ReplaceVar(order, args), capacity);
				int min = 0;
				int max = 0;
				if (!StringUtil.IsNullOrEmpty(range))
				{
					string[] vs = StringUtil.Split(FreeUtil.ReplaceVar(range, args), "-");
					if (vs.Length == 2)
					{
						min = int.Parse(vs[0]);
						max = int.Parse(vs[1]);
					}
				}
				foreach (ParaList pl in r)
				{
					if (min != max)
					{
						int paraOrder = (int)pl.GetFeatureValue(ParaListSet.PARA_ORDER);
						if (paraOrder < min || paraOrder > max)
						{
							continue;
						}
					}

				    SimpleProto b = FreePool.Allocate();
					b.Ks.Add(0);
					b.Key = 0 ;
					b.Ss.Add("0");
					string[] fs = StringUtil.Split(fields, ",");
					for (int i = 0; i < fs.Length; i++)
					{
						b.Ks.Add(i + 1);
						b.Ss.Add(pl.GetFeatureValue(fs[i]).ToString());
						if (select != null)
						{
							pl.TempUse(new IntPara("seq", i + 1));
							if (select.Meet(pl))
							{
								b.Ins.Add(2 * 100);
							}
							else
							{
								b.Ins.Add(0);
							}
							pl.Resume("seq");
						}
						else
						{
							b.Ins.Add(0);
						}
					}
					results.Add(b);
				}
			}

		    return results.ToArray();
		}
	}
}
