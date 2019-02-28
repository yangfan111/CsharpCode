using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.para.exp;
using com.wd.free.unit;
using com.wd.free.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.condition
{
    [Serializable]
    public class DistanceCondition : IParaCondition
    {
        private IPosSelector source;
        private IPosSelector target;

        private String distance;
        private bool @out;

        public virtual bool Meet(IEventArgs args)
        {
            UnitPosition sourcePosition = source.Select(args);
            UnitPosition targetPosition = target.Select(args);
            if (sourcePosition != null && targetPosition != null)
            {
                double dis = sourcePosition.Distance(targetPosition);
                if (@out)
                {
                    return FreeUtil.ReplaceInt(distance, args) <= dis;
                }
                else
                {
                    return FreeUtil.ReplaceInt(distance, args) >= dis;
                }
            }
            return false;
        }
    }
}
