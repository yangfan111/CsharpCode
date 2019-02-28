using App.Server.GameModules.GamePlay;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.map.position;
using com.wd.free.util;
using gameplay.gamerule.free.rule;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class CreateItemToSceneAction : AbstractGameAction
	{
		private const long serialVersionUID = 5687056626382340079L;

		private const string INI_COUNT = "1";

        public string key;

        public string count;

		private string time;

		private IGameAction action;

		public IPosSelector pos;

		public override void DoAction(IEventArgs args)
		{
			FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
			if (StringUtil.IsNullOrEmpty(count) || count.Equals("0"))
			{
				count = INI_COUNT;
			}
			int c = FreeUtil.ReplaceInt(count, args);
			FreeItem item = FreeItemManager.GetItem(fr, FreeUtil.ReplaceVar(key, args), c);
			if (item != null)
			{
				if (!StringUtil.IsNullOrEmpty(time))
				{
					((FreeGameItem)item).SetTime(time);
				}
				item.Drop(fr, pos.Select(args));
				if (action != null)
				{
					args.TempUse(ParaConstant.PARA_ITEM, item);
					action.Act(args);
					args.Resume(ParaConstant.PARA_ITEM);
				}
			}
		}
	}
}
