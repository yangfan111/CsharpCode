using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class OrderBySQL : ISQL
	{
		private const long serialVersionUID = 1690455484006921669L;

		private FieldsSQL fields;

		public OrderBySQL()
		{
			this.fields = new FieldsSQL();
		}

		public virtual void AddField(string field)
		{
			this.fields.AddField(field);
		}

		public virtual void AddDescField(string field)
		{
			this.fields.AddField(field + " desc ");
		}

		public virtual FieldsSQL GetFields()
		{
			return this.fields;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (fields.Size() == 0)
			{
				return string.Empty;
			}
			else
			{
				string field = fields.GetSQLString(type);
				return " order by " + field + SQLUtils.BLANK;
			}
		}
	}
}
