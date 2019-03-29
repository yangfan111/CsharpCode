using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class UpdatePara : AbstractPara
	{
		private const long serialVersionUID = 6146836621145028581L;

		private int current;

		public override object GetValue()
		{
			return current;
		}

		public override string[] GetConditions()
		{
			return new string[] { "==", ">", "<", ">=", "<=", "<>" };
		}

		public override bool Meet(string con, IPara v)
		{
			if (v is IntPara || v is UpdatePara)
			{
				int ev = ((int)GetValue());
				int cv = ((int)v.GetValue());
				if ("==".Equals(con))
				{
					return ev == cv;
				}
				else
				{
					if (">".Equals(con))
					{
						return ev > cv;
					}
					else
					{
						if ("<".Equals(con))
						{
							return ev < cv;
						}
						else
						{
							if (">=".Equals(con))
							{
								return ev >= cv;
							}
							else
							{
								if ("<=".Equals(con))
								{
									return ev <= cv;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public override IPara Initial(string con, string v)
		{
			UpdatePara para = new UpdatePara();
			return para;
		}

		public override IPoolable Copy()
		{
			return new UpdatePara();
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new UpdatePara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
