using System.Collections.Generic;
using System.Text;
using Sharpen;
using com.cpkf.yyjd.tools.data;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class InsertSQL : ISQL
	{
		private const long serialVersionUID = -7951460546071669844L;

		private bool ignore;

		private bool @override;

		private SQLTable tables;

		private FieldValuesSQL fieldValues;

		public InsertSQL()
		{
			this.tables = new SQLTable();
			this.fieldValues = new FieldValuesSQL();
			this.ignore = true;
		}

		public virtual void AddTable(string table)
		{
			this.tables.AddTable(table);
		}

		public virtual void AddFieldValue(string field, string value)
		{
			this.fieldValues.AddFieldValue(field, value);
		}

		public virtual void AddFieldValue(DataRecord dataRecord)
		{
			foreach (string field in dataRecord.GetAllFields())
			{
				this.fieldValues.AddFieldValue(field, dataRecord.GetFieldValue(field));
			}
		}

		public virtual bool IsIgnore()
		{
			return ignore;
		}

		public virtual void SetIgnore(bool ignore)
		{
			this.ignore = ignore;
		}

		public virtual bool IsOverride()
		{
			return @override;
		}

		public virtual void SetOverride(bool @override)
		{
			this.@override = @override;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			StringBuilder sb = new StringBuilder();
			if (type == ISQL.DBType.mysql)
			{
				if (ignore)
				{
					sb.Append("insert ");
					sb.Append(" ignore ");
				}
				else
				{
					sb.Append("replace");
				}
			}
			else
			{
				sb.Append("insert ");
			}
			sb.Append(" into ");
			sb.Append(tables.GetSQLString(type));
			if (type == ISQL.DBType.mysql)
			{
				sb.Append(" set ");
				sb.Append(fieldValues.GetSQLString(type));
			}
			else
			{
				ISQL[] fields = Sharpen.Collections.ToArray(fieldValues.GetFieldMap(), new ISQL[0]);
				if (fields.Length > 0)
				{
					IList<string> fList = new List<string>();
					IList<string> vList = new List<string>();
					foreach (ISQL f in fields)
					{
						SQLCondition sc = (SQLCondition)f;
						fList.Add("\"" + sc.GetField() + "\"");
						vList.Add(" '" + SQLUtils.GetStandardSQLValue(sc.GetValue()) + "' ");
					}
					sb.Append(" (" + StringUtil.GetStringFromStrings(fList, ",") + ") values(" + StringUtil.GetStringFromStrings(vList, ",") + ") ");
				}
			}
			return SQLUtils.RemoveMultiBlank(sb.ToString().Trim());
		}
	}
}
