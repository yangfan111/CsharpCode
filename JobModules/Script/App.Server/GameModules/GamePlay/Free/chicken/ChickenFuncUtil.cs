using App.Shared.FreeFramework.framework.util;
using com.wd.free.@event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    public class ChickenFuncUtil
    {
        public static void UpdateBagCapacity(IEventArgs args, float weight, float capcacity)
        {
            FuntionUtil.Call(args, "updateBagCapacity", "weight", weight.ToString(), "capacity", capcacity.ToString());
        }
    }
}
