using Sharpen;
using com.wd.free.@event;
using com.wd.free.para.exp;
using Core.Free;

namespace gameplay.gamerule.free.condition
{
	[System.Serializable]
	public class AndCondition : IParaCondition, IRule
    {
		private const long serialVersionUID = -5968357087453563990L;

		private IParaCondition condition1;

		private IParaCondition condition2;

        public int GetRuleID()
        {
            return (int)ERuleIds.AndCondition;
        }

        public virtual bool Meet(IEventArgs args)
		{
			bool flag = false;
			if (condition1 != null)
			{
				flag = condition1.Meet(args);
			}
			if (flag == false)
			{
				return false;
			}
			bool flag2 = false;
			if (condition2 != null)
			{
				flag2 = condition2.Meet(args);
			}
			return flag && flag2;
		}
	}
}
