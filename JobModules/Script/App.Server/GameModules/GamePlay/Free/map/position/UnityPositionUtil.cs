using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.unit;
using Core.Components;
using UnityEngine;
using com.wd.free.@event;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.map.position
{
    public class UnityPositionUtil
    {
        public static void SetPlayerToUnitPosition(PlayerEntity player, UnitPosition up)
        {
            up.SetX(player.position.Value.x);
            up.SetY(player.position.Value.y);
            up.SetZ(player.position.Value.z);
            up.SetYaw(player.orientation.Yaw);
            up.SetPitch(player.orientation.Pitch);
        }

        public static Vector3 ToVector3(UnitPosition up)
        {
            return new Vector3(up.GetX(), up.GetY(), up.GetZ());
        }

        public static void SetEntityToUnitPosition(PositionComponent position, UnitPosition up)
        {
            up.SetX(position.Value.x);
            up.SetY(position.Value.y);
            up.SetZ(position.Value.z);
        }

        public static void SetUnitPositionToPlayer(PlayerEntity player, UnitPosition up)
        {
            player.position.Value = new Vector3(up.GetX(), up.GetY(), up.GetZ());
            player.orientation.Yaw = up.GetYaw();
            player.orientation.Pitch = up.GetPitch();
        }

        public static void SetUnitPositionToEntity(PositionComponent position, UnitPosition up)
        {
            position.Value = new Vector3(up.GetX(), up.GetY(), up.GetZ());
        }

        // 一个位置向另外一个位置移动一段距离后的点位置, 修改from的值
        public static void Move(UnitPosition from, UnitPosition target, float dis)
        {
            Vector3 v = new Vector3(target.GetX() - from.GetX(), target.GetY() - from.GetY(), target.GetZ() - from.GetZ());

            if (v.magnitude != 0)
            {
                float scale = dis / v.magnitude;

                from.SetX(from.GetX() + v.x * scale);
                from.SetY(from.GetY() + v.y * scale);
                from.SetZ(from.GetZ() + v.z * scale);
            }
        }

        public static UnitPosition FromVector(Vector3 v)
        {
            UnitPosition tempPosition = new UnitPosition();

            tempPosition.SetX(v.x);
            tempPosition.SetY(v.y);
            tempPosition.SetZ(v.z);

            return tempPosition;
        }

        public static UnitPosition GetPlayerPosition(PlayerEntity player)
        {
            UnitPosition tempPosition = new UnitPosition();

            SetPlayerToUnitPosition(player, tempPosition);

            return tempPosition;
        }

        public static UnitPosition GetEntityPosition(PositionComponent entity)
        {
            UnitPosition tempPosition = new UnitPosition();

            SetEntityToUnitPosition(entity, tempPosition);

            return tempPosition;
        }

        public static UnitPosition GetAnglePosition(UnitPosition old,
            double angle, float pitch, float distance, float height)
        {
            Vector3 dir = new Vector3();
            Vector3 end = new Vector3();

            AnglesToVector(angle, pitch, ref dir);
            Vector3DMA(new Vector3(old.GetX(), old.GetY(), old.GetZ()), distance, dir, ref end);

            UnitPosition up = new UnitPosition();
            up.SetX(end.x);
            up.SetY(end.y + height);
            up.SetZ(end.z);
            up.SetYaw(old.GetYaw());

            return up;
        }

        public static double RAD = Math.PI / 180;

        public static void AnglesToVector(double yaw, double pitch, ref Vector3 v)
        {
            float xy = (float)Math.Cos(-pitch * RAD);

            v.z = xy * (float)Math.Cos(yaw * RAD);
            v.x = xy * (float)Math.Sin(yaw * RAD);
            v.y = (float)Math.Sin(-pitch * RAD);
        }

        public static void Vector3DMA(Vector3 v, float s, Vector3 b, ref Vector3 o)
        {
            o.x = v.x + b.x * s;
            o.y = v.y + b.y * s;
            o.z = v.z + b.z * s;
        }

        public static Vector3 vectorToAngles(Vector3 dir)
        {
            double forward;
            double yaw, pitch;
            Vector3 angles = new Vector3();

            if (dir.x == 0 && dir.y == 0)
            {
                yaw = 0;
                if (dir.z > 0)
                {
                    pitch = 90;
                }
                else
                {
                    pitch = 270;
                }
            }
            else
            {
                if (dir.x != 0)
                {
                    yaw = (Math.Atan2(dir.y, dir.x) * 180 / Math.PI);
                }
                else if (dir.y > 0)
                {
                    yaw = 90;
                }
                else
                {
                    yaw = 270;
                }
                if (yaw < 0)
                {
                    yaw += 360;
                }

                forward = Math.Sqrt(dir.x * dir.x + dir.y * dir.y);
                pitch = Math.Atan2(dir.z, forward) * 180 / Math.PI;
                if (pitch < 0)
                {
                    pitch += 360;
                }
            }

            angles.x = (float)yaw;
            angles.y = (float)pitch;
            angles.z = 0;
            return angles;
        }

    }
}
