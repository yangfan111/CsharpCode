using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class SelectSQL : ISQL
	{
		private const long serialVersionUID = 1005831042004398319L;

		private FieldsSQL fields;

		private SQLTable tables;

		private WhereSQL where;

		private OrderBySQL orderBy;

		private GroupBySQL groupBy;

		private LimitSQL limit;

		private string sql;

		public SelectSQL()
		{
			this.tables = new SQLTable();
			this.fields = new FieldsSQL();
			this.where = new WhereSQL();
			this.orderBy = new OrderBySQL();
			this.groupBy = new GroupBySQL();
			this.limit = new LimitSQL();
		}

		public SelectSQL(string table)
			: this()
		{
			AddTable(table);
		}

		public SelectSQL(string table, string[] fields)
			: this()
		{
			AddTable(table);
			AddFields(fields);
		}

		public virtual string GetSql()
		{
			return sql;
		}

		public virtual void SetSql(string sql)
		{
			this.sql = sql;
		}

		public virtual void AddField(string field)
		{
			this.fields.AddField(field);
		}

		public virtual void AddFields(string[] fields)
		{
			this.fields.AddFields(fields);
		}

		public virtual void AddTable(string table)
		{
			this.tables.AddTable(table);
		}

		public virtual void AddWhereCondition(string field, string value)
		{
			this.where.AddCondition(field, value);
		}

		public virtual void AddWhereCondition(SQLCondition condition)
		{
			this.where.AddCondition(condition);
		}

		public virtual void AddWhereCondition(ISQL condition)
		{
			this.where.AddCondition(condition);
		}

		public virtual void AddBooleanCondition(BooleanCondition condition)
		{
			this.where.AddCondition(condition);
		}

		public virtual void AddGroupByField(string field)
		{
			this.groupBy.AddField(field);
		}

		public virtual void AddGroupByFields(string[] fields)
		{
			foreach (string field in fields)
			{
				this.groupBy.AddField(field);
			}
		}

		public virtual FieldsSQL GetFields()
		{
			return fields;
		}

		public virtual void SetFields(FieldsSQL fields)
		{
			this.fields = fields;
		}

		public virtual SQLTable GetTables()
		{
			return tables;
		}

		public virtual void SetTables(SQLTable tables)
		{
			this.tables = tables;
		}

		public virtual WhereSQL GetWhere()
		{
			return where;
		}

		public virtual void SetWhere(WhereSQL where)
		{
			this.where = where;
		}

		public virtual OrderBySQL GetOrderBy()
		{
			return orderBy;
		}

		public virtual void SetOrderBy(OrderBySQL orderBy)
		{
			this.orderBy = orderBy;
		}

		public virtual GroupBySQL GetGroupBy()
		{
			return groupBy;
		}

		public virtual void SetGroupBy(GroupBySQL groupBy)
		{
			this.groupBy = groupBy;
		}

		public virtual LimitSQL GetLimit()
		{
			return limit;
		}

		public virtual void SetLimit(LimitSQL limit)
		{
			this.limit = limit;
		}

		public virtual void AddOrderByField(string field, bool desc)
		{
			if (!desc)
			{
				this.orderBy.AddField(field);
			}
			else
			{
				this.orderBy.AddDescField(field);
			}
		}

		public virtual void SetLimitStart(int start)
		{
			this.limit.SetStart(start);
		}

		public virtual void SetLimitCount(int count)
		{
			this.limit.SetCount(count);
		}

		public virtual com.cpkf.yyjd.tools.sql.SelectSQL Clone()
		{
			return (com.cpkf.yyjd.tools.sql.SelectSQL)SerializeUtil.ByteToObject(SerializeUtil.ObjectToByte(this));
		}

		public virtual string GetSQLString()
		{
			return GetSQLString(ISQL.DBType.mysql);
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (!StringUtil.IsNullOrEmpty(this.sql))
			{
				return this.sql;
			}
			string field = fields.Size() == 0 ? "*" : fields.GetSQLString(type);
			string sql = string.Empty;
			if (type == ISQL.DBType.mysql || limit == null || (limit.GetCount() == 0 && limit.GetStart() == 0))
			{
				sql = "select " + field + " from " + tables.GetSQLString(type) + where.GetSQLString(type) + groupBy.GetSQLString(type) + orderBy.GetSQLString(type) + limit.GetSQLString(type);
			}
			else
			{
				WhereSQL w1 = where.Clone();
				w1.AddCondition(new SQLCondition("rownum", (limit.GetStart() + limit.GetCount() + 1).ToString(), "<", true));
				WhereSQL w2 = where.Clone();
				w2.AddCondition(new SQLCondition("rownum", (limit.GetStart() + 1).ToString(), "<", true));
				sql = "SELECT * FROM ( SELECT A.*, ROWNUM RNNNNNN FROM (SELECT " + field + " FROM " + tables.GetSQLString(type) + where.GetSQLString(type) + groupBy.GetSQLString(type) + orderBy.GetSQLString(type) + ") A WHERE ROWNUM <= " + (limit.GetStart()
					 + limit.GetCount()) + " ) WHERE RNNNNNN >= " + (limit.GetStart() + 1);
			}
			return SQLUtils.RemoveMultiBlank(sql.Trim()).Trim();
		}

		public override string ToString()
		{
			return GetSQLString(ISQL.DBType.mysql);
		}
	}
}
