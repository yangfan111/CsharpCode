using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.condition
{
	public class ClauseParser<T> : IClausable
		where T : IClausable
	{
		public const string And = "&&";

		public const string Or = "||";

		public const string Append = "**";

		public const string Not = "!";

		public const string Left = "(";

		public const string Right = ")";

		private static HashSet<string> spliter;

		static ClauseParser()
		{
			spliter = new HashSet<string>();
			spliter.Add(And);
			spliter.Add(Not);
			spliter.Add(Or);
			spliter.Add(Append);
			spliter.Add(Left);
			spliter.Add(Right);
		}

		public ClauseParser()
		{
		}

		public virtual IClausable Parse(string exp, IExpParser<T> subParser, IExpClause clause)
		{
			string[] ss = StringUtil.SplitRemainRex(exp, Sharpen.Collections.ToArray(spliter, new string[] {  }));
			HashSet<int> used = new HashSet<int>();
			IList<StartEnd> startEnd = new List<StartEnd>();
			Dictionary<StartEnd, IClausable> map = new MyDictionary<StartEnd, IClausable>();
			while (true)
			{
				bool success = HandleParenthese(ss, used, startEnd, map, subParser, clause);
				if (!success)
				{
					break;
				}
			}
			string[] total = new StartEnd(-1, ss.Length).GetStrings(ss, Sharpen.Collections.ToArray(map.Keys, new StartEnd[] {  }));
			return ParseNoParenthese(total, map, subParser, clause);
		}

		private bool HandleParenthese(string[] ss, HashSet<int> used, IList<StartEnd> startEnd, Dictionary<StartEnd, IClausable> map, IExpParser<T> parser, IExpClause clause)
		{
			for (int i = 0; i < ss.Length; i++)
			{
				if (Right.Equals(ss[i]) && !used.Contains(i))
				{
					for (int j = i - 1; j >= 0; j--)
					{
						if (Right.Equals(ss[j]) && !used.Contains(j))
						{
							break;
						}
						if (Left.Equals(ss[j]) && !used.Contains(j))
						{
							used.Add(i);
							used.Add(j);
							StartEnd se = new StartEnd(j, i);
							IClausable con = ParseNoParenthese(se.GetStrings(ss, Sharpen.Collections.ToArray(startEnd, new StartEnd[] {  })), map, parser, clause);
							map[se] = con;
							startEnd.Add(se);
							foreach (StartEnd sub in se.GetChildren(Sharpen.Collections.ToArray(startEnd, new StartEnd[] {  })))
							{
								startEnd.Remove(sub);
								map.Remove(sub);
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		public virtual IClausable ParseNoParenthese(string[] ss, Dictionary<StartEnd, IClausable> map, IExpParser<T> parser, IExpClause clause)
		{
			clause = clause.Clone();
			IList<int> ids = new List<int>();
			for (int i = 0; i < ss.Length; i++)
			{
				if (IsLogic(ss[i]))
				{
					ids.Add(i);
				}
			}
			Check(ss, ids);
			HashSet<int> used = new HashSet<int>();
			HashSet<int> nots = new HashSet<int>();
			Sharpen.Collections.AddAll(used, ids);
			// handle one
			if (ss.Length == 1)
			{
				clause.AddAnd(GetCondition(ss[0], map, parser), false);
			}
			else
			{
				if (ss.Length == 2)
				{
					clause.AddAnd(GetCondition(ss[1], map, parser), true);
				}
			}
			// handle not
			for (int i_1 = 0; i_1 < ss.Length - 1; i_1++)
			{
				if (Not.Equals(ss[i_1]))
				{
					nots.Add(i_1 + 1);
				}
			}
			// handle and
			for (int i_2 = 1; i_2 < ss.Length - 1; i_2++)
			{
				if (And.Equals(ss[i_2]))
				{
					if (!used.Contains(i_2 - 1))
					{
						clause.AddAnd(GetCondition(ss[i_2 - 1], map, parser), nots.Contains(i_2 - 1));
						used.Add(i_2 - 1);
					}
					int back = i_2 + 1;
					if (Not.Equals(ss[back]))
					{
						back = i_2 + 2;
					}
					if (!used.Contains(back))
					{
						clause.AddAnd(GetCondition(ss[back], map, parser), nots.Contains(back));
						used.Add(back);
					}
				}
			}
			// handle or
			for (int i_3 = 1; i_3 < ss.Length - 1; i_3++)
			{
				if (Or.Equals(ss[i_3]))
				{
					if (!used.Contains(i_3 - 1))
					{
						clause.AddOr(GetCondition(ss[i_3 - 1], map, parser), nots.Contains(i_3 - 1));
						used.Add(i_3 - 1);
					}
					int back = i_3 + 1;
					if (Not.Equals(ss[back]))
					{
						back = i_3 + 2;
					}
					if (!used.Contains(back))
					{
						clause.AddOr(GetCondition(ss[back], map, parser), nots.Contains(back));
						used.Add(back);
					}
				}
			}
			// handle append
			for (int i_4 = 1; i_4 < ss.Length - 1; i_4++)
			{
				if (Append.Equals(ss[i_4]))
				{
					if (!used.Contains(i_4 - 1))
					{
						clause.AddApend(GetCondition(ss[i_4 - 1], map, parser), nots.Contains(i_4 - 1));
						used.Add(i_4 - 1);
					}
					int back = i_4 + 1;
					if (Not.Equals(ss[back]))
					{
						back = i_4 + 2;
					}
					if (!used.Contains(back))
					{
						clause.AddApend(GetCondition(ss[back], map, parser), nots.Contains(back));
						used.Add(back);
					}
				}
			}
			return clause;
		}

		private IClausable GetCondition(string exp, Dictionary<StartEnd, IClausable> map, IExpParser<T> subParser)
		{
			foreach (StartEnd se in map.Keys)
			{
				if (se.ToKey().Equals(exp))
				{
					return map[se];
				}
			}
			return subParser.Parse(exp);
		}

		private void Check(string[] ss, IList<int> ids)
		{
			// check expression
			for (int i = 0; i < ids.Count - 1; i++)
			{
				if (ids[i] + 1 == ids[i + 1])
				{
					if (!Not.Equals(ss[ids[i + 1]]))
					{
						throw new ConditionParseException(ss[ids[i]] + "和" + ss[ids[i + 1]] + "不能连续使用");
					}
					else
					{
						if (Not.Equals(ss[ids[i]]))
						{
							throw new ConditionParseException("不建议使用!!，等同于没有用");
						}
					}
				}
			}
			if (IsLogic(ss[ss.Length - 1]))
			{
				throw new ConditionParseException("最后的一个不能为逻辑符号'" + ss[ss.Length - 1] + "'");
			}
			if (And.Equals(ss[0]) || Or.Equals(ss[0]) || Append.Equals(ss[0]))
			{
				throw new ConditionParseException("第一个不能为逻辑符号'" + ss[0] + "'");
			}
		}

		private bool IsLogic(string s)
		{
			return Not.Equals(s) || And.Equals(s) || Or.Equals(s) || Append.Equals(s);
		}
	}
}
