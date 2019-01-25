using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
	public class ExpSetParser
	{
		private static IList<IExpReplacer> replacers;

		static ExpSetParser()
		{
			replacers = new List<IExpReplacer>();
		}

		public ExpSetParser()
		{
		}

		public static void RegisterReplacer(IExpReplacer replacer)
		{
			replacers.Add(replacer);
		}

		public static IPara GetReplaceValue(string exp, IEventArgs args)
		{
			foreach (IExpReplacer replacer in replacers)
			{
				if (replacer.CanHandle(exp, args))
				{
					return replacer.Replace(exp, args);
				}
			}
			return null;
		}

		public static ExpParaOp Parse(string exp, IEventArgs args)
		{
			string left = null;
			string right = null;
			int deep = 0;
			for (int i = 0; i < exp.Length; i++)
			{
				char c = exp[i];
				if (c == '=')
				{
					if (deep == 0)
					{
						left = Sharpen.Runtime.Substring(exp, 0, i).Trim();
						right = Sharpen.Runtime.Substring(exp, i + 1).Trim();
					}
				}
				else
				{
					if (c == '[')
					{
						deep++;
					}
					else
					{
						if (c == ']')
						{
							deep--;
						}
					}
				}
			}
			ExpParaOp pc = new ExpParaOp();
			if (left != null && right != null)
			{
				string s1 = left.Trim();
				string[] vs = StringUtil.Split(s1, ".");
				pc.SetSource(new ParaExp(s1, "="));
				string s2 = right.Trim();
				IPara para = pc.GetSource().GetSourcePara(args);
				if (para == null)
				{
					throw new GameConfigExpception(s1 + " is not a valid source para");
				}
				s2 = s2.Replace("='", "listatrricondition");
				s2 = s2.Replace("/@", "listatrriget");
				vs = StringUtil.SplitRemainRex(s2, para.GetOps());
				for (int i_1 = 0; i_1 < vs.Length; i_1++)
				{
					vs[i_1] = vs[i_1].Replace("listatrricondition", "='");
					vs[i_1] = vs[i_1].Replace("listatrriget", "/@");
				}
				if (vs.Length == 1)
				{
					pc.SetT1(new ParaExp(vs[0].Trim(), "="));
					pc.SetOp("=");
				}
				else
				{
					if (vs.Length == 3)
					{
						string op = vs[1].Trim();
						pc.SetT1(new ParaExp(vs[0].Trim(), "="));
						pc.SetT2(new ParaExp(vs[2].Trim(), op));
						pc.SetOp(op);
					}
					else
					{
						if (vs.Length == 2)
						{
							if (vs[0].Trim().Equals("-"))
							{
								pc.SetT1(new ParaExp(vs[1], "="));
								pc.SetOp("=");
							}
						}
						else
						{
							throw new GameConfigExpception(s2 + " is not a valid assign method");
						}
					}
				}
			}
			else
			{
				throw new GameConfigExpception(exp + " is not valid.");
			}
			return pc;
		}
	}
}
