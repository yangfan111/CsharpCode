using System;
using App.Shared;
using Core.Free;
using Free.framework;
using UnityEngine;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeAddMarkHandler : IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.MarkPos;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            foreach (var other in room.RoomContexts.player.GetInitializedPlayerEntities())
            {
                try
                {
                    if (other.hasNetwork && other.hasPlayerInfo && other.playerInfo.TeamId == player.playerInfo.TeamId)
                    {
                        SimpleProto proto = FreePool.Allocate();
                        proto.Key = message.Key;
                        proto.Ls.Add(player.playerInfo.PlayerId);
                        proto.Ins.Add(player.playerInfo.Num);
                        proto.Fs.Add(message.Fs[0]); //x
                        proto.Fs.Add(message.Fs[1]); //y
                        proto.Fs.Add(message.Fs[2]); //flag
                        other.network.NetworkChannel.SendReliable((int) EServer2ClientMessage.FreeData, proto);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat(e.Message);
                }
            }
        }
    }
}
