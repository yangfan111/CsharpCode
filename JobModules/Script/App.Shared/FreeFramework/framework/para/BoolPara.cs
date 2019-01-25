using System;
using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class BoolPara : AbstractPara
	{
		private const long serialVersionUID = -8824870098367160681L;

		public BoolPara()
		{
			this.value = false;
		}

		public BoolPara(string name)
			: base(name)
		{
			this.value = false;
		}

		public BoolPara(string name, bool v)
			: base(name)
		{
			this.value = v;
		}

		public override IPara Initial(string con, string v)
		{
			com.wd.free.para.BoolPara p = (com.wd.free.para.BoolPara)pool.BorrowObject();
			p.name = EMPTY_NAME;
			try
			{
				p.value = bool.Parse(v);
			}
			catch (Exception)
			{
				p.value = false;
			}
			return p;
		}

		public override IPoolable Copy()
		{
			return new com.wd.free.para.BoolPara(this.name, (bool)this.value);
		}

		internal static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.BoolPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
