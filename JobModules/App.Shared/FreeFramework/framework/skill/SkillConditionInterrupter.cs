using Sharpen;
using com.wd.free.para.exp;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillConditionInterrupter : ISkillInterrupter
	{
		private const long serialVersionUID = 4268647993385830385L;

		public IParaCondition condition;

		public virtual bool IsInterrupted(ISkillArgs args)
		{
			return condition != null && condition.Meet(args);
		}
	}
}
