using App.Server.GameModules.GamePlay.Free.entity;
using com.wd.free.@event;
using com.wd.free.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    public class ChickenRuleVars
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

        // 航线位置
        private const string LineStartX = "{rStartX}";
        private const string LineStartY = "{rStartY}";
        private const string LineStopX = "{rStopX}";
        private const string LineStopY = "{rStopY}";

        // 场景中物件
        private const string DropPlane = "plane1";
        private const string Plane = "plane";

        private const string ServerTime = "{serverTime}";
        private const string StartWaitTime = "{startWaitTime}";

        public static int GetServerTime(IEventArgs args)
        {
            return FreeUtil.ReplaceInt("{serverTime}", args);
        }

        public static int GetStartWaitTime(IEventArgs args)
        {
            return FreeUtil.ReplaceInt("{startWaitTime}", args);
        }

        public static FreeEntityData GetDropPlane(IEventArgs args)
        {
            return (FreeEntityData)args.GetUnit(DropPlane);
        }

        public static FreeEntityData GetPlane(IEventArgs args)
        {
            return (FreeEntityData)args.GetUnit(Plane);
        }

        public static int GetFogStopX(IEventArgs args)
        {
            return FreeUtil.ReplaceInt(StopX, args);
        }

        public static int GetFogStopY(IEventArgs args)
        {
            return FreeUtil.ReplaceInt(StopY, args);
        }

        public static int GetFogStopRadius(IEventArgs args)
        {
            return FreeUtil.ReplaceInt(StopR, args);
        }

        public static Vector2 GetAirLineStartPos(IEventArgs args)
        {
            return new Vector2(FreeUtil.ReplaceInt(LineStartX, args), FreeUtil.ReplaceInt(LineStartY, args));
        }

        public static Vector2 GetAirLineStopPos(IEventArgs args)
        {
            return new Vector2(FreeUtil.ReplaceInt(LineStopX, args), FreeUtil.ReplaceInt(LineStopY, args));
        }
    }
}
