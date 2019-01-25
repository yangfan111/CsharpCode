using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class SameSQL : ISQL
	{
		private const long serialVersionUID = -3479331882891751395L;

		private string sql;

		public SameSQL(string sql)
			: base()
		{
			this.sql = sql;
		}

		public virtual string GetSql()
		{
			return sql;
		}

		public virtual void SetSql(string sql)
		{
			this.sql = sql;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			return sql;
		}
	}
}
