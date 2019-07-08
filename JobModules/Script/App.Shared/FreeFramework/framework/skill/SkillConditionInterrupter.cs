using Sharpen;
using com.wd.free.para.exp;
using Core.Free;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillConditionInterrupter : ISkillInterrupter, IRule
    {
		private const long serialVersionUID = 4268647993385830385L;

		public IParaCondition condition;

        public int GetRuleID()
        {
            return (int)ERuleIds.SkillConditionInterrupter;
        }

        public virtual bool IsInterrupted(ISkillArgs args)
		{
			return condition != null && condition.Meet(args);
		}
	}
}
