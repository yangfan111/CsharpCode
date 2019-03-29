using System;
using Sharpen;
using com.cpkf.yyjd.tools.condition;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
	public class ParaCondition : IClausable, ICondition<IEventArgs>
	{
		private ParaExp para;

		private string con;

		private ParaExp value;

		public ParaCondition()
			: base()
		{
		}

		public virtual bool Meet(IEventArgs args)
		{
			if (value.IsNull())
			{
				if (con.Equals("=="))
				{
					return !para.IsNotNull(args);
				}
				else
				{
					if (con.Equals("<>"))
					{
						return para.IsNotNull(args);
					}
				}
			}
			IPara source = para.GetSourcePara(args);
			if (source != null)
			{
				IPara target = value.GetTargetPara(args, source);
				try
				{
					bool r = source.Meet(con, target);
					target.Recycle();
					return r;
				}
				catch (Exception e)
				{
					throw new GameConfigExpception(para.ToString() + con + value.ToString() + " is not valid.\n" + ExceptionUtil.GetExceptionContent(e));
				}
			}
			return false;
		}

		public virtual ParaExp GetPara()
		{
			return para;
		}

		public virtual void SetPara(ParaExp para)
		{
			this.para = para;
		}

		public virtual string GetCon()
		{
			return con;
		}

		public virtual void SetCon(string con)
		{
			this.con = con;
		}

		public virtual ParaExp GetValue()
		{
			return value;
		}

		public virtual void SetValue(ParaExp value)
		{
			this.value = value;
		}

		public virtual ICondition<IEventArgs> Parse(string expression)
		{
			return null;
		}

		public override string ToString()
		{
			return para + con + value;
		}
	}
}
