using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.para.exp;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillExpTrigger : AbstractSkillTrigger
	{
		private const long serialVersionUID = 8042541448273731065L;

		private string exp;

		[System.NonSerialized]
		private ExpParaCondition conditon;

		private IGameAction action;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			if (conditon == null && !StringUtil.IsNullOrEmpty(exp))
			{
				conditon = new ExpParaCondition(exp);
			}
			if (action != null)
			{
				action.Act(args);
			}
			if (conditon == null || conditon.Meet(args))
			{
				return IsInter(args);
			}
			else
			{
				return ISkillTrigger.TriggerStatus.Failed;
			}
		}
	}
}
