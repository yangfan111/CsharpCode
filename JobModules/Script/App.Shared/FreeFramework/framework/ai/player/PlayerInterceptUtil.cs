using Core.Prediction.UserPrediction.Cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.FreeFramework.framework.ai.player
{
    public class PlayerInterceptUtil
    {
        public static bool IsFaceTo(PlayerEntity player, Vector3 v, IUserCmd cmd)
        {
            float toYaw = (float)(Math.Atan2(v.x - player.position.Value.x, v.z - player.position.Value.z) * 180 / Math.PI);

            float deltaYaw = toYaw - player.orientation.Yaw;
            while (deltaYaw > 180)
            {
                deltaYaw = deltaYaw - 360;
            }
            while (deltaYaw < -180)
            {
                deltaYaw = deltaYaw + 360;
            }

            if (Math.Abs(deltaYaw) < 2)
            {
                return true;
            }
            else
            {
                if (deltaYaw > 0)
                {
                    cmd.DeltaYaw = 2;
                    if (player.orientation.Yaw > toYaw)
                    {
                        cmd.DeltaYaw = toYaw - player.orientation.Yaw;
                    }
                }
                else
                {
                    cmd.DeltaYaw = -2;
                    if (player.orientation.Yaw < toYaw)
                    {
                        cmd.DeltaYaw = toYaw - player.orientation.Yaw;
                    }
                }
            }

            return false;
        }
    }
}
