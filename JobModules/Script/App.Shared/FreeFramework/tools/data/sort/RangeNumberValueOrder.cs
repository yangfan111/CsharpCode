using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class RangeNumberValueOrder : AbstractValueOrder
	{
		private const long serialVersionUID = -8423378689478782174L;

		private double start;

		private double end;

		private bool containsStart;

		private bool containsEnd;

		public RangeNumberValueOrder()
		{
		}

		public RangeNumberValueOrder(double start, double end)
		{
			this.start = start;
			this.end = end;
		}

		public RangeNumberValueOrder(double start, double end, bool containsStart, bool containsEnd)
		{
			this.start = start;
			this.end = end;
			this.containsStart = containsStart;
			this.containsEnd = containsEnd;
		}

		protected internal override IComparer<MarkedData> GetComparator(string feature)
		{
			return null;
		}

		protected internal override void SetIndex(int[] r, int i, object v, MyDictionary<string, int> set)
		{
			if (v is int || v is double || (v != null && StringUtil.IsNumberString(v.ToString())))
			{
				int iv = (int)double.Parse(v.ToString());
				if (iv >= start && iv <= end)
				{
					r[i] = 0;
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

		public virtual double GetStart()
		{
			return start;
		}

		public virtual void SetStart(double start)
		{
			this.start = start;
		}

		public virtual double GetEnd()
		{
			return end;
		}

		public virtual void SetEnd(double end)
		{
			this.end = end;
		}

		public virtual bool IsContainsStart()
		{
			return containsStart;
		}

		public virtual void SetContainsStart(bool containsStart)
		{
			this.containsStart = containsStart;
		}

		public virtual bool IsContainsEnd()
		{
			return containsEnd;
		}

		public virtual void SetContainsEnd(bool containsEnd)
		{
			this.containsEnd = containsEnd;
		}

		public override string ToString()
		{
			return "nr_" + start + "_" + end;
		}
	}
}
