using App.Server.GameModules.GamePlay;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.map.position;
using com.wd.free.para;
using gameplay.gamerule.free.rule;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class DropItemToSceneAction : AbstractGameAction
	{
		private const long serialVersionUID = 2908167368363002320L;

		private string item;

		private string time;

		private IPosSelector pos;

		public override void DoAction(IEventArgs args)
		{
			FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
			IParable pe = args.GetUnit(item);
			if (pe != null && pe is FreeItem)
			{
				FreeItem fi = (FreeItem)pe;
				if (!StringUtil.IsNullOrEmpty(time))
				{
					((FreeGameItem)fi).SetTime(time);
				}
				fi.Drop(fr, pos.Select(args));
			}
		}
	}
}
