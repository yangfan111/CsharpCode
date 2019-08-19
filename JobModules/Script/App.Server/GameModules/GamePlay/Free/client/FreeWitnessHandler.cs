using App.Shared.EntityFactory;
using Core.Attack;
using Core.Free;
using Free.framework;
using UnityEngine;
using Utils.Configuration;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeWitnessHandler : IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.Witness;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            bool witness = message.Bs[0];
            player.gamePlay.Witness = witness;
            if (witness)
                player.playerMask.SelfMask = (byte)EPlayerMask.Invincible;
            else
            {
                player.playerMask.SelfMask = (byte)EPlayerMask.TeamA | (byte)EPlayerMask.TeamB;
                player.playerMask.TargetMask = (byte)EPlayerMask.TeamA | (byte)EPlayerMask.TeamB;
            }
        }
    }
}
