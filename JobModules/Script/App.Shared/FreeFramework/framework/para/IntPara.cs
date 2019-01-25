using System;
using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.exception;

namespace com.wd.free.para
{
	[System.Serializable]
	public class IntPara : AbstractPara, IComparable<com.wd.free.para.IntPara>
	{
		private const long serialVersionUID = -6695618994224887482L;

		public IntPara()
			: base()
		{
			this.value = 0;
		}

		public IntPara(string name)
			: base(name)
		{
			this.value = 0;
		}

		public IntPara(string name, int value)
			: base(name)
		{
			this.value = value;
		}

		public virtual int CompareTo(com.wd.free.para.IntPara o)
		{
			return (int)value - (int)o.value;
		}

		public override string[] GetConditions()
		{
			return new string[] { CON_EQUAL, CON_GE, CON_GREATER, CON_LE, CON_LESS, CON_NOT_EQUAL };
		}

		private int GetInt(object v)
		{
			if (v is int)
			{
				return (int)v;
			}
			else
			{
				if (v is double)
				{
					return (int)(double)v;
				}
				else
				{
					if (v is long)
					{
						return (int)(long)v;
					}
					else
					{
						if (v is float)
						{
							return (int)(float)v;
						}
						else
						{
							if (v is string)
							{
								return NumberUtil.GetInt((string)v);
							}
						}
					}
				}
			}
			throw new GameConfigExpception(v + " is not a valid long value");
		}

		public override bool Meet(string con, IPara v)
		{
			if (v == null)
			{
				return false;
			}
			int v1 = GetInt(value);
			int v2 = GetInt(v.GetValue());
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
			int v1 = GetInt(value);
			int v2 = GetInt(v.GetValue());
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
			com.wd.free.para.IntPara p = (com.wd.free.para.IntPara)pool.BorrowObject();
			p.name = EMPTY_NAME;
			try
			{
				p.value = int.Parse(v);
			}
			catch (Exception)
			{
				p.value = 0;
			}
			return p;
		}

		public override IPoolable Copy()
		{
			return new com.wd.free.para.IntPara(this.name, (int)this.value);
		}

		[System.NonSerialized]
		internal static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.IntPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
