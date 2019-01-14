using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class SQLTable : ISQL
	{
		private const long serialVersionUID = 1954729741879429984L;

		private FieldsSQL fields;

		public SQLTable()
		{
			this.fields = new FieldsSQL();
		}

		public virtual void AddTable(string table)
		{
			this.fields.AddField(table);
		}

		public virtual string GetTable(int index)
		{
			return Sharpen.Collections.ToArray(fields.GetFields(), new string[] {  })[index];
		}

		public virtual int Size()
		{
			return fields.Size();
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (fields.Size() == 0)
			{
				return string.Empty;
			}
			else
			{
				return SQLUtils.BLANK + fields.GetSQLString(type) + SQLUtils.BLANK;
			}
		}
	}
}
