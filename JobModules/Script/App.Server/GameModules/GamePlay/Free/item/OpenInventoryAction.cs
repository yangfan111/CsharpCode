using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.unit;
using gameplay.gamerule.free.ui;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class OpenInventoryAction : AbstractPlayerAction
	{
		private const long serialVersionUID = 7873774135852129840L;

		private string name;

		private bool alwaysClose;

		private bool alwaysOpen;

		public override void DoAction(IEventArgs args)
		{
			IGameUnit player = GetPlayer(args);
			if (player != null)
			{
				if (StringUtil.IsNullOrEmpty(name))
				{
					name = InventoryManager.DEFAULT;
				}
				ItemInventory ii = ((FreeData)player).freeInventory.GetInventoryManager().GetInventory(name);
				FreeUIShowAction fui = new FreeUIShowAction();
				fui.SetScope(SendMessageAction.SCOPE_PLYAER);
				fui.SetPlayer(this.player);
				if (alwaysOpen)
				{
					fui.SetTime(FreeUIShowAction.ALWAYS);
					ii.SetOpen(true);
				}
				else
				{
					if (alwaysClose)
					{
						fui.SetTime(FreeUIShowAction.HIDE);
						ii.SetOpen(false);
					}
					else
					{
						if (ii.IsOpen())
						{
							fui.SetTime(FreeUIShowAction.HIDE);
							ii.SetOpen(false);
						}
						else
						{
							fui.SetTime(FreeUIShowAction.ALWAYS);
							ii.SetOpen(true);
						}
					}
				}
				if (ii.IsOpen())
				{
					// ii.reDraw(fr);
					if (ii.GetOpenAction() != null)
					{
						ii.GetOpenAction().Act(args);
					}
				}
				else
				{
					if (ii.GetCloseAction() != null)
					{
						ii.GetCloseAction().Act(args);
					}
				}
				fui.SetKey(ii.GetUIKey());
				fui.Act(args);
				foreach (ItemPosition ip in ii.GetItems())
				{
					fui.SetKey(ip.GetUIKey());
					fui.Act(args);
				}
			}
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual bool IsAlwaysClose()
		{
			return alwaysClose;
		}

		public virtual void SetAlwaysClose(bool alwaysClose)
		{
			this.alwaysClose = alwaysClose;
		}

		public virtual bool IsAlwaysOpen()
		{
			return alwaysOpen;
		}

		public virtual void SetAlwaysOpen(bool alwaysOpen)
		{
			this.alwaysOpen = alwaysOpen;
		}
	}
}
