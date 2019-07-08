using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.unit;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class PlayerUseItemAction : AbstractPlayerAction, IRule
    {
		private const long serialVersionUID = 6361325829890462542L;

		private string item;

		public override void DoAction(IEventArgs args)
		{
			FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
			IGameUnit player = GetPlayer(args);
			if (StringUtil.IsNullOrEmpty(item))
			{
				item = "item";
			}
			if (player != null)
			{
				FreeData fd = (FreeData)player;
				FreeItem fi = (FreeItem)fr.GetUnit(FreeUtil.ReplaceVar(item, args));
				FreeItemManager.UseItem(fd.GetFreeInventory().GetItemPosition(fi), fd, fr);
			}
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerUseItemAction;
        }
    }
}
