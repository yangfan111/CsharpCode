using System.Text;
using Sharpen;
using com.cpkf.yyjd.tools.data;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class UpdateSQL : ISQL
	{
		private const long serialVersionUID = 3314259832917685587L;

		private SQLTable tables;

		private FieldValuesSQL fieldValues;

		private WhereSQL where;

		public UpdateSQL()
		{
			this.tables = new SQLTable();
			this.fieldValues = new FieldValuesSQL();
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

		public virtual void AddFieldValues(DataRecord dr)
		{
			foreach (string field in dr.GetAllFields())
			{
				this.fieldValues.AddFieldValue(field, dr.GetFieldValue(field));
			}
		}

		public virtual void AddFiendValue(string field, string value)
		{
			this.fieldValues.AddFieldValue(field, value);
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("update ");
			sb.Append(tables.GetSQLString(type));
			sb.Append(" set ");
			sb.Append(fieldValues.GetSQLString(type));
			sb.Append(where.GetSQLString(type));
			return SQLUtils.RemoveMultiBlank(sb.ToString().Trim());
		}
	}
}
