using System;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.rule;

namespace gameplay.gamerule.free.item
{
	/// <summary>丢弃物品和移除物品的区别在于,丢弃物品丢弃的是上次使用的物品</summary>
	/// <author>Dave</author>
	[System.Serializable]
	public class PlayerDropItemAction : AbstractPlayerAction
	{
		private const long serialVersionUID = 6361325829890462542L;

		private string cat;

		private IGameAction message;

		private IGameAction drop;

		public override void DoAction(IEventArgs args)
		{
			FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
			IGameUnit player = GetPlayer(args);
			if (player != null)
			{
				FreeData fd = (FreeData)player;
				try
				{
					ItemPosition currentItem = fd.freeInventory.GetCurrentItem();
					if (!StringUtil.IsNullOrEmpty(cat))
					{
						currentItem = fd.freeInventory.GetCurrentItem(FreeUtil.ReplaceVar(cat, args));
					}
					if (currentItem != null)
					{
						currentItem.GetInventory().RemoveItem(fr, currentItem);
						if (drop != null)
						{
							fr.TempUse("item", currentItem.GetKey());
							drop.Act(args);
							fr.Resume("item");
						}
					}
				}
				catch (Exception e)
				{
					StringPara sp = new StringPara("message", e.Message);
					args.GetDefault().GetParameters().TempUse(sp);
					if (message != null)
					{
						message.Act(args);
					}
					args.GetDefault().GetParameters().Resume("message");
				}
			}
		}
	}
}
