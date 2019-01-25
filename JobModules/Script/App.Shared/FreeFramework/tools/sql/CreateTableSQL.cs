using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class CreateTableSQL : ISQL
	{
		private const long serialVersionUID = -2599858174701900906L;

		private string tableName;

		private FieldValuesSQL fields;

		private TableOptions options;

		public CreateTableSQL()
		{
			this.fields = new FieldValuesSQL();
			this.options = new TableOptions();
		}

		public CreateTableSQL(string tableName)
		{
			this.tableName = tableName;
			this.fields = new FieldValuesSQL();
			this.options = new TableOptions();
		}

		public virtual void AddField(TableField field)
		{
			this.fields.AddSQLCondition(field);
		}

		public virtual void SetPrimaryKey(PrimaryKey key)
		{
			this.fields.AddSQLCondition(key);
		}

		public virtual string GetTableName()
		{
			return tableName;
		}

		public virtual void SetTableName(string tableName)
		{
			this.tableName = tableName;
		}

		public virtual FieldValuesSQL GetFields()
		{
			return fields;
		}

		public virtual void SetFields(FieldValuesSQL fields)
		{
			this.fields = fields;
		}

		public virtual TableOptions GetOptions()
		{
			return options;
		}

		public virtual void SetOptions(TableOptions options)
		{
			this.options = options;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			return SQLUtils.RemoveMultiBlank("CREATE TABLE `" + tableName + "` (" + fields.GetSQLString(type) + ")" + SQLUtils.BLANK + options.GetSQLString(type) + ";");
		}
	}
}
