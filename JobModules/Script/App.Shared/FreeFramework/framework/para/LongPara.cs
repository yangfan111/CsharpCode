using System;
using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.exception;

namespace com.wd.free.para
{
	[System.Serializable]
	public class LongPara : AbstractPara, IComparable<com.wd.free.para.LongPara>
	{
		private const long serialVersionUID = 2439252715469717717L;

		public LongPara()
			: base()
		{
			this.value = 0;
		}

		public LongPara(string name)
			: base(name)
		{
			this.value = 0;
		}

		public LongPara(string name, long value)
			: base(name)
		{
			this.value = value;
		}

		public virtual int CompareTo(com.wd.free.para.LongPara o)
		{
			long f = (long)value - (long)o.value;
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

		public override bool Meet(string con, IPara v)
		{
			long v1 = GetLong(value);
			long v2 = GetLong(v.GetValue());
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

		private long GetLong(object v)
		{
			if (v is long)
			{
				return (long)v;
			}
			else
			{
				if (v is double)
				{
					return (long)(double)v;
				}
				else
				{
					if (v is int)
					{
					    return (long)(int)v;
                    }
					else
					{
						if (v is float)
						{
						    return (long)(float)v;
                        }
						else
						{
							if (v is string)
							{
								return (long)NumberUtil.GetDouble((string)v);
							}
						}
					}
				}
			}
			throw new GameConfigExpception(v + " is not a valid long value");
		}

		public override void SetValue(string op, IPara v)
		{
			long v1 = GetLong(value);
			long v2 = GetLong(v.GetValue());
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
			com.wd.free.para.LongPara p = (com.wd.free.para.LongPara)pool.BorrowObject();
			p.name = EMPTY_NAME;
			try
			{
				p.value = long.Parse(v);
			}
			catch (Exception)
			{
				p.value = 0;
			}
			return p;
		}

		public override IPoolable Copy()
		{
			return new com.wd.free.para.LongPara(this.name, (long)this.value);
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.LongPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
