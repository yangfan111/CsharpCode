using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public abstract class AbstractValueOrder : IValueOrder
	{
		private const long serialVersionUID = -6013579513376842003L;

		private IComparer<MarkedData> comparator;

		public virtual bool IsRemoved(int index)
		{
			return index == REMOVE;
		}

		public override int[] Sort(IList<MarkedData> datas, string feature)
		{
			int[] r = new int[datas.Count];
		    MyDictionary<string, int> set = new MyDictionary<string, int>();
			if (comparator == null)
			{
				comparator = GetComparator(feature);
			}
			if (comparator != null)
			{
				datas.Sort(GetComparator(feature));
			}
			for (int i = 0; i < datas.Count; i++)
			{
				MarkedData data = datas[i];
				if (data != null)
				{
					IFeaturable fe = data.GetFe();
					if (fe != null)
					{
						object v = fe.GetFeatureValue(feature);
						if (v != null)
						{
							SetIndex(r, i, v, set);
						}
						else
						{
							r[i] = REMOVE;
						}
					}
					else
					{
						r[i] = REMOVE;
					}
				}
				else
				{
					r[i] = REMOVE;
				}
			}
			return r;
		}

		protected internal abstract IComparer<MarkedData> GetComparator(string feature);

		protected internal abstract void SetIndex(int[] r, int index, object v, MyDictionary<string, int> set);
	}
}
