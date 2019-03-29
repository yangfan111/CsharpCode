using gameplay.gamerule.free.ui.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.ui
{
    [Serializable]
    class SimpleBagAuto : AbstractAutoValue
    {
        private int radius;

        public override string ToConfig(IEventArgs arg1)
        {
            return "bagauto|" + radius;
        }
    }
}
