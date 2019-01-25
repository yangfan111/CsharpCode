using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class DataBlocks
	{
		private const long serialVersionUID = -8932003463854348556L;

		private IList<DataBlock> blocks;

		public DataBlocks()
		{
			this.blocks = new List<DataBlock>();
		}

		public virtual void AddDataBlock(DataBlock block)
		{
			this.blocks.Add(block);
		}

		public virtual void AddDataAtBlock(MarkedData data, int block)
		{
			while (blocks.Count <= block)
			{
				blocks.Add(new DataBlock());
			}
			blocks[block].AddMarkedData(data);
		}

		public virtual void RemoveEmptyBlokcs()
		{
			IList<DataBlock> temp = new List<DataBlock>();
			foreach (DataBlock db in blocks)
			{
				if (db.Count() != 0)
				{
					temp.Add(db);
				}
			}
			blocks = temp;
		}

		// 与操作时，记blocks1 = a1,a2..an, blocks2 = b1,b2...bm
		// 则返回结果为 newblocks = a1*b1, a1*b2...a1*bm, a2*b1 ... an*b1, an*b2...an*bm
		// 一共分为n*m层次，当前blocks拥有更高的优先级.
		public virtual com.cpkf.yyjd.tools.data.sort.DataBlocks AndMerge(com.cpkf.yyjd.tools.data.sort.DataBlocks anotherBlocks)
		{
			com.cpkf.yyjd.tools.data.sort.DataBlocks result = new com.cpkf.yyjd.tools.data.sort.DataBlocks();
			foreach (DataBlock block in blocks)
			{
				foreach (DataBlock ab in anotherBlocks.GetBlocks())
				{
					result.AddDataBlock(block.AndDataBlock(ab));
				}
			}
			return result;
		}

		public virtual IList<IFeaturable> GetAllDatas()
		{
			IList<IFeaturable> list = new List<IFeaturable>();
			foreach (DataBlock bl in blocks)
			{
				IList<IFeaturable> sub = bl.GetDataList();
				foreach (IFeaturable fe in sub)
				{
					if (fe != null)
					{
						list.Add(fe);
					}
				}
			}
			return list;
		}

		internal virtual DataBlock GetMarkedDatas()
		{
			DataBlock block = new DataBlock();
			foreach (DataBlock bl in blocks)
			{
				block.AddBlock(bl);
			}
			return block;
		}

		public virtual com.cpkf.yyjd.tools.data.sort.DataBlocks AppendMerge(com.cpkf.yyjd.tools.data.sort.DataBlocks anotherBlocks)
		{
			com.cpkf.yyjd.tools.data.sort.DataBlocks result = new com.cpkf.yyjd.tools.data.sort.DataBlocks();
			foreach (DataBlock block in blocks)
			{
				result.AddDataBlock(block);
			}
			DataBlock bl = result.GetMarkedDatas();
			foreach (DataBlock block_1 in anotherBlocks.GetBlocks())
			{
				block_1.RemoveBlock(bl);
				result.AddDataBlock(block_1);
			}
			return result;
		}

		// 或操作时，取消所有的分块，按照一个块返回所有可能
		public virtual com.cpkf.yyjd.tools.data.sort.DataBlocks OrMerge(com.cpkf.yyjd.tools.data.sort.DataBlocks anotherBlocks)
		{
			com.cpkf.yyjd.tools.data.sort.DataBlocks result = new com.cpkf.yyjd.tools.data.sort.DataBlocks();
			DataBlock db = new DataBlock();
			foreach (DataBlock block in blocks)
			{
				db = db.OrDataBlock(block);
			}
			foreach (DataBlock ab in anotherBlocks.GetBlocks())
			{
				db = db.OrDataBlock(ab);
			}
			result.AddDataBlock(db);
			return result;
		}

		public virtual void AddDataBlocks(com.cpkf.yyjd.tools.data.sort.DataBlocks blocks)
		{
			Sharpen.Collections.AddAll(this.blocks, blocks.GetBlocks());
		}

		public virtual IList<DataBlock> GetBlocks()
		{
			return blocks;
		}

		public virtual void SetBlocks(IList<DataBlock> blocks)
		{
			this.blocks = blocks;
		}

		public virtual void Clear()
		{
			this.blocks.Clear();
		}

		public override string ToString()
		{
			return this.GetType().Name + "[" + this.blocks + "]";
		}
	}
}
