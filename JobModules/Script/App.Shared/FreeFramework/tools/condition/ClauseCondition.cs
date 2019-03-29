using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	public class ClauseCondition<T> : ICondition<T>
	{
		private IList<ICondition<T>> ands;

		private IList<ICondition<T>> ors;

		private IList<ICondition<T>> nots;

		public virtual void AddAndClause(ICondition<T> condition)
		{
			this.ands.Add(condition);
		}

		public virtual void AddNotClause(ICondition<T> condition)
		{
			this.nots.Add(condition);
		}

		public virtual void AddOrClause(ICondition<T> condition)
		{
			this.ors.Add(condition);
		}

		public virtual bool Meet(T t)
		{
			if (ands != null)
			{
				foreach (ICondition<T> con in ands)
				{
					if (!con.Meet(t))
					{
						return false;
					}
				}
			}
			if (nots != null)
			{
				foreach (ICondition<T> con in nots)
				{
					if (con.Meet(t))
					{
						return false;
					}
				}
			}
			if (ors != null)
			{
				if (ors.Count > 0)
				{
					bool meet = false;
					foreach (ICondition<T> con in ors)
					{
						if (con.Meet(t))
						{
							meet = true;
							break;
						}
					}
					if (!meet)
					{
						return false;
					}
				}
			}
			return true;
		}

		public virtual ICondition<T> Parse(string expression)
		{
			// TODO Auto-generated method stub
			return null;
		}
	}
}
