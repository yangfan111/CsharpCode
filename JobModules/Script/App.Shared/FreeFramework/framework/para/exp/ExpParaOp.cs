using Sharpen;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
	public class ExpParaOp
	{
		private ParaExp source;

		private ParaExp t1;

		private ParaExp t2;

		private string op;

		public static ExpParaOp FromExp(string exp)
		{
			ExpParaOp op = new ExpParaOp();
			return op;
		}

		public virtual void Op(IEventArgs args)
		{
			IPara s = source.GetSourcePara(args);
			IPara tt1 = t1.GetTargetPara(args, s);
			if (t2 == null)
			{
				s.SetValue("=", tt1);
			}
			else
			{
				IPara tt2 = t2.GetTargetPara(args, s);
				IPara temp = (IPara)s.Borrow();
				temp.SetValue("=", tt1);
				temp.SetValue(op, tt2);
				s.SetValue("=", temp);
				temp.Recycle();
				tt2.Recycle();
			}
			tt1.Recycle();
		}

		public virtual ParaExp GetSource()
		{
			return source;
		}

		public virtual void SetSource(ParaExp source)
		{
			this.source = source;
		}

		public virtual ParaExp GetT1()
		{
			return t1;
		}

		public virtual void SetT1(ParaExp t1)
		{
			this.t1 = t1;
		}

		public virtual ParaExp GetT2()
		{
			return t2;
		}

		public virtual void SetT2(ParaExp t2)
		{
			this.t2 = t2;
		}

		public virtual string GetOp()
		{
			return op;
		}

		public virtual void SetOp(string op)
		{
			this.op = op;
		}
	}
}
