using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.entity;
using App.Shared.Components.Player;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.para.exp;
using com.wd.free.unit;
using com.wd.free.util;
using UnityEngine;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.Free.map.position
{
    [Serializable]
    public class PosEntitySelector : AbstractPosSelector
    {
        public string condition;

        [NonSerialized]
        private ExpParaCondition con;

        public string player;

        public string distance;
        private string height;
        private string pitch;
        private string angle;
        private bool fromEye;

        public override UnitPosition Select(IEventArgs args)
        {
            IniCon();

            UnitPosition up = new UnitPosition();

            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            if (!string.IsNullOrEmpty(player))
            {
                object obj = fr.GetEntity(player);
                if (obj != null)
                {
                    if (obj is PlayerEntity)
                    {
                        PlayerEntity playerEntity = (PlayerEntity) obj;
                        fr.TempUse("current", (FreeData)playerEntity.freeData.FreeData);

                        UnityPositionUtil.SetPlayerToUnitPosition(playerEntity, up);
                        if (fromEye)
                        {
                            up.SetY(up.GetY() + 1.7f);
                        }

                        fr.Resume("current");
                    }

                    if (obj is FreeMoveEntity)
                    {
                        FreeMoveEntity playerEntity = (FreeMoveEntity)obj;
                        fr.TempUse("current", (FreeEntityData)playerEntity.freeData.FreeData);

                        UnityPositionUtil.SetEntityToUnitPosition(playerEntity.position, up);

                        fr.Resume("current");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(condition))
            {
                if (con == null || con.Meet(args))
                {
                    foreach (PlayerEntity unit in args.GameContext.player.GetInitializedPlayerEntities())
                    {
                        if (unit.hasFreeData)
                        {
                            fr.TempUse("current", (FreeData)unit.freeData.FreeData);

                            UnityPositionUtil.SetPlayerToUnitPosition(unit, up);

                            fr.Resume("current");
                        }
                    }
                }
            }

            return GetPosition(args, up, FreeUtil.ReplaceFloat(angle, args) + up.GetYaw());
        }

        private void IniCon()
        {
            if (con == null || (condition != null && condition.Contains(FreeUtil.VAR_START)
                                                  && condition.Contains(FreeUtil.VAR_END)))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    con = new ExpParaCondition(condition);
                }
            }
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
    }
}
