using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using UnityEngine;

namespace com.wd.free.map.position
{
    [Serializable]
    public class PosDirectionSelector : AbstractPosSelector
    {
        private IPosSelector pos;

        private String distance;
        private String height;
        private String pitch;
        private String angle;

        public override UnitPosition Select(IEventArgs args)
        {

            UnitPosition up = pos.Select(args);

            if (up != null)
            {
                return GetPosition(args, up, FreeUtil.ReplaceFloat(angle, args));
            }

            return null;
        }

        private UnitPosition GetPosition(IEventArgs args, UnitPosition old,
            double angle)
        {
            Vector3 dir = new Vector3();
            Vector3 end = new Vector3();

            AnglesToVector(angle, FreeUtil.ReplaceFloat(pitch, args), ref dir);
            Vector3DMA(new Vector3(old.GetX(), old.GetY(), old.GetZ()), FreeUtil.ReplaceFloat(distance, args), dir, ref end);

            UnitPosition up = new UnitPosition();
            up.SetX(end.x);
            up.SetY(end.y + FreeUtil.ReplaceFloat(height, args));
            up.SetZ(end.z);
            up.SetYaw(old.GetYaw());

            return up;
        }

        public static double RAD = Math.PI / 180;

        public static void AnglesToVector(double yaw, double pitch, ref Vector3 v)
        {
            float xy = (float)Math.Cos(pitch * RAD);

            v.z = xy * (float)Math.Cos(yaw * RAD);
            v.x = -xy * (float)Math.Sin(yaw * RAD);
            v.y = (float)Math.Sin(pitch * RAD);
        }

        public static void Vector3DMA(Vector3 v, float s, Vector3 b, ref Vector3 o)
        {
            o.x = v.x + b.x * s;
            o.y = v.y + b.y * s;
            o.z = v.z + b.z * s;
        }
    }
}
