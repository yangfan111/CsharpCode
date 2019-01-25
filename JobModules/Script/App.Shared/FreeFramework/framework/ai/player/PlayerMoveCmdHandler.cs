using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;
using App.Shared.FreeFramework.framework.ai.move;
using com.wd.free.ai;

namespace App.Shared.FreeFramework.framework.ai.player
{
    public class PlayerMoveCmdHandler : IPlayerCmdHandler
    {
        private Vector3 Arrive = new Vector3(-100000, -10000, -10000);

        public bool CanHandle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            return player.playerIntercept.InterceptType == (int)PlayerActEnum.CmdType.Walk;
        }

        public void Handle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            Vector3 to = player.playerIntercept.MovePos;

            if (Vector3.Distance(player.position.Value, to) < 1)
            {
                player.playerIntercept.InterceptType = (int)PlayerActEnum.CmdType.Idle;
                return;
            }

            if (!PlayerInterceptUtil.IsFaceTo(player, to, cmd))
            {
                return;
            }

            AutoMoveUtil.MoveType m = AutoMoveUtil.GetMoveType(player, to);
            switch (m)
            {
                case AutoMoveUtil.MoveType.Walk:
                    cmd.MoveVertical = 1;
                    break;
                case AutoMoveUtil.MoveType.Jump:
                    cmd.MoveVertical = 1;
                    cmd.IsJump = true;
                    cmd.IsCrouch = true;
                    break;
                default:
                    break;
            }
        }

    }
}
