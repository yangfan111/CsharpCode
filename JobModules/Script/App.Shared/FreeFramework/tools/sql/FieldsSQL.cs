using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class FieldsSQL : ISQL
	{
		private const long serialVersionUID = -7334138548956597158L;

		private LinkedHashSet<string> fields;

		public FieldsSQL()
		{
			this.fields = new LinkedHashSet<string>();
		}

		public virtual void AddField(string field)
		{
			this.fields.AddItem(field);
		}

		public virtual void AddFields(string[] fields)
		{
			foreach (string field in fields)
			{
				this.fields.AddItem(field);
			}
		}

		public virtual void Clear()
		{
			this.fields.Clear();
		}

		public virtual LinkedHashSet<string> GetFields()
		{
			return this.fields;
		}

		public virtual int Size()
		{
			return fields.Count;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (fields.Count == 0)
			{
				return string.Empty;
			}
			else
			{
				return SQLUtils.BLANK + StringUtil.GetStringFromStrings(Sharpen.Collections.ToArray(fields, new string[] {  }), ", ") + SQLUtils.BLANK;
			}
		}
	}
}
