using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.unit;
using com.wd.free.util;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class SetItemCountAction : AbstractPlayerAction
	{
		private const long serialVersionUID = -8785665361284226681L;

		private string exp;

		private string count;

		[System.NonSerialized]
		private SelectMethod method;

		public override void DoAction(IEventArgs args)
		{
			FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
			IGameUnit player = GetPlayer(args);
			if (StringUtil.IsNullOrEmpty(count) || count.Equals("0"))
			{
				count = "0";
			}
			method = new SelectMethod(FreeUtil.ReplaceVar(exp, args));
			if (player != null)
			{
				FreeData fd = (FreeData)player;
				ItemPosition[] currentItems = fd.freeInventory.Select(method);
				fr.TempUse("current", fd);
				int c = FreeUtil.ReplaceInt(count, args);
				for (int i = 0; i < currentItems.Length; i++)
				{
					ItemPosition ip = currentItems[i];
					ip.SetCount(c);
					ip.GetKey().SetCount(c);
					ip.GetInventory().GetInventoryUI().UpdateItem(fr, ip.GetInventory(), ip);
				}
				fr.Resume("current");
			}
		}
	}
}
