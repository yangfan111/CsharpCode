using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class TimeElapsePara : AbstractPara
	{
		private const long serialVersionUID = -5389493185692272386L;

		private int totalTime;

		private int elapseTime;

		public TimeElapsePara()
			: base()
		{
		}

		public TimeElapsePara(string name)
			: base(name)
		{
		}

		public TimeElapsePara(string name, int totalTime)
			: base(name)
		{
			this.totalTime = totalTime;
		}

		public override object GetValue()
		{
			if (totalTime >= elapseTime)
			{
				return this.totalTime - this.elapseTime;
			}
			else
			{
				return 0;
			}
		}

		public override string[] GetOps()
		{
			return new string[] { OP_ADD, OP_MINUS, OP_ASSIGN };
		}

		public override void SetValue(string op, IPara v)
		{
			if (OP_ADD.Equals(op))
			{
				elapseTime = elapseTime + (int)v.GetValue();
			}
			else
			{
				if (OP_MINUS.Equals(op))
				{
					elapseTime = elapseTime - (int)v.GetValue();
				}
				else
				{
					if (OP_ASSIGN.Equals(op))
					{
						if (v is IntPara)
						{
							totalTime = (int)v.GetValue();
							elapseTime = 0;
						}
						else
						{
							if (v is com.wd.free.para.TimeElapsePara)
							{
								com.wd.free.para.TimeElapsePara tep = (com.wd.free.para.TimeElapsePara)v;
								totalTime = tep.totalTime;
								elapseTime = tep.elapseTime;
							}
						}
					}
				}
			}
		}

		public override string[] GetConditions()
		{
			return new string[] { CON_EQUAL, CON_GE, CON_GREATER, CON_LE, CON_LESS, CON_NOT_EQUAL };
		}

		public override bool Meet(string con, IPara v)
		{
			int v1 = (int)GetValue();
			int v2 = (int)v.GetValue();
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

		public override string ToString()
		{
			return totalTime + " of " + elapseTime;
		}

		public override IPara Initial(string con, string v)
		{
			IntPara ip = (IntPara)IntPara.pool.BorrowObject();
			ip.name = EMPTY_NAME;
			ip.value = int.Parse(v);
			return ip;
		}

		public override IPoolable Copy()
		{
			com.wd.free.para.TimeElapsePara t = new com.wd.free.para.TimeElapsePara(this.name);
			t.totalTime = this.totalTime;
			t.elapseTime = this.elapseTime;
			return t;
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.TimeElapsePara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
