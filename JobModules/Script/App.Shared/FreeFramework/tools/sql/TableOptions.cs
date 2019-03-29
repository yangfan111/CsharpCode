using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class TableOptions : ISQL
	{
		private const long serialVersionUID = 647157794159301082L;

		public const string INNODB = "InnoDB";

		public const string MYISAM = "MyISAM";

		public const string UTF8 = "utf8";

		private string engine;

		private string charset;

		public TableOptions()
		{
			this.engine = INNODB;
			this.charset = UTF8;
		}

		public TableOptions(string engine, string charset)
			: base()
		{
			this.engine = engine;
			this.charset = charset;
		}

		public virtual string GetEngine()
		{
			return engine;
		}

		public virtual void SetEngine(string engine)
		{
			this.engine = engine;
		}

		public virtual string GetCharset()
		{
			return charset;
		}

		public virtual void SetCharset(string charset)
		{
			this.charset = charset;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			return SQLUtils.BLANK + "ENGINE=" + engine + SQLUtils.BLANK + "DEFAULT" + SQLUtils.BLANK + "CHARSET=" + charset + SQLUtils.BLANK;
		}
	}
}
