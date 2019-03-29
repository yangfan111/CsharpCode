using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using UnityEngine;
using Free.framework;
using Core.Free;
using Assets.App.Server.GameModules.GamePlay.Free;

namespace App.Shared.FreeFramework.framework.ai.move
{
    [Serializable]
    public class MoveToAiAction : AbstractPlayerAction
    {
        private long startTime;
        private IPosSelector pos;
        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
            if (string.IsNullOrEmpty(player))
            {
                player = "current";
            }

            if (DateTime.Now.Ticks - startTime < 10000 * 200)
            {
                return;
            }
            startTime = DateTime.Now.Ticks;

            PlayerEntity entity = GetPlayerEntity(args);
            if (entity != null)
            {
                UnitPosition target = pos.Select(args);
                Vector3 from = entity.position.Value;
                Vector3 to = AutoMoveUtil.GetGround(target.Vector3).point;

                float dis = Vector3.Distance(from, to);

                Debug.LogFormat("dis:{0} to:{1}, from:{2}", dis, to, from);

                if (dis < 2)
                {
                    args.FreeContext.AiSuccess = true;
                    Debug.LogFormat("arrive");

                    PlayerInterceptCommands.DoNothing(entity);

                    if (action != null)
                    {
                        action.Act(args);
                    }
                }
                else
                {
                    Vector3 v = GetNextPoint(entity, from, to, dis);
                    Debug.LogFormat("move to {0}", v);
                    //player.orientation.Yaw = player.orientation.Yaw + Vector3.Angle(from, v);

                    PlayerInterceptCommands.WalkTo(entity, v);
                }
            }
        }

        private Vector3 GetNextPoint(PlayerEntity player, Vector3 from, Vector3 to, float dis)
        {
            Vector3 v = to;

            if (dis > 100)
            {
                v = AutoMoveUtil.GetDistancePoint(from, to, 100);
            }

            if (AutoMoveUtil.CanDirectMoveTo(from, to))
            {
                return v;
            }
            else
            {
                v = GetDirectMovePos(from, to, Math.Min((int)Vector3.Distance(from, to), 100));
            }

            return v;
        }

        private Vector3 GetDirectMovePos(Vector3 from, Vector3 to, int dis)
        {
            for (int i = 1; i <= 18; i++)
            {
                Vector3 deltaV = AutoMoveUtil.GetGround(AutoMoveUtil.GetRotationPos(from, to, i * 10, dis)).point;
                if (AutoMoveUtil.CanDirectMoveTo(from, deltaV))
                {
                    Debug.LogFormat("select angle {0}, pos:{1}", i * 10, deltaV);
                    return deltaV;
                }
                deltaV = AutoMoveUtil.GetGround(AutoMoveUtil.GetRotationPos(from, to, -i * 10, dis)).point;
                if (AutoMoveUtil.CanDirectMoveTo(from, deltaV))
                {
                    Debug.LogFormat("select angle {0}, pos:{1}", -i * 10, deltaV);
                    return deltaV;
                }
            }

            Debug.LogFormat("select none");

            return to;
        }

        private void SendCmd(PlayerEntity player)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add(0);
            FreeMessageSender.SendMessage(player, proto);
        }

        private void SendCmd(PlayerEntity player, Vector3 v)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add(1);
            proto.Fs.Add(v.x);
            proto.Fs.Add(v.y);
            proto.Fs.Add(v.z);

            FreeMessageSender.SendMessage(player, proto);
        }
    }
}
