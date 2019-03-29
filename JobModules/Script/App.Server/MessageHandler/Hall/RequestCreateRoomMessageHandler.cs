using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Network;
using Core.Room;
using Core.Utils;
using RpcNetwork.RpcNetwork;

namespace App.Server.MessageHandler.Hall
{
    /* class RequestCreateRoomMessageHandler : AbstractRpcMessageHandler
     {
         private IHallRoomServer _hallRoomServer;
         public RequestCreateRoomMessageHandler(IHallRoomServer hallRoomServer)
         {
             _hallRoomServer = hallRoomServer;
         }

         public void Handle(INetworkChannel networkChannel, object messagName, RequestCreateRoomMessage messageData)
         {
             if (_hallRoomServer._hallPlayerManager.HandleCreateRoomMessage(networkChannel, messageData))
             {
                 AllocationClient.Instance.UpdateBattleServerStatus(1);
             }
         }
     }*/

    class RequestCreateRoomMessageHandler : AbstractRpcMessageHandler
    {
        private RoomEventDispatcher _dispatcher;
        public RequestCreateRoomMessageHandler(RoomEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Handle(INetworkChannel networkChannel, object messagName, RequestCreateRoomMessage messageData)
        {
            var e = RoomEvent.AllocEvent<CreateRoomEvent>();
            e.Message = messageData as RequestCreateRoomMessage;
            _dispatcher.AddEvent(e);

            if(e.Message !=  null)
                _logger.InfoFormat("Receive Create Room Message Hall Room Id {0} Map Id {1}",  e.Message.HallRoomId, e.Message.MapId);
            else
                _logger.ErrorFormat("Receive Create Room Message is Null");
        }
    }
}
