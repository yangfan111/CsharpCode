using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.para;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    public class ComputeLineAction : AbstractGameAction
    {
        // 地图限制
        private const string MinX = "{minX}";
        private const string MaxX = "{maxX}";
        private const string MinY = "{minY}";
        private const string MaxY = "{maxY}";

        // 毒圈终止位置
        private const string StopX = "{stopX}";
        private const string StopY = "{stopZ}";
        private const string StopR = "{fogStopR}";

        // 航线起始位置
        private const string RStartX = "rStartX";
        private const string RStopX = "rStopX";
        private const string RStartY = "rStartY";
        private const string RStopY = "rStopY";

        public override void DoAction(IEventArgs args)
        {
            int minX = FreeUtil.ReplaceInt(MinX, args);
            int maxX = FreeUtil.ReplaceInt(MaxX, args);
            int minY = FreeUtil.ReplaceInt(MinY, args);
            int maxY = FreeUtil.ReplaceInt(MaxY, args);

            int stopX = FreeUtil.ReplaceInt(StopX, args);
            int stopY = FreeUtil.ReplaceInt(StopY, args);
            int stopR = FreeUtil.ReplaceInt(StopR, args);

            int random = RandomUtil.Random(1, 4);

            // 飞机航线
            if(stopR == 0)
            {
                FlyLine line = SelectStart(random, minX, minY, maxX, maxY);
                Set(args, (int)line.from.x, (int)line.from.y, (int)line.to.x, (int)line.to.y);
            }
            else
            {
                FlyLine line = SelectStart(random, minX, minY, maxX, maxY);
                Set(args, (int)line.from.x, (int)line.from.y, (int)line.to.x, (int)line.to.y);
            }
        }

        private FlyLine SelectStart(int random, int minX, int minY, int maxX, int maxY)
        {
            Vector2 from = new Vector2();
            Vector2 to = new Vector2();

            switch (random)
            {
                case 1:
                    from.x = minX;
                    from.y = RandomUtil.Random(minY, maxY);
                    to.x = maxX;
                    to.y = FindFarPosition(from.y, minY, maxY);
                    break;
                case 2:
                    from.x = maxX;
                    from.y = RandomUtil.Random(minY, maxY);
                    to.x = minX;
                    to.y = FindFarPosition(from.y, minY, maxY);
                    break;
                case 3:
                    from.x = RandomUtil.Random(minX, maxX);
                    from.y = minY;
                    to.y = maxY;
                    to.x = FindFarPosition(from.x, minX, maxX);
                    break;
                case 4:
                    from.x = RandomUtil.Random(minX, maxX);
                    from.y = maxY;
                    to.y = minY;
                    to.x = FindFarPosition(from.x, minX, maxX);
                    break;
                default:
                    break;
            }

            return new FlyLine(from, to);
        }

        private FlyLine FindCirclePosotion(bool useX, int minX, int minY, int maxX, int maxY, int stopX, int stopY)
        {
            FlyLine line = new FlyLine();
            Vector2 to = new Vector2(stopX, stopY);
            int angle = RandomUtil.Random(0, 360);
            if (angle == 90 || angle == 270)
            {
                angle = angle + RandomUtil.Random(5, 10);
            }
            double d = Math.Tan(angle * Math.PI / 180);
            Vector2 p1 = new Vector2(minX, GetY(to, d, minX));
            Vector2 p2 = new Vector2(maxX, GetY(to, d, maxX));
            Vector2 p3 = new Vector2(GetX(to, d, minY), minY);
            Vector2 p4 = new Vector2(GetX(to, d, maxY), maxY);

            SetToLine(line, p1, minY, maxY, false);
            SetToLine(line, p2, minY, maxY, false);
            SetToLine(line, p3, minX, maxX, true);
            SetToLine(line, p4, minX, maxX, true);

            return line;
        }

        private void SetToLine(FlyLine line, Vector2 p, int min, int max, bool useX)
        {
            if ((useX && p.x <= max && p.x >= min) || (!useX && p.y <= max && p.y >= min))
            {
                if (line.from == null)
                {
                    line.from = p;
                }
                else
                {
                    line.to = p;
                }
            }
        }

        /// <summary>
        /// 直线已知一点和斜率，用Y值求X值
        /// </summary>
        /// <param name="point"></param>
        /// <param name="to"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int GetX(Vector2 point, double v, int y)
        {
            return (int)((y - point.y) / v + point.x);
        }

        /// <summary>
        /// 直线已知一点和斜率，用X值求Y值
        /// </summary>
        /// <param name="point"></param>
        /// <param name="to"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private int GetY(Vector2 point, double v, int x)
        {
            return (int)(v * (x - point.x) + point.y);
        }

        private int FindFarPosition(float first, int min, int max)
        {
            if (first > (min + max) / 2)
            {
                return RandomUtil.Random(min, (min + max) / 2);
            }
            else
            {
                return RandomUtil.Random((min + max) / 2, max);
            }
        }

        private void Set(IEventArgs args, int startX, int startY, int endX, int endY)
        {
            SetOne(args, RStartX, startX);
            SetOne(args, RStartY, startY);
            SetOne(args, RStopX, endX);
            SetOne(args, RStopY, endY);
        }

        private void SetOne(IEventArgs args, string field, int value)
        {
            IntPara para = (IntPara)args.GetDefault().GetParameters().Get(field);
            if (para != null)
            {
                para.SetValue(value);
            }
        }

    }

    struct FlyLine : IComparable<FlyLine>
    {
        public Vector2 from;
        public Vector2 to;

        public FlyLine(Vector2 from, Vector2 to)
        {
            this.from = from;
            this.to = to;
        }

        public float Distance()
        {
            return (from.x - to.x) * (from.x - to.x) + (from.y - to.y) * (from.y - to.y);
        }

        public int CompareTo(FlyLine other)
        {
            return (int)(other.Distance() - this.Distance());
        }
    }

}
