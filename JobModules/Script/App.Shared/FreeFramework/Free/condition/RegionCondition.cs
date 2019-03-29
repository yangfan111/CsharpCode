using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.unit;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class RegionCondition : IParaCondition
    {
        private IMapRegion region;
        private IPosSelector pos;
        private bool useOut;

        public bool Meet(IEventArgs args)
        {
            UnitPosition up = pos.Select(args);

            if (useOut)
            {
                return !region.In(args, up);
            }
            else
            {
                return region.In(args, up);
            }
        }
    }
}
