using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.condition;
using com.cpkf.yyjd.tools.util;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
	public class ExpConditionParser : IExpParser<ParaCondition>
	{
		private static string[] OPS;

		static ExpConditionParser()
		{
			OPS = new string[] { "==", "<", ">", "!=", ">=", "<=" };
			Register(new IntPara());
		}

		public static void Register(IPara para)
		{
			ICollection<string> ops = new HashSet<string>();
			Sharpen.Collections.AddAll(ops, Arrays.AsList(OPS));
			Sharpen.Collections.AddAll(ops, Arrays.AsList(para.GetConditions()));
			OPS = Sharpen.Collections.ToArray(ops, new string[0]);
		}

		public ExpConditionParser()
		{
		}

		public virtual ParaCondition Parse(string exp)
		{
			string[] ss = StringUtil.SplitRemainRex(exp, OPS);
			ParaCondition pc = new ParaCondition();
			if (ss.Length == 3)
			{
				string s = ss[0].Trim();
				string op = ss[1].Trim();
				pc.SetPara(new ParaExp(s, op));
				pc.SetCon(ss[1].Trim());
				s = ss[2].Trim();
				pc.SetValue(new ParaExp(s, op));
			}
			else
			{
				throw new GameConfigExpception(exp + " is not defined.");
			}
			return pc;
		}
	}
}
