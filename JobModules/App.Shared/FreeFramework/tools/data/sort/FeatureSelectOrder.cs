using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class FeatureSelectOrder : ISelectOrder
	{
		private const long serialVersionUID = 1119076988934964658L;

		private string feature;

		private IValueOrder value;

		public FeatureSelectOrder()
		{
		}

		public FeatureSelectOrder(string feature, IValueOrder value)
		{
			this.feature = feature;
			this.value = value;
		}

		public virtual string GetFeature()
		{
			return feature;
		}

		public virtual void SetFeature(string feature)
		{
			this.feature = feature;
		}

		public virtual IValueOrder GetValue()
		{
			return value;
		}

		public virtual void SetValue(IValueOrder value)
		{
			this.value = value;
		}

		public virtual DataBlocks Sort(DataBlock block)
		{
			DataBlocks result = new DataBlocks();
			IList<MarkedData> list = block.GetMarkedDataList();
			int[] index = value.Sort(list, feature);
			for (int i = 0; i < list.Count; i++)
			{
				if (IValueOrder.REMOVE != index[i])
				{
					result.AddDataAtBlock(list[i], index[i]);
				}
			}
			result.RemoveEmptyBlokcs();
			return result;
		}

		public virtual ICollection<string> GetAllFeatures()
		{
			HashSet<string> set = new HashSet<string>();
			set.Add(feature);
			return set;
		}
	}
}
