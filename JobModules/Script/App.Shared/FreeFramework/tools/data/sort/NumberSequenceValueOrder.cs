using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.data.sort
{
	/// <summary>按照数字大小顺序返回一个排列</summary>
	/// <author>dave</author>
	[System.Serializable]
	public class NumberSequenceValueOrder : AbstractValueOrder
	{
		private const long serialVersionUID = -9136362527122430448L;

		private int min;

		private int max;

		private bool desc;

		public NumberSequenceValueOrder()
		{
			this.max = 1000;
			this.min = 0;
			this.desc = false;
		}

		public NumberSequenceValueOrder(int min, int max)
		{
			this.max = max;
			this.min = min;
			this.desc = false;
		}

		public NumberSequenceValueOrder(int min, int max, bool desc)
		{
			this.max = max;
			this.min = min;
			this.desc = desc;
		}

		protected internal override void SetIndex(int[] r, int i, object v, MyDictionary<string, int> set)
		{
			if (v is int || v is double || (v != null && StringUtil.IsNumberString(v.ToString())))
			{
				int iv = (int)double.Parse(v.ToString());
				if (iv >= min && iv <= max)
				{
					if (!set.ContainsKey(iv.ToString()))
					{
						set[v.ToString()] = set.Count;
					}
					r[i] = set[v.ToString()];
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

		protected internal override IComparer<MarkedData> GetComparator(string feature)
		{
			return new _Comparator_65(this, feature);
		}

		private sealed class _Comparator_65 : IComparer<MarkedData>
		{
			public _Comparator_65(NumberSequenceValueOrder _enclosing, string feature)
			{
				this._enclosing = _enclosing;
				this.feature = feature;
			}

			public int Compare(MarkedData o1, MarkedData o2)
			{
				if ((o1.GetFe() == null || !o1.GetFe().HasFeature(feature)) && (o2.GetFe() == null || !o2.GetFe().HasFeature(feature)))
				{
					return 0;
				}
				if (o1.GetFe() == null || !o1.GetFe().HasFeature(feature))
				{
					if (this._enclosing.desc)
					{
						return 1;
					}
					else
					{
						return -1;
					}
				}
				if (o2.GetFe() == null || !o2.GetFe().HasFeature(feature))
				{
					if (this._enclosing.desc)
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}
				int i1 = (int)double.Parse(o1.GetFe().GetFeatureValue(feature).ToString());
				int i2 = (int)double.Parse(o2.GetFe().GetFeatureValue(feature).ToString());
				if (i1 == i2)
				{
					return 0;
				}
				if (this._enclosing.desc)
				{
					return i2 - i1;
				}
				else
				{
					return i1 - i2;
				}
			}

			private readonly NumberSequenceValueOrder _enclosing;

			private readonly string feature;
		}

		public virtual int GetMax()
		{
			return max;
		}

		public virtual void SetMax(int max)
		{
			this.max = max;
		}

		public virtual bool IsDesc()
		{
			return desc;
		}

		public virtual void SetDesc(bool desc)
		{
			this.desc = desc;
		}

		public virtual int GetMin()
		{
			return min;
		}

		public virtual void SetMin(int min)
		{
			this.min = min;
		}

		public override string ToString()
		{
			return "ns_" + min + "_" + max;
		}
	}
}
