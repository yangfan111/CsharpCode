using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class PrimaryKey : ISQL
	{
		private const long serialVersionUID = 242242553394550932L;

		private FieldsSQL keys;

		public PrimaryKey()
		{
			this.keys = new FieldsSQL();
		}

		public PrimaryKey(string[] keys)
		{
			this.keys = new FieldsSQL();
			this.keys.AddFields(keys);
		}

		public virtual void AddKey(string key)
		{
			this.keys.AddField(key);
		}

		public virtual void AddKeys(string[] keys)
		{
			this.keys.AddFields(keys);
		}

		public virtual FieldsSQL GetKeys()
		{
			return keys;
		}

		public virtual void SetKeys(FieldsSQL keys)
		{
			this.keys = keys;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			return SQLUtils.BLANK + "PRIMARY KEY (" + keys.GetSQLString(type) + ")" + SQLUtils.BLANK;
		}
	}
}
