using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class LimitSQL : ISQL
	{
		private const long serialVersionUID = 8073415816067491934L;

		private int start;

		private int count;

		public LimitSQL()
		{
			this.start = 0;
			this.count = 0;
		}

		public LimitSQL(int count)
		{
			this.start = 0;
			this.count = count;
		}

		public LimitSQL(int start, int count)
		{
			this.start = start;
			this.count = count;
		}

		public virtual int GetStart()
		{
			return start;
		}

		public virtual void SetStart(int start)
		{
			this.start = start;
		}

		public virtual int GetCount()
		{
			return count;
		}

		public virtual void SetCount(int count)
		{
			this.count = count;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (count == 0 && start == 0)
			{
				return string.Empty;
			}
			else
			{
				if (count != 0 && start == 0)
				{
					return " limit " + count;
				}
				else
				{
					return " limit " + start + ", " + count;
				}
			}
		}
	}
}
