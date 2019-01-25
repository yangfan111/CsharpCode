using System;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.ui;

namespace com.wd.free.map.position
{
    [System.Serializable]
    public class MapCircleRegion : AbstractMapRegion
    {
        private const long serialVersionUID = 6163747739967853887L;

        private IPosSelector selector;

        private string radius;

        private bool change;

        private string zRange;

        [System.NonSerialized]
        private UnitPosition pos;

        public override bool IsIn(IEventArgs args, UnitPosition entity)
        {
            if (pos == null || change)
            {
                pos = selector.Select(args);
            }
            int r = 0;
            try
            {
                r = int.Parse(radius);
            }
            catch (Exception)
            {
                r = FreeUtil.ReplaceInt(radius, args);
            }
            double dx = MyMath.Abs(entity.GetX() - pos.GetX());
            if (dx > r)
            {
                if (useOut)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            double dz = MyMath.Abs(entity.GetZ() - pos.GetZ());
            if (dz > r)
            {
                if (useOut)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            double dy = MyMath.Abs(entity.GetY() - pos.GetY());
            int zrange = FreeUtil.ReplaceInt(zRange, args);
            if (zrange == 0)
            {
                zrange = 170;
            }

            bool isIn = (dx * dx + dz * dz) <= r * r && (dy < zrange);

            if (useOut)
            {
                return !isIn;
            }
            else
            {
                return isIn;
            }
        }

        public override bool InRectange(FreeUIUtil.Rectangle rec, IEventArgs args)
        {
            if (pos == null || change)
            {
                pos = selector.Select(args);
            }
            int px = (int)(pos.GetX());
            int py = (int)(pos.GetY());
            int r = 0;
            try
            {
                r = int.Parse(radius);
            }
            catch (Exception)
            {
                r = FreeUtil.ReplaceInt(radius, args);
            }
            int x1 = rec.x - r;
            int x2 = rec.x + rec.width + r;
            int y1 = rec.y - r;
            int y2 = rec.y + rec.height + r;
            // 圆心在矩形的加上圆半径的范围内
            return px >= x1 && px <= x2 && py >= y1 && py <= y2;
        }

        public override UnitPosition GetCenter(IEventArgs args)
        {
            if (pos == null || change)
            {
                pos = selector.Select(args);
            }
            return pos;
        }

        public virtual IPosSelector GetSelector()
        {
            return selector;
        }

        public virtual void SetSelector(IPosSelector selector)
        {
            this.selector = selector;
        }

        public virtual string GetRadius()
        {
            return radius;
        }

        public virtual void SetRadius(string radius)
        {
            this.radius = radius;
        }

        public virtual bool IsChange()
        {
            return change;
        }

        public virtual void SetChange(bool change)
        {
            this.change = change;
        }

        public override bool IsDynamic()
        {
            return change;
        }
    }
}
