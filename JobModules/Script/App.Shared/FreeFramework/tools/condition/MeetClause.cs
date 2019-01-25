using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	public class MeetClause<T, C> : ICondition<T>, IExpClause
		where C : IClausable
	{
		private IList<ICondition<T>> ands;

		private IList<ICondition<T>> ors;

		private IList<bool> andNegs;

		private IList<bool> orNegs;

		private IExpParser<C> subParser;

		public MeetClause(IExpParser<C> subParser)
		{
			this.subParser = subParser;
			this.ands = new List<ICondition<T>>();
			this.ors = new List<ICondition<T>>();
			this.andNegs = new List<bool>();
			this.orNegs = new List<bool>();
		}

		public virtual bool Meet(T t)
		{
			if (ands.Count > 0)
			{
				for (int i = 0; i < ands.Count; i++)
				{
					bool neg = andNegs[i];
					ICondition<T> con = ands[i];
					bool m = con.Meet(t);
					if ((m && !neg) || (!m && neg))
					{
						continue;
					}
					else
					{
						return false;
					}
				}
			}
			int count = 0;
			if (ors.Count > 0)
			{
				for (int j = 0; j < orNegs.Count; j++)
				{
					bool neg = orNegs[j];
					ICondition<T> con = ors[j];
					bool m = con.Meet(t);
					if ((m && !neg) || (!m && neg))
					{
						count++;
					}
				}
			}
			return count > 0 || ors.Count == 0;
		}

		public virtual ICondition<T> Parse(string expression)
		{
			return (com.cpkf.yyjd.tools.condition.MeetClause<T, C>)new ClauseParser<C>().Parse(expression, subParser, this);
		}

		public virtual void AddAnd(IClausable t, bool negative)
		{
			this.ands.Add((ICondition<T>)t);
			this.andNegs.Add(negative);
		}

		public virtual void AddOr(IClausable t, bool negative)
		{
			this.ors.Add((ICondition<T>)t);
			this.orNegs.Add(negative);
		}

		public virtual IExpClause Clone()
		{
			return new com.cpkf.yyjd.tools.condition.MeetClause<T, C>(this.subParser);
		}

		public virtual void AddApend(IClausable t, bool negative)
		{
		}
	}
}
