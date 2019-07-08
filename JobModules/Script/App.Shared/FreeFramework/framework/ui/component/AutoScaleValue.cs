using gameplay.gamerule.free.ui.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace App.Shared.FreeFramework.framework.ui.component
{
    [Serializable]
    public class AutoScaleValue : AbstractAutoValue, IRule
    {
        public int GetRuleID()
        {
            return (int)ERuleIds.AutoScaleValue;
        }

        public override string ToConfig(IEventArgs arg1)
        {
            return "scale|" + FreeUtil.ReplaceInt(id, arg1);
        }
    }
}
