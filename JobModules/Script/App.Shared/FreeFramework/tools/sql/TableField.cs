using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class TableField : ISQL
	{
		private const long serialVersionUID = 6780927375233634166L;

		private string fieldName;

		private string fieldType;

		private bool canNull;

		private string defaultValue = string.Empty;

		private bool autoIncrement;

		public TableField()
			: base()
		{
		}

		public TableField(string fieldName, string fieldType, bool canNull, string defaultValue, bool autoIncrement)
			: base()
		{
			this.fieldName = fieldName;
			this.fieldType = fieldType;
			this.canNull = canNull;
			this.defaultValue = defaultValue;
			this.autoIncrement = autoIncrement;
		}

		public virtual bool IsAutoIncrement()
		{
			return autoIncrement;
		}

		public virtual void SetAutoIncrement(bool autoIncrement)
		{
			this.autoIncrement = autoIncrement;
		}

		public virtual string GetFieldName()
		{
			return fieldName;
		}

		public virtual void SetFieldName(string fieldName)
		{
			this.fieldName = fieldName;
		}

		public virtual string GetFieldType()
		{
			return fieldType;
		}

		public virtual void SetFieldType(string fieldType)
		{
			this.fieldType = fieldType;
		}

		public virtual bool IsCanNull()
		{
			return canNull;
		}

		public virtual void SetCanNull(bool canNull)
		{
			this.canNull = canNull;
		}

		public virtual string GetDefaultValue()
		{
			return defaultValue;
		}

		public virtual void SetDefaultValue(string defaultValue)
		{
			this.defaultValue = defaultValue;
		}

		public static long GetSerialVersionUID()
		{
			return serialVersionUID;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (!autoIncrement)
			{
				return "`" + fieldName + "`" + SQLUtils.BLANK + fieldType + SQLUtils.BLANK + (canNull ? string.Empty : " NOT NULL ") + " DEFAULT '" + defaultValue + "'";
			}
			else
			{
				return "`" + fieldName + "`" + SQLUtils.BLANK + fieldType + SQLUtils.BLANK + (canNull ? string.Empty : " NOT NULL ") + " AUTO_INCREMENT ";
			}
		}
	}
}
