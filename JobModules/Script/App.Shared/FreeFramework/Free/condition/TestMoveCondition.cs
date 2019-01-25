using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using UnityEngine;
using com.wd.free.unit;
using App.Shared.FreeFramework.framework.ai.move;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class DirectMoveCondition : IParaCondition
    {
        private IPosSelector from;
        private IPosSelector to;

        public bool Meet(IEventArgs args)
        {
            UnitPosition upf = from.Select(args);
            UnitPosition upt = to.Select(args);

            long s = DateTime.Now.Ticks;

            bool can = false;

            for (int i = 0; i < 1; i++)
            {
                can = AutoMoveUtil.CanDirectMoveTo(upf.Vector3, upt.Vector3);
            }

            //Debug.LogFormat("can time: {0}", DateTime.Now.Ticks - s);

            return can;
        }
    }
}
