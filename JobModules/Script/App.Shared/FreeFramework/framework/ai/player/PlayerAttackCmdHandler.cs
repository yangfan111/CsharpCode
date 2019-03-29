using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.FreeFramework.framework.ai.player
{
    public class PlayerAttackCmdHandler : IPlayerCmdHandler
    {
        public bool CanHandle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            return player.playerIntercept.AttackPlayerId != 0;
        }

        public void Handle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            PlayerEntity target = contexts.player.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(player.playerIntercept.AttackPlayerId, (short)EEntityType.Player));
           /// DebugUtil.LogInUnity("target.WeaponController():" + target.WeaponController().ToString());
            if (target != null)
            {
                if (PlayerInterceptUtil.IsFaceTo(player, target.position.Value, player.userCmd.LastTemp))
                {
                    player.userCmd.LastTemp.IsLeftAttack = true;
                }
            }
        }
    }
}
