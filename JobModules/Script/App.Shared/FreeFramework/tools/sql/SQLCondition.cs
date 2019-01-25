using Sharpen;

namespace com.cpkf.yyjd.tools.sql
{
	[System.Serializable]
	public class SQLCondition : ISQL
	{
		private const long serialVersionUID = -7914377517128087257L;

		private string field;

		private string value;

		private bool negative = false;

		private string condition = "=";

		private bool number;

		public SQLCondition()
		{
		}

		public virtual bool IsNegative()
		{
			return negative;
		}

		public virtual void SetNegative(bool negative)
		{
			this.negative = negative;
		}

		public virtual bool IsNumber()
		{
			return number;
		}

		public virtual void SetNumber(bool number)
		{
			this.number = number;
		}

		public SQLCondition(string field, string value)
		{
			this.field = field;
			this.value = value;
		}

		public SQLCondition(string field, string value, string condition, bool number)
		{
			this.field = field;
			this.value = value;
			this.condition = condition;
			this.number = number;
		}

		public SQLCondition(string field, string value, string condition)
		{
			this.field = field;
			this.value = value;
			this.condition = condition;
			this.number = false;
		}

		public virtual string GetField()
		{
			return field;
		}

		public virtual void SetField(string field)
		{
			this.field = field;
		}

		public virtual string GetValue()
		{
			return value;
		}

		public virtual void SetValue(string value)
		{
			this.value = value;
		}

		public virtual string GetCondition()
		{
			return condition;
		}

		public virtual void SetCondition(string condition)
		{
			this.condition = condition;
		}

		public override string GetSQLString(ISQL.DBType type)
		{
			if (value != null && field != null)
			{
				string v = " '" + SQLUtils.GetStandardSQLValue(value) + "' ";
				if (number)
				{
					v = " " + SQLUtils.GetStandardSQLValue(value) + " ";
				}
				if (type == ISQL.DBType.mysql)
				{
					if (negative)
					{
						return "!(`" + field + "`" + condition + v + ")";
					}
					else
					{
						return "`" + field + "`" + condition + v;
					}
				}
				else
				{
					if (negative)
					{
						return "!(" + field + string.Empty + condition + v + ")";
					}
					else
					{
						return string.Empty + field + string.Empty + condition + v;
					}
				}
			}
			return string.Empty;
		}

		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + ((condition == null) ? 0 : condition.GetHashCode());
			result = prime * result + ((field == null) ? 0 : field.GetHashCode());
			result = prime * result + ((value == null) ? 0 : value.GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			com.cpkf.yyjd.tools.sql.SQLCondition other = (com.cpkf.yyjd.tools.sql.SQLCondition)obj;
			if (condition == null)
			{
				if (other.condition != null)
				{
					return false;
				}
			}
			else
			{
				if (!condition.Equals(other.condition))
				{
					return false;
				}
			}
			if (field == null)
			{
				if (other.field != null)
				{
					return false;
				}
			}
			else
			{
				if (!field.Equals(other.field))
				{
					return false;
				}
			}
			if (value == null)
			{
				if (other.value != null)
				{
					return false;
				}
			}
			else
			{
				if (!value.Equals(other.value))
				{
					return false;
				}
			}
			return true;
		}
	}
}
