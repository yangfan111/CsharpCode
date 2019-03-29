using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Network;
using Core.Room;
using RpcNetwork.RpcNetwork;

namespace App.Server.MessageHandler.Hall
{
    /*  class RequestJoinRoomMessageHandler : AbstractRpcMessageHandler
      {
          private IHallRoomServer _hallRoomServer;
          public RequestJoinRoomMessageHandler(IHallRoomServer hallRoomServer)
          {
              _hallRoomServer = hallRoomServer;
          }

          public void Handle(INetworkChannel networkChannel, object messagName, LongData roomId, RoomPlayer player)
          {
               _hallRoomServer._hallPlayerManager.HandleJoinRoomMessage(networkChannel, roomId.Value, player);
          }
      }*/

    class RequestJoinRoomMessageHandler : AbstractRpcMessageHandler
    {
        private RoomEventDispatcher _dispatcher;
        public RequestJoinRoomMessageHandler(RoomEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Handle(INetworkChannel networkChannel, object messagName, LongData roomId, RoomPlayer player)
        {
            var e = RoomEvent.AllocEvent<JoinRoomEvent>();
            e.HallRoomId = roomId.Value;
            e.RoomPlayer = player;

            _dispatcher.AddEvent(e);

            _logger.InfoFormat("Receive Join Room Message Hall Room Id {0} Map Id {1}", roomId, player.Id);
        }
    }
}
