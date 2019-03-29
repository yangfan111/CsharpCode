using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Sharpen;

namespace App.Server.GameModules.GamePlay.framework.action
{
    [Serializable]
    public class TimeFilterAction : AbstractGameAction
    {
        private string interval;

        // 第一次触发是否需要等待间隔时间
        private bool firstDelay;

        private IGameAction action;

        private long lastDoTime;

        public override void DoAction(IEventArgs args)
        {
            if (Runtime.CurrentTimeMillis() - lastDoTime > FreeUtil.ReplaceInt(interval,args))
            {
                
                if(firstDelay && lastDoTime == 0)
                {
                    lastDoTime = Runtime.CurrentTimeMillis();
                }
                else
                {
                    lastDoTime = Runtime.CurrentTimeMillis();
                    if (action != null)
                    {
                        action.Act(args);
                    }
                } 
            }
        }

        public override void Reset(IEventArgs args)
        {
            lastDoTime = 0;
        }
    }
}
