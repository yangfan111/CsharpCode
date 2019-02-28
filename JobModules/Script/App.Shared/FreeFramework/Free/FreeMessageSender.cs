using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay;
using App.Shared;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using Free.framework.ui;
using com.wd.free.unit;
using com.wd.free.util;
using com.wd.free.para;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;

namespace Assets.App.Server.GameModules.GamePlay.Free
{
    public class FreeMessageSender : IFreeMessageSender
    {

        public static void SendMessage(PlayerEntity player, SimpleProto message)
        {
            player.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
        }

        public static void SendMessage(IEventArgs args, string player, SimpleProto message)
        {
            IParable unit = args.GetUnit(FreeUtil.ReplaceVar(player, args));
            if (unit != null)
            {
                PlayerEntity p = ((FreeData)unit).Player;
                p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }

        public void SendMessage(IEventArgs args, SimpleProto message, int scope, string player)
        {
            //PlayerEntity[] players = args.GameContext.player.GetInitializedPlayerEntities();
            PlayerEntity[] players = args.GameContext.player.GetEntities();
            if (scope == 4)
            {
                foreach (PlayerEntity p in players)
                {
                    p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
                }
            }
            else if (scope == 1)
            {
                IParable unit = args.GetUnit(FreeUtil.ReplaceVar(player, args));
                if(unit != null)
                {
                    PlayerEntity p = ((FreeData)unit).Player;
                    p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
                }
            }

        }
    }
}
