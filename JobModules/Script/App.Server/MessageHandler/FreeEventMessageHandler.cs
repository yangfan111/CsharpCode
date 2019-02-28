using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Server.GameModules.GamePlay.free.client;
using App.Shared;
using Core.Network;
using Core.Prediction.VehiclePrediction.TimeSync;
using Free.framework;

namespace App.Server.MessageHandler
{
    public class FreeEventMessageHandler : AbstractServerMessageHandler<PlayerEntity, SimpleProto>
    {
        private ServerRoom _room;

        public FreeEventMessageHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            this._room = (ServerRoom)converter;
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage,
            SimpleProto messageBody)
        {
            FreeMessageHandler.Handle(_room, entity, messageBody);
        }
    }
}
