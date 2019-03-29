using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	public class SQLUtils
	{
		public const string BLANK = " ";

		public static string GetStandardSQLValue(string value)
		{
			if (value != null)
			{
				value = value.Replace("\\", "\\\\");
				value = value.Replace("'", "''");
			}
			return value;
		}

		public static string RemoveMultiBlank(string sql)
		{
			if (sql != null)
			{
				while (sql.Contains("  "))
				{
					sql = sql.Replace("  ", " ");
				}
			}
			return sql;
		}
	}
}
