using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class CharValueOrder : AbstractValueOrder
	{
		private const long serialVersionUID = 6706324246785664306L;

		public CharValueOrder()
		{
		}

		protected internal override IComparer<MarkedData> GetComparator(string feature)
		{
			return null;
		}

		protected internal override void SetIndex(int[] r, int index, object v, MyDictionary<string, int> set)
		{
			if (v is string)
			{
				string s = (string)v;
				if (!set.ContainsKey(s))
				{
					set[s] = set.Count;
				}
				r[index] = set[s];
			}
			else
			{
				r[index] = REMOVE;
			}
		}
	}
}
