using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.wd.free.item
{
	public class InventoryManager : Iterable<ItemInventory>
	{
		public const string DEFAULT = "default";

		private MyDictionary<string, ItemInventory> map;

		private string currentInventory;

		public InventoryManager()
		{
			this.map = new MyDictionary<string, ItemInventory>();
		}

		public virtual void AddDefaultInventory(ItemInventory @in)
		{
			@in.SetName(DEFAULT);
			this.map[DEFAULT] = @in;
		}

		public virtual void AddInventory(string name, ItemInventory @in)
		{
			@in.SetName(name);
			this.map[name] = @in;
		}

		public virtual void UseInventory(string name)
		{
			this.currentInventory = name;
		}

		public virtual ItemInventory GetCurrentInventory()
		{
			if (StringUtil.IsNullOrEmpty(currentInventory))
			{
				currentInventory = DEFAULT;
			}
			return map[currentInventory];
		}

		public virtual ItemInventory GetDefaultInventory()
		{
			return map[DEFAULT];
		}

		public virtual string[] GetInventoryNames()
		{
			return Sharpen.Collections.ToArray(map.Keys, new string[0]);
		}

		public virtual ItemInventory GetInventory(string name)
		{
			return this.map[name];
		}

		public override Sharpen.Iterator<ItemInventory> Iterator()
		{
			return map.Values.Iterator();
		}
	}
}
