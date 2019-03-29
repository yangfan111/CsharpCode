using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class SortedSelectOrder : ISelectOrder
	{
		private const long serialVersionUID = 9094133092796713995L;

		private IList<ISelectOrder> orders;

		public SortedSelectOrder()
		{
			this.orders = new List<ISelectOrder>();
		}

		public virtual void AddSelectOrder(ISelectOrder order)
		{
			this.orders.Add(order);
		}

		public virtual DataBlocks Sort(DataBlock block)
		{
			DataBlocks result = new DataBlocks();
			foreach (ISelectOrder order in orders)
			{
				DataBlocks dbs = order.Sort(block);
				result = result.AppendMerge(dbs);
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

		public virtual IList<ISelectOrder> GetOrders()
		{
			return orders;
		}

		public virtual void SetOrders(IList<ISelectOrder> orders)
		{
			this.orders = orders;
		}
	}
}
