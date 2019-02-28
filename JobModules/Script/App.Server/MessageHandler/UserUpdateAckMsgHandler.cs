using System;
using App.Protobuf;
using App.Shared;
using Core.Network;
using Core.UpdateLatest;
using Core.Utils;

namespace App.Server.MessageHandler
{
    class UserUpdateAckMsgHandler : AbstractServerMessageHandler<PlayerEntity, ReusableList<UpdateLatestPacakge>>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UserUpdateAckMsgHandler));
      
        

        public UserUpdateAckMsgHandler(IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
        }


        public override void DoHandle(INetworkChannel channel, PlayerEntity entity,
            EClient2ServerMessage eClient2ServerMessage, ReusableList<UpdateLatestPacakge> messageBody)
        {
            
            var messageAck = UpdateMessageAck.Allocate();
            messageAck.AckSeq = channel.Serializer.MessageTypeInfo.LatestUpdateMessageSeq;
            _logger.DebugFormat("UserUpdateAckMsgHandler:{0}",   messageAck.AckSeq);
            channel.SendRealTime((int)EServer2ClientMessage.UpdateAck, messageAck);
            messageAck.ReleaseReference();
        
        }
    }
}