using System.Collections.Generic;
using Sharpen;
using com.wd.free.item;

namespace gameplay.gamerule.free.item
{
	public class SceneItemManager
	{
		private IDictionary<int, FreeItem> itemMap;

		public SceneItemManager()
		{
			itemMap = new Dictionary<int, FreeItem>();
		}

		public virtual void AddSceneItem(FreeItem item)
		{
			itemMap[item.GetId()] = item;
		}

		public virtual void Remove(int id)
		{
			itemMap.Remove(id);
		}

		public virtual void Remove(FreeItem item)
		{
			itemMap.Remove(item.GetId());
		}

		public virtual FreeItem GetItem(int id)
		{
			return itemMap[id];
		}
	}
}
