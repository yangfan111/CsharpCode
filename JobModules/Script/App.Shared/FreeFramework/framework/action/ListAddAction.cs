using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.action
{
	[System.Serializable]
	public class ListAddAction : AbstractGameAction
	{
		private const long serialVersionUID = -1981334499779306804L;

		public const string TEMP_SORT = "temp_sort";

		private string key;

		private string sorter;

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
				if (sort != null && sort is ParaListSet)
				{
					ParaListSet pls = (ParaListSet)sort;
					IParable pa = Get();
					if (action != null)
					{
						args.TempUse(TEMP_SORT, pa);
						args.TempUse("element", pa);
						action.Act(args);
						args.Resume("element");
						args.Resume(TEMP_SORT);
					}
					pls.AddParaList(pa.GetParameters());
				}
			}
		}

		private IParable Get()
		{
			return new _IParable_53();
		}

		private sealed class _IParable_53 : IParable
		{
			public _IParable_53()
			{
				this.pl = new ParaList();
			}

			private ParaList pl;

			public ParaList GetParameters()
			{
				return this.pl;
			}
		}
	}
}
