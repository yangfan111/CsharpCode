using Sharpen;
using com.wd.free.skill;
using com.wd.free.action;

namespace com.wd.free.item
{
	public interface IInventoryUI
	{
		void DeleteItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip);

		void AddItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip);

		void UpdateItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip);

		void UseItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip);

		void ReDraw(ISkillArgs args, ItemInventory inventory, bool includeBack);

		void Error(ISkillArgs args, ItemInventory inventory, string msg);

        IGameAction MoveAction { get; }

        IGameAction CanNotMoveAction { get; }

        IGameAction MoveOutAction { get; }

        IGameAction ErrorAction { get; }
    }
}
