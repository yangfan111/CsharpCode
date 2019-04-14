using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.ui;
using Sharpen;
using System;
using Shared.Scripts.MapConfigPoint;

namespace com.wd.free.map.position
{
    [Serializable]
    public class MapPointRegion : AbstractMapRegion
    {
        public string type;

        public override UnitPosition GetCenter(IEventArgs arg1)
        {
            foreach (MapConfigPoints.ID_Point p in MapConfigPoints.current.IDPints)
            {
                if (p.ID == FreeUtil.ReplaceInt(type, arg1))
                {
                    UnitPosition unitPosition = new UnitPosition();
                    if (p.points.Count > 0)
                    {
                        unitPosition.SetX(p.points[0].pos.x);
                        unitPosition.SetY(p.points[0].pos.y);
                        unitPosition.SetZ(p.points[0].pos.z);
                        unitPosition.SetYaw(p.points[0].dir);
                        unitPosition.SetCylinderVolR(p.points[0].cylinderVolR);
                        unitPosition.SetCylinderVolH(p.points[0].cylinderVolH);
                        return unitPosition;
                    }
                }
            }
            return null;
        }

        public override bool InRectange(FreeUIUtil.Rectangle arg1, IEventArgs arg2)
        {
            return false;
        }

        public override bool IsDynamic()
        {
            return false;
        }

        public override bool IsIn(IEventArgs args, UnitPosition entity)
        {
            bool isIn = false;
            foreach (MapConfigPoints.ID_Point p in MapConfigPoints.current.IDPints)
            {
                if (p.ID == FreeUtil.ReplaceInt(type, args))
                {
                    foreach (MapConfigPoints.SavedPointData spd in p.points)
                    {
                        float dx = MyMath.Abs(entity.GetX() - spd.pos.x);
                        float dz = MyMath.Abs(entity.GetZ() - spd.pos.z);
                        float dy = MyMath.Abs(entity.GetY() - spd.pos.y);
                        if (dx * dx + dz * dz <= spd.cylinderVolR * spd.cylinderVolR && dy <= spd.cylinderVolH)
                        {
                            isIn = true;
                            break;
                        }
                    }

                    if (isIn)
                    {
                        break;
                    }
                }
            }

            if (useOut)
            {
                return !isIn;
            }

            return isIn;
        }
    }
}
