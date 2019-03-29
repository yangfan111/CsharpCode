using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.UserPrediction.Cmd;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Shared.FreeFramework.framework.ai.player
{
    public class PlayerKeyCmdHandler : IPlayerCmdHandler
    {
        public bool CanHandle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            return !player.playerIntercept.PresssKeys.Empty || !player.playerIntercept.InterceptKeys.Empty;
        }

        public void Handle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            int[] keys = player.playerIntercept.PresssKeys.Keys;
            for(int i = 0; i < keys.Length; i++)
            {
                SimplePlayerInput.PressKey(cmd, keys[i]);
            }

            player.playerIntercept.InterceptKeys.Frame();

            keys = player.playerIntercept.InterceptKeys.Keys;
            for (int i = 0; i < keys.Length; i++)
            {
                SimplePlayerInput.ReleaseKey(cmd, keys[i]);
            }

            player.playerIntercept.PresssKeys.Frame();
        }
    }
}
