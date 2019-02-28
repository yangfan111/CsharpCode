using Sharpen;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.util;

namespace gameplay.gamerule.free.item
{
	public class ConditionHotKey
	{
		internal string condition;

		internal string key;

		internal string ui;

		[System.NonSerialized]
		private IParaCondition con;

		public virtual bool Meet(IEventArgs args)
		{
			if (con == null || (condition != null && condition.Contains(FreeUtil.VAR_START) && condition.Contains(FreeUtil.VAR_END)))
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
