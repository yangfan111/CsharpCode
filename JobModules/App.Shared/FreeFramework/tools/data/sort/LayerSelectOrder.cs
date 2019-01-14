using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class LayerSelectOrder : ISelectOrder
	{
		private const long serialVersionUID = 720907101916391579L;

		private IList<ISelectOrder> orders;

		public LayerSelectOrder()
		{
			this.orders = new List<ISelectOrder>();
		}

		public virtual void AddSelectOrder(ISelectOrder order)
		{
			this.orders.Add(order);
		}

		public virtual IList<ISelectOrder> GetOrders()
		{
			return orders;
		}

		public virtual void SetOrders(IList<ISelectOrder> orders)
		{
			this.orders = orders;
		}

		public virtual DataBlocks Sort(DataBlock block)
		{
			DataBlocks result = new DataBlocks();
			result.AddDataBlock(block);
			DataBlocks temp = new DataBlocks();
			for (int i = 0; i < orders.Count; i++)
			{
				temp.Clear();
				foreach (DataBlock b in result.GetBlocks())
				{
					temp.AddDataBlocks(orders[i].Sort(b));
				}
				result.Clear();
				foreach (DataBlock db in temp.GetBlocks())
				{
					result.AddDataBlock(db);
				}
			}
			result.RemoveEmptyBlokcs();
			return result;
		}

		public virtual ICollection<string> GetAllFeatures()
		{
			HashSet<string> set = new HashSet<string>();
			foreach (ISelectOrder order in orders)
			{
				Sharpen.Collections.AddAll(set, order.GetAllFeatures());
			}
			return set;
		}
	}
}
