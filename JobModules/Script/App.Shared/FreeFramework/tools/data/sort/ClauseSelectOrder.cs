using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.condition;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class ClauseSelectOrder : ISelectOrder, IExpClause
	{
		private const long serialVersionUID = 8119849591462167518L;

		private IList<ISelectOrder> orList;

		private IList<ISelectOrder> andList;

		private IList<ISelectOrder> appendList;

		public ClauseSelectOrder()
		{
			this.orList = new List<ISelectOrder>();
			this.andList = new List<ISelectOrder>();
			this.appendList = new List<ISelectOrder>();
		}

		public static com.cpkf.yyjd.tools.data.sort.ClauseSelectOrder FromExpression(string exp)
		{
			return (com.cpkf.yyjd.tools.data.sort.ClauseSelectOrder)new ClauseParser<ISelectOrder>().Parse(exp, new ExpSelectOrder(), new com.cpkf.yyjd.tools.data.sort.ClauseSelectOrder());
		}

		public virtual void AddOrSelectOrder(ISelectOrder order)
		{
			this.orList.Add(order);
		}

		public virtual void AddAndSelectOrder(ISelectOrder order)
		{
			this.andList.Add(order);
		}

		public virtual void AddAppendSelectOrder(ISelectOrder order)
		{
			this.appendList.Add(order);
		}

		public virtual DataBlocks Sort(DataBlock block)
		{
			DataBlocks result = new DataBlocks();
			if ((andList == null || andList.Count == 0) && (orList == null || orList.Count == 0) && (appendList == null || appendList.Count == 0))
			{
				result.AddDataBlock(block);
			}
			if (andList != null && andList.Count > 0)
			{
				IList<DataBlock> blocks = new List<DataBlock>();
				blocks.Add(block);
				IList<DataBlock> temps = new List<DataBlock>();
				foreach (ISelectOrder order in andList)
				{
					temps.Clear();
					foreach (DataBlock bl in blocks)
					{
						DataBlocks dbs = order.Sort(bl);
						foreach (DataBlock db in dbs.GetBlocks())
						{
							temps.Add(db);
						}
					}
					if (temps.Count == 0)
					{
						return new DataBlocks();
					}
					blocks.Clear();
					Sharpen.Collections.AddAll(blocks, temps);
				}
				foreach (DataBlock bl_1 in blocks)
				{
					result.AddDataBlock(bl_1);
				}
			}
			if (orList != null && orList.Count > 0)
			{
				DataBlocks orResult = new DataBlocks();
				foreach (ISelectOrder order in orList)
				{
					orResult = orResult.OrMerge(order.Sort(block));
				}
				if (andList != null && andList.Count > 0)
				{
					result = result.AndMerge(orResult);
				}
				else
				{
					result = orResult;
				}
			}
			if (appendList != null && appendList.Count > 0)
			{
				foreach (ISelectOrder order in appendList)
				{
					result = result.AppendMerge(order.Sort(block));
				}
			}
			result.RemoveEmptyBlokcs();
			return result;
		}

		public virtual ICollection<string> GetAllFeatures()
		{
			HashSet<string> set = new HashSet<string>();
			foreach (ISelectOrder order in andList)
			{
				Sharpen.Collections.AddAll(set, order.GetAllFeatures());
			}
			foreach (ISelectOrder order_1 in orList)
			{
				Sharpen.Collections.AddAll(set, order_1.GetAllFeatures());
			}
			return set;
		}

		public virtual IList<ISelectOrder> GetOrList()
		{
			return orList;
		}

		public virtual void SetOrList(IList<ISelectOrder> orList)
		{
			this.orList = orList;
		}

		public virtual IList<ISelectOrder> GetAndList()
		{
			return andList;
		}

		public virtual void SetAndList(IList<ISelectOrder> andList)
		{
			this.andList = andList;
		}

		public virtual void AddAnd(IClausable t, bool negative)
		{
			this.andList.Add((ISelectOrder)t);
		}

		public virtual void AddOr(IClausable t, bool negative)
		{
			this.orList.Add((ISelectOrder)t);
		}

		public virtual IExpClause Clone()
		{
			return new com.cpkf.yyjd.tools.data.sort.ClauseSelectOrder();
		}

		public virtual void AddApend(IClausable t, bool negative)
		{
			this.appendList.Add((ISelectOrder)t);
		}
	}
}
