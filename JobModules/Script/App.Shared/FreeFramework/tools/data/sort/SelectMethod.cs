using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class SelectMethod
	{
		private IList<OrderSequence> sequences;

		public SelectMethod()
		{
			this.sequences = new List<OrderSequence>();
		}

		public SelectMethod(ISelectOrder order)
		{
			this.sequences = new List<OrderSequence>();
			OrderSequence os = new OrderSequence();
			os.AddSelectOrder(order);
			this.sequences.Add(os);
		}

		public SelectMethod(string exp)
			: this(ClauseSelectOrder.FromExpression(exp))
		{
		}

		public virtual void AddOrderSequence(OrderSequence sequence)
		{
			this.sequences.Add(sequence);
		}

		public virtual IList<OrderSequence> GetSequences()
		{
			return sequences;
		}

		public virtual void SetSequences(IList<OrderSequence> sequences)
		{
			this.sequences = sequences;
		}

		public virtual ICollection<string> GetAllFeatures()
		{
			HashSet<string> set = new HashSet<string>();
			foreach (OrderSequence os in sequences)
			{
				Sharpen.Collections.AddAll(set, os.GetAllFeatures());
			}
			return set;
		}

		public virtual bool Meet(IFeaturable fe)
		{
			DataBlock bl = new DataBlock();
			bl.AddData(fe);
			return this.Select(bl).GetAllDatas().Count == 1;
		}

		public virtual DataBlocks Select(DataBlock block)
		{
			DataBlocks result = new DataBlocks();
			if (sequences.Count > 0)
			{
				foreach (OrderSequence sequence in sequences)
				{
					DataBlocks bd = sequence.Sort(block);
					result = result.AppendMerge(bd);
				}
			}
			else
			{
				result.AddDataBlock(block);
			}
			result.RemoveEmptyBlokcs();
			return result;
		}
	}
}
