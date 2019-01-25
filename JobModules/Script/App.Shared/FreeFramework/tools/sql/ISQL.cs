using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public abstract class ISQL
	{
		public enum DBType
		{
			mysql,
			oracle
		}

		public abstract string GetSQLString(ISQL.DBType type);
	}

	public static class ISQLConstants
	{
	}
}
