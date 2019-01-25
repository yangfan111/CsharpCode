using Sharpen;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class FormulaAction : AbstractGameAction
	{
		private const long serialVersionUID = -6466171136828397968L;

		private string para;

		private string formula;

		public override void DoAction(IEventArgs args)
		{
			UnitPara up = UnitPara.Parse(FreeUtil.ReplaceVar(this.para, args))[0];
			IPara para = up.GetPara(args);
			if (para != null)
			{
				double d = FreeUtil.ReplaceDouble(formula, args);
				if (para is IntPara)
				{
					para.SetValue("=", new IntPara(string.Empty, (int)d));
				}
				if (para is FloatPara)
				{
					para.SetValue("=", new FloatPara(string.Empty, (float)d));
				}
				if (para is DoublePara)
				{
					para.SetValue("=", new DoublePara(string.Empty, d));
				}
				else
				{
					IPara v = new DoublePara("v", d);
					para.SetValue("=", v);
				}
			}
		}
	}
}
