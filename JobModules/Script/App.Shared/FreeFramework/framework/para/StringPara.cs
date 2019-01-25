using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class StringPara : AbstractPara
	{
		private const long serialVersionUID = 7461497950056856464L;

		public StringPara()
		{
			this.value = string.Empty;
		}

		public StringPara(string name)
		{
			this.name = name;
			this.value = string.Empty;
		}

		public StringPara(string name, string v)
		{
			this.name = name;
			this.value = v;
		}

		public override IPara Initial(string con, string v)
		{
			com.wd.free.para.StringPara p = (com.wd.free.para.StringPara)pool.BorrowObject();
			p.name = EMPTY_NAME;
			p.value = v;
			return p;
		}

		public override string[] GetOps()
		{
			return new string[] { OP_ADD, OP_ASSIGN };
		}

		public override void SetValue(string op, IPara v)
		{
			if (v == null)
			{
				return;
			}
			string v1 = value.ToString();
			string v2 = v.GetValue().ToString();
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
			}
		}

		public override IPoolable Copy()
		{
			return new com.wd.free.para.StringPara(this.name, (string)this.value);
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.StringPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
