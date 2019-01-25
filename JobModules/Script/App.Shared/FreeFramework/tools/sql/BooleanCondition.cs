using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class BooleanCondition : ISQL
	{
		private const long serialVersionUID = 5167210794489313487L;

		private List<ISQL> andList;

		private List<ISQL> orList;

		public enum BooleanClause
		{
			Must,
			Should
		}

		public BooleanCondition()
		{
			this.andList = new List<ISQL>();
			this.orList = new List<ISQL>();
		}

		public virtual void AddCondition(ISQL sql, BooleanCondition.BooleanClause clause)
		{
			switch (clause)
			{
				case BooleanCondition.BooleanClause.Should:
				{
					orList.Add(sql);
					break;
				}

				case BooleanCondition.BooleanClause.Must:
				{
					andList.Add(sql);
					break;
				}

				default:
				{
					andList.Add(sql);
					break;
				}
			}
		}

		public virtual List<ISQL> GetAndList()
		{
			return andList;
		}

		public virtual List<ISQL> GetOrList()
		{
			return orList;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (orList.Count > 0 || andList.Count > 0)
			{
				string sb = string.Empty;
				if (andList.Count > 0)
				{
					FieldValuesSQL fv = new FieldValuesSQL(" and ");
					foreach (ISQL sql in andList)
					{
						fv.AddSQLCondition(sql);
					}
					sb = sb + fv.GetSQLString(type);
				}
				if (orList.Count > 0)
				{
					FieldValuesSQL fv = new FieldValuesSQL(" or ");
					foreach (ISQL sql in orList)
					{
						fv.AddSQLCondition(sql);
					}
					if (andList.Count > 0)
					{
						sb = sb + " and (" + fv.GetSQLString(type) + ")";
					}
					else
					{
						sb = fv.GetSQLString(type);
					}
				}
				return "(" + sb + ")";
			}
			return string.Empty;
		}
	}
}
