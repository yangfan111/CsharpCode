using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.wd.free.item;
using com.wd.free.skill;

namespace gameplay.gamerule.free.item
{
	public class FreeInventory
	{
		private InventoryManager inventoryManager;

		private ItemPosition currentItem;

		private IDictionary<string, ItemPosition> lastUseItem;

		public FreeInventory()
		{
			this.inventoryManager = new InventoryManager();
			this.lastUseItem = new Dictionary<string, ItemPosition>();
		}

		public virtual InventoryManager GetInventoryManager()
		{
			return inventoryManager;
		}

		public virtual ItemPosition GetCurrentItem()
		{
			return currentItem;
		}

		public virtual ItemPosition GetItemPosition(FreeItem item)
		{
			foreach (string invName in inventoryManager.GetInventoryNames())
			{
				ItemInventory ii = inventoryManager.GetInventory(invName);
				foreach (ItemPosition ip in ii.GetItems())
				{
					if (ip.GetKey().GetId() == item.GetId())
					{
						return ip;
					}
				}
			}
			return null;
		}

		public virtual ItemPosition GetCurrentItem(string cat)
		{
			return lastUseItem[cat];
		}

		public virtual ItemPosition GetItemById(int id)
		{
			foreach (string inv in inventoryManager.GetInventoryNames())
			{
				ItemInventory ii = inventoryManager.GetInventory(inv);
				foreach (ItemPosition ip in ii.GetItems())
				{
					if (ip.GetKey().GetId() == id)
					{
						return ip;
					}
				}
			}
			return null;
		}

		public virtual ItemPosition[] Select(SelectMethod sm)
		{
			IList<ItemPosition> items = new List<ItemPosition>();
			DataBlock bl = new DataBlock();
			foreach (string inv in inventoryManager.GetInventoryNames())
			{
				ItemInventory ii = inventoryManager.GetInventory(inv);
				foreach (ItemPosition ip in ii.GetItems())
				{
					bl.AddData(ip);
				}
			}
			if (sm != null)
			{
				foreach (IFeaturable fe in sm.Select(bl).GetAllDatas())
				{
					items.Add((ItemPosition)fe);
				}
			}
			else
			{
				foreach (IFeaturable fe in bl.GetDataList())
				{
					items.Add((ItemPosition)fe);
				}
			}
			return Sharpen.Collections.ToArray(items, new ItemPosition[0]);
		}

		public virtual void SetCurrentItem(ItemPosition currentItem, ISkillArgs args)
		{
			this.currentItem = currentItem;
			string cat = ((FreeGameItem)currentItem.GetKey()).GetCat();
			if (cat != null)
			{
				this.lastUseItem[cat] = currentItem;
			}
		}
	}
}
