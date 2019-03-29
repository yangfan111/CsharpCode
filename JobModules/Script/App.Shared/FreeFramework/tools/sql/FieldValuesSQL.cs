using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class FieldValuesSQL : ISQL
	{
		private const long serialVersionUID = 4890847426921847082L;

		private LinkedHashSet<ISQL> fieldMap;

		private string spliter = ", ";

		public FieldValuesSQL()
		{
			this.fieldMap = new LinkedHashSet<ISQL>();
		}

		public FieldValuesSQL(string spliter)
		{
			this.fieldMap = new LinkedHashSet<ISQL>();
			this.spliter = spliter;
		}

		public void AddFieldValue(string field, string value)
		{
			this.fieldMap.AddItem(new SQLCondition(field, value));
		}

		public void AddSQLCondition(ISQL condition)
		{
			this.fieldMap.AddItem(condition);
		}

		public virtual LinkedHashSet<ISQL> GetFieldMap()
		{
			return fieldMap;
		}

		public virtual int Size()
		{
			return fieldMap.Count;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (fieldMap.Count == 0)
			{
				return string.Empty;
			}
			else
			{
				IList<string> list = new List<string>();
				foreach (ISQL con in fieldMap)
				{
					string sql = con.GetSQLString(type);
					if (!StringUtil.IsNullOrEmpty(sql))
					{
						list.Add(con.GetSQLString(type));
					}
				}
				return SQLUtils.BLANK + StringUtil.GetStringFromStrings(Sharpen.Collections.ToArray(list, new string[] {  }), spliter) + SQLUtils.BLANK;
			}
		}
	}
}
