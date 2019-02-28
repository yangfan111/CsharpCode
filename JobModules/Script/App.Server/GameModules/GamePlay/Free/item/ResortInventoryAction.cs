using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.unit;
using com.wd.free.util;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class ResortInventoryAction : AbstractPlayerAction
	{
		private const long serialVersionUID = -1253447267320235916L;

		private string inventory;

		private string order;

		public override void DoAction(IEventArgs args)
		{
			FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
			IGameUnit player = GetPlayer(args);
			if (player != null)
			{
				if (StringUtil.IsNullOrEmpty(inventory))
				{
					inventory = "default";
				}
				string inKey = FreeUtil.ReplaceVar(inventory, args);
				FreeData fd = (FreeData)player;
				ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(inKey);
				if (ii != null)
				{
					ii.Resort(fr, order);
				}
			}
		}
	}
}
