using System;
using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.exception;

namespace com.wd.free.para
{
	[System.Serializable]
	public class FloatPara : AbstractPara, IComparable<com.wd.free.para.FloatPara>
	{
		private const long serialVersionUID = -4601862312271307979L;

		public FloatPara()
			: base()
		{
			this.value = 0f;
		}

		public FloatPara(string name)
			: base(name)
		{
			this.value = 0f;
		}

		public FloatPara(string name, float value)
			: base(name)
		{
			this.value = value;
		}

		public virtual int CompareTo(com.wd.free.para.FloatPara o)
		{
			float f = (float)value - (float)o.value;
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

		private float GetFloat(object v)
		{
			if (v is float)
			{
				return (float)v;
			}
			else
			{
				if (v is double)
				{
					return (float)(double)v;
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
								return (float)NumberUtil.GetDouble((string)v);
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
			float v1 = GetFloat(value);
			float v2 = GetFloat(v.GetValue());
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
			float v1 = GetFloat(value);
			float v2 = GetFloat(v.GetValue());
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
			com.wd.free.para.FloatPara p = (com.wd.free.para.FloatPara)pool.BorrowObject();
			p.name = EMPTY_NAME;
			try
			{
				p.value = float.Parse(v);
			}
			catch (Exception)
			{
				p.value = 0;
			}
			return p;
		}

		public override IPoolable Copy()
		{
			return new com.wd.free.para.FloatPara(this.name, (float)this.value);
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.FloatPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
