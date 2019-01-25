using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class ForAction : AbstractGameAction
	{
		private const long serialVersionUID = -5366033203749986601L;

		private string from;

		private string to;

		private IGameAction action;

		private string i;

		public override void DoAction(IEventArgs args)
		{
			int from = FreeUtil.ReplaceInt(this.from, args);
			int to = FreeUtil.ReplaceInt(this.to, args);
			if (StringUtil.IsNullOrEmpty(i))
			{
				i = "i";
			}
			if (from > to)
			{
				for (int i = from; i >= to; i--)
				{
					args.GetDefault().GetParameters().TempUse(new IntPara(this.i, i));
					action.Act(args);
					args.GetDefault().GetParameters().Resume(this.i);
				}
			}
			else
			{
				for (int i = from; i <= to; i++)
				{
					args.GetDefault().GetParameters().TempUse(new IntPara(this.i, i));
					action.Act(args);
					args.GetDefault().GetParameters().Resume(this.i);
				}
			}
		}
	}
}
