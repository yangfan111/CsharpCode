using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class RangeStringValueOrder : AbstractValueOrder
	{
		private const long serialVersionUID = -7377061476226919247L;

		private ICollection<string> values;

		private string stringValues;

		private bool notContains;

		public RangeStringValueOrder()
		{
			this.notContains = false;
		}

		public RangeStringValueOrder(string[] values)
		{
			this.stringValues = StringUtil.GetStringFromStrings(values, ",");
			this.notContains = false;
		}

		public RangeStringValueOrder(string[] values, bool notContains)
			: this(values)
		{
			this.notContains = notContains;
		}

		public virtual bool IsNotContains()
		{
			return notContains;
		}

		public virtual void SetNotContains(bool notContains)
		{
			this.notContains = notContains;
		}

		public virtual string GetStringValues()
		{
			return stringValues;
		}

		public virtual void SetStringValues(string stringValues)
		{
			this.stringValues = stringValues;
		}

		protected internal override IComparer<MarkedData> GetComparator(string feature)
		{
			return null;
		}

		protected internal override void SetIndex(int[] r, int index, object v, MyDictionary<string, int> set)
		{
			Prepare();
			if (v is string)
			{
				string s = (string)v;
				if (values.Count == 1 && values.Iterator().Next().Equals("true"))
				{
					if (!StringUtil.IsNullOrEmpty(s) && !s.Equals("null") && !s.Equals("false"))
					{
						r[index] = 0;
					}
					else
					{
						r[index] = REMOVE;
					}
				}
				if (values.Contains(v))
				{
					r[index] = 0;
				}
				else
				{
					r[index] = REMOVE;
				}
			}
			else
			{
				r[index] = REMOVE;
			}
		}

		private void Prepare()
		{
			if ((values == null || values.Count == 0))
			{
				values = new HashSet<string>();
				if (!StringUtil.IsNullOrEmpty(stringValues))
				{
					string[] ss = stringValues.Split(",");
					foreach (string s in ss)
					{
						values.Add(s.Trim());
					}
				}
			}
		}

		public virtual ICollection<string> GetValues()
		{
			return values;
		}

		public virtual void SetValues(ICollection<string> values)
		{
			this.values = values;
		}

		public override string ToString()
		{
			return "sr_" + StringUtil.GetStringFromStrings(Sharpen.Collections.ToArray(values, new string[0]), "_");
		}
	}
}
