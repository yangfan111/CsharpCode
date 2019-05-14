using System;
using App.Shared;
using Core.Free;
using Free.framework;
using UnityEngine;
using App.Shared.GameModules.Player;
using App.Shared.EntityFactory;
using Utils.Configuration;

namespace App.Server.GameModules.GamePlay.free.client
{

    public class FreeSprayPaintHandle : IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            int serverTime =  room.RoomContexts.session.currentTimeObject.CurrentTime;
            var config = IndividuationConfigManager.GetInstance().GetConfigById(message.Ins[0]);
            int intervalCD = 0;
            if (null == config) {
                intervalCD = 0;
            }
            else {
                intervalCD = config.IntervalCD;
            }
            return message.Key == FreeMessageConstant.PlayerSprayPaint &&
                  player.playerSpray.mLastCreateTime + intervalCD < serverTime;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            int serverTime = room.RoomContexts.session.currentTimeObject.CurrentTime;
            ClientEffectFactory.CreateSprayPaint(room.RoomContexts.clientEffect,
                room.RoomContexts.session.commonSession.EntityIdGenerator,
                new Vector3(message.Fs[0], message.Fs[1], message.Fs[2]),
                new Vector3(message.Fs[3], message.Fs[4], message.Fs[5]),
                1,
                new Vector3(message.Fs[6], message.Fs[7], message.Fs[8]),
                XmlConfig.ESprayPrintType.TypeBounds_1,
                message.Ins[0], message.Ins[1]);
            player.playerSpray.mLastCreateTime = (float)serverTime;
        }
    }
}
