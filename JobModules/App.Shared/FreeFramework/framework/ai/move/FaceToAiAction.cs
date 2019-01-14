using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using UnityEngine;

namespace App.Shared.FreeFramework.framework.ai.move
{
    [Serializable]
    public class FaceToAiAction : AbstractPlayerAction
    {
        private long startTime;

        private IPosSelector pos;

        private float rotationSpeed = 90;

        public override void DoAction(IEventArgs args)
        {
            if (string.IsNullOrEmpty(player))
            {
                player = "current";
            }
            PlayerEntity entity = GetPlayerEntity(args);

            if (entity != null)
            {
                UnitPosition up = pos.Select(args);
                float toYaw = (float)(Math.Atan2(up.Vector3.x - entity.position.Value.x, up.Vector3.z - entity.position.Value.z) * 180 / Math.PI);

                double deltaYaw = toYaw - entity.orientation.Yaw;

                if (Math.Abs(deltaYaw) < 3)
                {
                    args.FreeContext.AiSuccess = true;
                }
                else
                {
                    Debug.LogFormat("face delta:{0}, from:{1} {3}, to:{2} {4}", deltaYaw, entity.orientation.Yaw, toYaw, entity.position.Value, up.Vector3);
                }

                if (startTime > 0)
                {
                    float time = (DateTime.Now.Ticks - startTime) / 10000;

                    if (deltaYaw > 0)
                    {
                        entity.orientation.Yaw += rotationSpeed * time / 1000;
                        if (entity.orientation.Yaw > toYaw)
                        {
                            entity.orientation.Yaw = toYaw;
                        }
                    }
                    else
                    {
                        entity.orientation.Yaw -= rotationSpeed * time / 1000;
                        if (entity.orientation.Yaw < toYaw)
                        {
                            entity.orientation.Yaw = toYaw;
                        }
                    }

                }
            }

            startTime = DateTime.Now.Ticks;
        }
    }
}
