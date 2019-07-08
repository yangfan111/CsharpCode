using System;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.util;
using Core.Free;

namespace com.wd.free.action.stage
{
    [Serializable]
	public class OneTimeStageAction : IRule
	{
		public string time;

	    public string desc;

		public IGameAction action;

	    public IGameAction frameAction;

		[System.NonSerialized]
		private int realTime;

		public virtual int GetRealTime(IEventArgs args)
		{
			if (realTime == 0)
			{
				realTime = FreeUtil.ReplaceInt(time, args);
			}
			return realTime;
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.OneTimeStageAction;
        }
    }
}
