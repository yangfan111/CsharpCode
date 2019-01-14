using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class WhereSQL : ISQL
	{
		private const long serialVersionUID = 7905349494437641117L;

		private FieldValuesSQL fieldValues;

		public WhereSQL()
		{
			this.fieldValues = new FieldValuesSQL(" and ");
		}

		public virtual void AddCondition(string field, string value)
		{
			this.fieldValues.AddFieldValue(field, value);
		}

		public virtual void AddCondition(SQLCondition condition)
		{
			this.fieldValues.AddSQLCondition(condition);
		}

		public virtual void AddCondition(ISQL condition)
		{
			this.fieldValues.AddSQLCondition(condition);
		}

		public virtual void AddCondition(BooleanCondition condition)
		{
			this.fieldValues.AddSQLCondition(condition);
		}

		public virtual FieldValuesSQL GetConditions()
		{
			return fieldValues;
		}

		public virtual com.cpkf.yyjd.tools.sql.WhereSQL Clone()
		{
			return (com.cpkf.yyjd.tools.sql.WhereSQL)SerializeUtil.ByteToObject(SerializeUtil.ObjectToByte(this));
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (fieldValues.Size() == 0 || StringUtil.IsNullOrEmpty(fieldValues.GetSQLString(type)))
			{
				return string.Empty;
			}
			else
			{
				return " where " + fieldValues.GetSQLString(type);
			}
		}
	}
}
