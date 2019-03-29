using System.Linq;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Network;
using Core.Room;
using RpcNetwork.RpcNetwork;

namespace App.Server.MessageHandler.Hall
{
    public class RequestJoinRoomListMessageHandler : AbstractRpcMessageHandler
    {
        private RoomEventDispatcher _dispatcher;
        public RequestJoinRoomListMessageHandler(RoomEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Handle(INetworkChannel networkChannel, object messagName, RequestJoinRoomListMessage message)
        {
            var e = RoomEvent.AllocEvent<JoinRoomListEvent>();
            e.HallRoomId = message.HallRoomId;
            int count = message.Players.Count;
            var roomPlayerList = new RoomPlayer[count];
            for (int i = 0; i < count; ++i)
            {
                roomPlayerList[i] = message.Players[i];
            }
            e.RoomPlayerList = roomPlayerList;

            _dispatcher.AddEvent(e);

            _logger.InfoFormat("Receive Join Room List Message Hall Room Id {0} Count {1}", e.HallRoomId, count);
        }
    }
}
