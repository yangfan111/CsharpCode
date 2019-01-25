using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.action
{
	[System.Serializable]
	public class ListClearAction : AbstractGameAction
	{
		private const long serialVersionUID = -3082370039786904191L;

		private string key;

		private string sorter;

		private string exp;

		public override void DoAction(IEventArgs args)
		{
			if (StringUtil.IsNullOrEmpty(key))
			{
				key = BaseEventArgs.DEFAULT;
			}
			IParable unit = args.GetUnit(key);
			if (unit != null)
			{
				IPara sort = unit.GetParameters().Get(sorter);
				if (sort != null)
				{
					ParaListSet pls = (ParaListSet)sort;
					IList<ParaList> removes = new List<ParaList>();
					foreach (ParaList pl in pls)
					{
						if (StringUtil.IsNullOrEmpty(exp) || pl.Meet(exp))
						{
							removes.Add(pl);
						}
					}
					foreach (ParaList pl_1 in removes)
					{
						pls.RemoveParaList(pl_1);
					}
				}
			}
		}
	}
}
