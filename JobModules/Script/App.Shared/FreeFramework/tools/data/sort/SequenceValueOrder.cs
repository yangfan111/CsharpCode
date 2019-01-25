using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class SequenceValueOrder : AbstractValueOrder
	{
		private const long serialVersionUID = -767628029318396417L;

		private string stringValues;

		private MyDictionary<string, int> map;

		public SequenceValueOrder()
		{
		}

		public SequenceValueOrder(string[] values)
		{
			this.stringValues = StringUtil.GetStringFromStrings(values, ",");
		}

		public virtual string GetStringValues()
		{
			return stringValues;
		}

		public virtual void SetStringValues(string stringValues)
		{
			this.stringValues = stringValues;
		}

		protected internal override void SetIndex(int[] r, int index, object v, MyDictionary<string, int> set)
		{
			Prepare();
			string s = v.ToString();
			if (map.Count == 1 && map.Keys.Iterator().Next().Equals("true"))
			{
				if (!StringUtil.IsNullOrEmpty(s) && !s.Equals("null") && !s.Equals("false"))
				{
					r[index] = 0;
				}
				else
				{
					r[index] = -1;
				}
			}
			if (map.ContainsKey(s))
			{
				r[index] = map[s];
			}
			else
			{
				r[index] = REMOVE;
			}
		}

		private void Prepare()
		{
			if (map == null || map.Count == 0)
			{
				map = new MyDictionary<string, int>();
				if (!StringUtil.IsNullOrEmpty(stringValues))
				{
					string[] ss = StringUtil.Split(stringValues, ",");
					for (int i = 0; i < ss.Length; i++)
					{
						map[ss[i].Trim()] = i;
					}
				}
			}
		}

		public override string ToString()
		{
			return "ss_" + StringUtil.GetStringFromStrings(Sharpen.Collections.ToArray(map.Keys, new string[0]), "_");
		}

		protected internal override IComparer<MarkedData> GetComparator(string feature)
		{
			return null;
		}
	}
}
