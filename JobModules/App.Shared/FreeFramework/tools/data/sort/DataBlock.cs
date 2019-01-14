using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class DataBlock
	{
		private const long serialVersionUID = -3061238264871813866L;

		private IList<MarkedData> dataList;

		private IList<IFeaturable> feList;

		private Dictionary<int, int> indexMap;

		public DataBlock()
		{
			this.dataList = new List<MarkedData>();
			this.feList = new List<IFeaturable>();
			this.indexMap = new MyDictionary<int, int>();
		}

		public virtual void AddData(IFeaturable data)
		{
			int index = indexMap.Count;
			MarkedData md = new MarkedData(data, index);
			dataList.Add(md);
			this.feList.Add(data);
			indexMap[index] = index;
		}

		internal virtual void AddMarkedData(MarkedData data)
		{
			if (!indexMap.ContainsKey(data.GetIndex()))
			{
				indexMap[data.GetIndex()] = indexMap.Count;
				this.dataList.Add(data);
				this.feList.Add(data.GetFe());
			}
		}

		internal virtual void DeleteMarkedData(MarkedData data)
		{
			if (indexMap.ContainsKey(data.GetIndex()))
			{
				this.dataList.Set(indexMap[data.GetIndex()], null);
				this.feList.Set(indexMap[data.GetIndex()], null);
				this.indexMap.Remove(data.GetIndex());
			}
		}

		internal virtual void AddBlock(com.cpkf.yyjd.tools.data.sort.DataBlock block)
		{
			foreach (MarkedData md in block.dataList)
			{
				if (md != null)
				{
					AddMarkedData(md);
				}
			}
		}

		internal virtual void RemoveBlock(com.cpkf.yyjd.tools.data.sort.DataBlock bl)
		{
			foreach (MarkedData md in bl.dataList)
			{
				if (md != null)
				{
					DeleteMarkedData(md);
				}
			}
		}

		public virtual void AddDataIngoreCheck(IFeaturable data)
		{
			AddData(data);
		}

		public virtual void AddDatas(IList<IFeaturable> list)
		{
			foreach (IFeaturable fe in list)
			{
				AddData(fe);
			}
		}

		public virtual int Count()
		{
			return indexMap.Count;
		}

		internal virtual IList<MarkedData> GetMarkedDataList()
		{
			return dataList;
		}

		public virtual IList<IFeaturable> GetDataList()
		{
			return feList;
		}

		public virtual int Size()
		{
			return indexMap.Count;
		}

		public virtual com.cpkf.yyjd.tools.data.sort.DataBlock AndDataBlock(com.cpkf.yyjd.tools.data.sort.DataBlock block)
		{
			com.cpkf.yyjd.tools.data.sort.DataBlock result = new com.cpkf.yyjd.tools.data.sort.DataBlock();
			foreach (MarkedData md in this.dataList)
			{
				if (this.indexMap.ContainsKey(md.GetIndex()))
				{
					result.AddMarkedData(md);
				}
			}
			return result;
		}

		public virtual com.cpkf.yyjd.tools.data.sort.DataBlock OrDataBlock(com.cpkf.yyjd.tools.data.sort.DataBlock block)
		{
			com.cpkf.yyjd.tools.data.sort.DataBlock result = new com.cpkf.yyjd.tools.data.sort.DataBlock();
			foreach (MarkedData md in this.dataList)
			{
				result.AddMarkedData(md);
			}
			foreach (MarkedData md_1 in block.dataList)
			{
				result.AddMarkedData(md_1);
			}
			return result;
		}

		public override string ToString()
		{
			return this.GetType().Name + "[" + "dataList=" + this.dataList + "]";
		}
	}
}
