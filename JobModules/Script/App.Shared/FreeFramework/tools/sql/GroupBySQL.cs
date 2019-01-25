using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class GroupBySQL : ISQL
	{
		private const long serialVersionUID = 1558446713198103334L;

		private FieldsSQL fields;

		public GroupBySQL()
		{
			this.fields = new FieldsSQL();
		}

		public virtual void AddField(string field)
		{
			this.fields.AddField(field);
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
				return " group by " + fields.GetSQLString(type) + SQLUtils.BLANK;
			}
		}
	}
}
