using System;
using Sharpen;
using com.cpkf.yyjd.tools.condition;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.para.exp
{
	[System.Serializable]
	public class ExpParaCondition : IParaCondition
	{
		private const long serialVersionUID = -3958190970967367085L;

		[System.NonSerialized]
		private MeetClause<IEventArgs, ParaCondition> clause;

		private string exp;

		public ExpParaCondition()
			: base()
		{
		}

		public ExpParaCondition(string exp)
		{
			SetExp(exp);
		}

		public virtual string GetExp()
		{
			return exp;
		}

		public virtual void SetExp(string exp)
		{
			this.exp = exp;
		}

		private void Initial(IEventArgs args)
		{
			string realExp = FreeUtil.ReplaceVar(exp, args);
			if (StringUtil.IsNullOrEmpty(realExp))
			{
				return;
			}
			// 如果有变量每次重新生成
			if (clause == null || (exp.Contains(FreeUtil.VAR_START) && exp.Contains(FreeUtil.VAR_END)))
			{
				MeetClause<IEventArgs, ParaCondition> temp = new MeetClause<IEventArgs, ParaCondition>(new ExpConditionParser());
				clause = (MeetClause<IEventArgs, ParaCondition>)temp.Parse(realExp);
			}
		}

		private sealed class _IParable_55 : IParable
		{
			public _IParable_55()
			{
			}

			public ParaList GetParameters()
			{
				ParaList pl = new ParaList();
				pl.AddPara(new IntPara("a", 1));
				pl.AddPara(new IntPara("b", 1));
				return pl;
			}
		}

		public virtual bool Meet(IEventArgs args)
		{
			Initial(args);
			if (clause == null || StringUtil.IsNullOrEmpty(FreeUtil.ReplaceVar(exp, args)))
			{
				return false;
			}
			bool m = false;
			try
			{
				m = clause.Meet(args);
			}
			catch (Exception e)
			{
				throw new GameConfigExpception(exp + "\n" + ExceptionUtil.GetExceptionContent(e));
			}
			return m;
		}

		public override string ToString()
		{
			return this.GetType().Name + "-> " + exp;
		}
	}
}
