using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class ListSortAction : AbstractGameAction
	{
		private const long serialVersionUID = -3082370039786904191L;

		private string key;

		private string sorter;

		private string exp;

		private IGameAction action;

		private bool keep;

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
					if (action != null)
					{
						ParaListSet temp = pls.Sort(FreeUtil.ReplaceVar(exp, args));
						temp.SetName(pls.GetName());
						unit.GetParameters().TempUse(temp);
						action.Act(args);
						unit.GetParameters().Resume(temp.GetName());
					}
					if (!keep)
					{
						pls.ReOrder();
					}
				}
			}
		}
	}
}
