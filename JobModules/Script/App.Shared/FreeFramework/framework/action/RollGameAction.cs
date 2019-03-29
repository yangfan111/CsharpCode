using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class RollGameAction : AbstractGameAction
	{
		private const long serialVersionUID = 2178762156182717046L;

		private string percent;

		private IGameAction action;

		public override void DoAction(IEventArgs args)
		{
			int random = RandomUtil.Random(0, 100);
			int percent = FreeUtil.ReplaceInt(this.percent, args);
			if (random <= percent)
			{
				args.GetDefault().GetParameters().TempUse(new IntPara("roll", random));
				if (action != null)
				{
					action.Act(args);
				}
				args.GetDefault().GetParameters().Resume("roll");
			}
		}
	}
}
