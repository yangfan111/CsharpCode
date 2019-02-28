using com.wd.free.map.position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.unit;
using UnityEngine;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.map.position
{
    [Serializable]
    public class PosAngleSelector : AbstractPosSelector
    {
        private IPosSelector pos;
        private string radius;
        private string angle;

        private IPosSelector targetPos;

        public override UnitPosition Select(IEventArgs args)
        {
            UnitPosition up = pos.Select(args);
            UnitPosition targetUp = targetPos.Select(args);
            if (up != null && targetUp != null)
            {
                Vector3 dir = new Vector3();
                dir.x = targetUp.GetX() - up.GetX();
                dir.y = targetUp.GetY() - up.GetY();
                dir.z = targetUp.GetZ() - up.GetZ();

                dir = UnityPositionUtil.vectorToAngles(dir);

                double angle = FreeUtil.ReplaceDouble(this.angle, args);

                up = UnityPositionUtil.GetAnglePosition(up, angle + dir.x, 0, FreeUtil.ReplaceFloat(radius, args), 0);
            }

            return up;
        }
    }
}
