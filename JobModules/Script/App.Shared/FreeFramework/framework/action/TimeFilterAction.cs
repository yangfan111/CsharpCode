using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;
using Sharpen;
using System;

namespace App.Server.GameModules.GamePlay.framework.action
{
    [Serializable]
    public class TimeFilterAction : AbstractGameAction, IRule
    {
        private string interval;

        // 第一次触发是否需要等待间隔时间
        private bool firstDelay;

        private IGameAction action;

        private long lastDoTime;

        public override void DoAction(IEventArgs args)
        {
            long now = Runtime.CurrentTimeMillis();
            if (now - lastDoTime > (long) FreeUtil.ReplaceInt(interval, args))
            {
                if (!firstDelay || lastDoTime != 0L)
                {
                    if (action != null)
                    {
                        action.Act(args);
                    }
                }
                lastDoTime = now;
            }
        }

        public override void Reset(IEventArgs args)
        {
            lastDoTime = 0;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.TimeFilterAction;
        }
    }
}
