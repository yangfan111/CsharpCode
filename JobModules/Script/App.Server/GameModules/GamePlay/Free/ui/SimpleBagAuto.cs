using gameplay.gamerule.free.ui.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.ui
{
    [Serializable]
    class SimpleBagAuto : AbstractAutoValue, IRule
    {
        private int radius;

        public int GetRuleID()
        {
            return (int)ERuleIds.SimpleBagAuto;
        }

        public override string ToConfig(IEventArgs arg1)
        {
            return "bagauto|" + radius;
        }
    }
}
