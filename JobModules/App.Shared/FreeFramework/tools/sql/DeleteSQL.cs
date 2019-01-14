using System.Text;
using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class DeleteSQL : ISQL
	{
		private const long serialVersionUID = -2945644236668850194L;

		private SQLTable tables;

		private WhereSQL where;

		public DeleteSQL()
		{
			this.tables = new SQLTable();
			this.where = new WhereSQL();
		}

		public virtual void AddTable(string table)
		{
			this.tables.AddTable(table);
		}

		public virtual void AddWhereCondition(string field, string value)
		{
			this.where.AddCondition(field, value);
		}

		public virtual void AddWhereCondition(SQLCondition condition)
		{
			this.where.AddCondition(condition);
		}

		public virtual SQLTable GetTables()
		{
			return tables;
		}

		public virtual WhereSQL GetWhere()
		{
			return where;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("delete from ");
			sb.Append(tables.GetSQLString(type));
			sb.Append(where.GetSQLString(type));
			return SQLUtils.RemoveMultiBlank(sb.ToString());
		}
	}
}
