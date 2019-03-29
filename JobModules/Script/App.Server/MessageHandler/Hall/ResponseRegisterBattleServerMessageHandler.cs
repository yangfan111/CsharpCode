using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Network;
using Core.Room;
using RpcNetwork.RpcNetwork;

namespace App.Server.MessageHandler.Hall
{
    public class ResponseRegisterBattleServerMessageHandler : AbstractRpcMessageHandler
    {

        private RoomEventDispatcher _dispatcher;
        public ResponseRegisterBattleServerMessageHandler(RoomEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Handle(INetworkChannel networkChannel, object messagName, ResponseRegisterBattleServerMessage msg)
        {
            var e = RoomEvent.AllocEvent<HallServerConnectEvent>();
            _dispatcher.AddEvent(e);

            _logger.InfoFormat("Receive Response Register Battle Server Message, Has Hall Server");
        }
    }
}
