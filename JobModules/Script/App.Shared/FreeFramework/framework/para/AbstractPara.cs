using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public abstract class AbstractPara : IPara
	{
		private const long serialVersionUID = -7911220538519770370L;

		public const string EMPTY_NAME = "empty_name";

		protected internal string name;

		protected internal string desc;

		protected internal object value;

		protected internal bool isPublic;

		protected internal bool temp;

		public AbstractPara()
		{
		}

		public AbstractPara(string name)
			: base()
		{
			this.name = name;
		}

		public override bool IsTemp()
		{
			return temp;
		}

		public virtual void SetTemp(bool temp)
		{
			this.temp = temp;
		}

		public override bool IsPublic()
		{
			return isPublic;
		}

		public override void SetPublic(bool p)
		{
			this.isPublic = p;
		}

		public override string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public override void SetName(string name)
		{
			this.name = name;
		}

		public virtual void SetValue(object value)
		{
			this.value = value;
		}

		public override string GetName()
		{
			return name;
		}

		public override object GetValue()
		{
			return value;
		}

		public override string[] GetConditions()
		{
			return new string[] { "==", CON_NOT_EQUAL };
		}

		public override bool Meet(string con, IPara v)
		{
			if (v == null)
			{
				return false;
			}
			if ("==".Equals(con))
			{
				if (value != null)
				{
					return value.Equals(v.GetValue());
				}
			}
			else
			{
				if (CON_NOT_EQUAL.Equals(con))
				{
					if (value != null)
					{
						return !value.Equals(v.GetValue());
					}
				}
			}
			return false;
		}

		public override string[] GetOps()
		{
			return new string[] { "=" };
		}

		public override void SetValue(string op, IPara v)
		{
			if (v == null)
			{
				return;
			}
			if ("=".Equals(op))
			{
				value = v.GetValue();
			}
		}

		public override string ToString()
		{
			if (value != null)
			{
				return name + "=" + value.ToString();
			}
			else
			{
				return "null";
			}
		}

		public override IPoolable Borrow()
		{
			if (IsSimple())
			{
				com.wd.free.para.AbstractPara p = (com.wd.free.para.AbstractPara)GetPool().BorrowObject();
				return p;
			}
			else
			{
				return (IPara)Copy();
			}
		}

		protected internal virtual bool IsSimple()
		{
			return ParaUtil.IsBasicPara(this);
		}

		public override void Recycle()
		{
			if (temp)
			{
			}
		}

		// getPool().returnObject(this);
		protected internal abstract ParaPool<IPara> GetPool();
	}
}
