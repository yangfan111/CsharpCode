using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.skill;
using com.wd.free.unit;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class CreateInventoryAction : AbstractPlayerAction
	{
		private const long serialVersionUID = 3638982865899274709L;

		private string name;

		private int column;

		private int row;

		private string canDrop;

		private IInventoryUI inventoryUI;

		private IGameAction closeAction;

		private IGameAction openAction;

		private IGameAction dropAction;

		public override void DoAction(IEventArgs args)
		{
			IGameUnit player = GetPlayer(args);
			FreeData fd = (FreeData)player;
			ItemInventory ii = new ItemInventory(column, row);
			ii.SetCloseAction(closeAction);
			ii.SetOpenAction(openAction);
			ii.SetName(name);
			ii.SetInventoryUI(inventoryUI);
			ii.SetCanDrop(canDrop);
			ii.SetDropAction(dropAction);
			inventoryUI.ReDraw((ISkillArgs)args, ii, true);
			fd.freeInventory.GetInventoryManager().AddInventory(name, ii);
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual int GetColumn()
		{
			return column;
		}

		public virtual void SetColumn(int column)
		{
			this.column = column;
		}

		public virtual int GetRow()
		{
			return row;
		}

		public virtual void SetRow(int row)
		{
			this.row = row;
		}
	}
}
