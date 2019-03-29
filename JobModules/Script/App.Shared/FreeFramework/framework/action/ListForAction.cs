using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class ListForAction : AbstractGameAction
	{
		private const long serialVersionUID = -3082370039786904191L;

		private string key;

		private string sorter;

		private IParaCondition condition;

		private string remove;

		private IGameAction noneAction;

		private IGameAction action;

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
						Iterator<ParaList> it = pls.Iterator();
						int i = 1;
						while (it.HasNext())
						{
							ParaList pl = it.Next();
							args.TempUse("element", new SimpleParable(pl));
							args.GetDefault().GetParameters().TempUse(new IntPara("index", i++));
							if (condition == null || condition.Meet(args))
							{
								if (action != null)
								{
									action.Act(args);
								}
								if (FreeUtil.ReplaceBool(remove, args))
								{
									it.Remove();
								}
							}
							args.GetDefault().GetParameters().Resume("index");
							args.Resume("element");
						}
						if (i == 1)
						{
							if (noneAction != null)
							{
								noneAction.Act(args);
							}
						}
					}
				}
			}
		}
	}
}
