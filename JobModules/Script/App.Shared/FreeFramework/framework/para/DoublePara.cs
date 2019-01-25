using System;
using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.exception;

namespace com.wd.free.para
{
	[System.Serializable]
	public class DoublePara : AbstractPara, IComparable<com.wd.free.para.DoublePara>
	{
		private const long serialVersionUID = -8882601229586450862L;

		public DoublePara()
			: base()
		{
			this.value = 0d;
		}

		public DoublePara(string name)
			: base(name)
		{
			this.value = 0d;
		}

		public DoublePara(string name, double value)
			: base(name)
		{
			this.value = value;
		}

		public virtual int CompareTo(com.wd.free.para.DoublePara o)
		{
			double f = (double)value - (double)o.value;
			if (f > 0)
			{
				return 1;
			}
			else
			{
				if (f == 0)
				{
					return 0;
				}
				else
				{
					return -1;
				}
			}
		}

		public override string[] GetConditions()
		{
			return new string[] { CON_EQUAL, CON_GE, CON_GREATER, CON_LE, CON_LESS, CON_NOT_EQUAL };
		}

		private double GetDouble(object v)
		{
			if (v is double)
			{
				return (double)v;
			}
			else
			{
				if (v is float)
				{
					return (float)v;
				}
				else
				{
					if (v is int)
					{
						return (int)v;
					}
					else
					{
						if (v is long)
						{
							return (long)v;
						}
						else
						{
							if (v is string)
							{
								return NumberUtil.GetDouble((string)v);
							}
						}
					}
				}
			}
			throw new GameConfigExpception(v + " is not a valid double value");
		}

		public override bool Meet(string con, IPara v)
		{
			if (v == null)
			{
				return false;
			}
			double v1 = GetDouble(value);
			double v2 = GetDouble(v.GetValue());
			switch (con)
			{
				case CON_EQUAL:
				{
					return v1 == v2;
				}

				case CON_NOT_EQUAL:
				{
					return v1 != v2;
				}

				case CON_GE:
				{
					return v1 >= v2;
				}

				case CON_LE:
				{
					return v1 <= v2;
				}

				case CON_GREATER:
				{
					return v1 > v2;
				}

				case CON_LESS:
				{
					return v1 < v2;
				}
			}
			return false;
		}

		public override string[] GetOps()
		{
			return new string[] { OP_ADD, OP_ASSIGN, OP_DIVIDE, OP_MINUS, OP_MOD, OP_MULTI };
		}

		public override void SetValue(string op, IPara v)
		{
			if (v == null)
			{
				return;
			}
			double v1 = GetDouble(value);
			double v2 = GetDouble(v.GetValue());
			switch (op)
			{
				case OP_ASSIGN:
				{
					value = v2;
					break;
				}

				case OP_ADD:
				{
					value = v1 + v2;
					break;
				}

				case OP_MINUS:
				{
					value = v1 - v2;
					break;
				}

				case OP_MULTI:
				{
					value = v1 * v2;
					break;
				}

				case OP_DIVIDE:
				{
					value = v1 / v2;
					break;
				}

				case OP_MOD:
				{
					value = v1 % v2;
					break;
				}
			}
		}

		public override IPara Initial(string con, string v)
		{
			com.wd.free.para.DoublePara p = (com.wd.free.para.DoublePara)pool.BorrowObject();
			p.name = EMPTY_NAME;
			try
			{
				p.value = double.Parse(v);
			}
			catch (Exception)
			{
				p.value = 0;
			}
			return p;
		}

		public override IPoolable Copy()
		{
			return new com.wd.free.para.DoublePara(this.name, (double)this.value);
		}

		internal static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.DoublePara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
