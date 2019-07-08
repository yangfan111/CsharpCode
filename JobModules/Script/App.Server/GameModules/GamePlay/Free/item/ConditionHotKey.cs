using Sharpen;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.item
{
	public class ConditionHotKey : IRule
	{
		internal string condition;

		internal string key;

		internal string ui;

		[System.NonSerialized]
		private IParaCondition con;

        public int GetRuleID()
        {
            return (int)ERuleIds.ConditionHotKey;
        }

        public virtual bool Meet(IEventArgs args)
		{
			if (con == null || (condition != null && (condition.IndexOf(FreeUtil.VAR_START_CHAR) > -1) && (condition.IndexOf(FreeUtil.VAR_END_CHAR) > -1)))
			{
				con = new ExpParaCondition(condition);
			}
			if (con != null)
			{
				return con.Meet(args);
			}
			return false;
		}
	}
}
