using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeNumberComponet : AbstractFreeComponent
	{
		private const long serialVersionUID = -7699020163820046853L;

		private string number;

		private string font;

		private int length;

		private string align;

		private bool showZero;

		private string scale;

		public override int GetKey(IEventArgs args)
		{
			return NUMBER;
		}

		public override string GetStyle(IEventArgs args)
		{
			string[] ss = StringUtil.Split(font, "_");
			string s = scale;
			if (StringUtil.IsNullOrEmpty(s))
			{
				s = "1";
			}
			return FreeUtil.ReplaceNumber(number, args) + "_" + ss[0] + "_" + ss[1] + "_" + ss[2] + "_" + length + "_" + align + "_" + showZero + "_" + s;
		}

		public override IFreeUIValue GetValue()
		{
			return new FreeUINumberValue();
		}

		public override SimpleProto CreateChildren(IEventArgs args)
		{
		    SimpleProto b = FreePool.Allocate();
			b.Key = 1;
			return b;
		}

		public virtual string GetNumber()
		{
			return number;
		}

		public virtual void SetNumber(string number)
		{
			this.number = number;
		}
	}
}
