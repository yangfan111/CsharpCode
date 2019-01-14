using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
	public class ParaExp
	{
		private UnitPara unitPara;

		private string op;

		public ParaExp(string exp)
		{
			this.unitPara = UnitPara.ParseOne(exp);
			this.op = "=";
		}

		public ParaExp(string exp, string op)
		{
			this.unitPara = UnitPara.ParseOne(exp);
			this.op = op;
		}

		public virtual string GetUnit()
		{
			return unitPara.GetUnit();
		}

		public virtual string GetPara()
		{
			return unitPara.GetPara();
		}

		public virtual bool IsNotNull(IEventArgs args)
		{
			return args.GetUnit(unitPara.GetPara()) != null || (args.GetUnit(unitPara.GetUnit()) != null && unitPara.GetPara(args) != null);
		}

		public virtual IPara GetSourcePara(IEventArgs args)
		{
			return unitPara.GetPara(args);
		}

		public virtual IPara GetTargetPara(IEventArgs args, IPara source)
		{
			IPara replace = ExpSetParser.GetReplaceValue(unitPara.GetPara(), args);
			if (replace != null)
			{
				return replace;
			}
			IParable p1 = args.GetUnit(unitPara.GetUnit());
			if (p1 != null)
			{
				IPara p = unitPara.GetPara(args);
				if (p == null)
				{
					p = source.Initial(op, unitPara.GetPara());
				}
				return p;
			}
			else
			{
				if (!StringUtil.IsNullOrEmpty(unitPara.GetUnit()))
				{
					return source.Initial(op, unitPara.GetUnit() + "." + unitPara.GetPara());
				}
				else
				{
					return source.Initial(op, unitPara.GetPara());
				}
			}
		}

		public virtual bool IsNull()
		{
			return (StringUtil.IsNullOrEmpty(unitPara.GetUnit()) || "default".Equals(unitPara.GetUnit())) && "null".Equals(unitPara.GetPara());
		}

		public override string ToString()
		{
			if (StringUtil.IsNullOrEmpty(unitPara.GetUnit()))
			{
				return unitPara.GetPara();
			}
			return unitPara.GetUnit() + "." + unitPara.GetPara();
		}
	}
}
