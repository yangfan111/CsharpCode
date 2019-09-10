using System;
using App.Protobuf;
using App.Shared;
using Core.Network;
using Core.UpdateLatest;
using Core.Utils;

namespace App.Server.MessageHandler
{
    class UserUpdateMsgHandler : AbstractServerMessageHandler<PlayerEntity, ReusableList<UpdateLatestPacakge>>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerRoom));
        private int _lastSeq = -1;
        private DateTime _lastRecvTime = DateTime.MinValue;
        private int _frameRateInv = 50;

        public UserUpdateMsgHandler(IPlayerEntityDic<PlayerEntity> converter, Contexts contexts) : base(converter)
        {
        }


        public override void DoHandle(INetworkChannel channel, PlayerEntity entity,
            EClient2ServerMessage eClient2ServerMessage, ReusableList<UpdateLatestPacakge> messageBody)
        {
            
            var pool = entity.updateMessagePool.Value;
            foreach (UpdateLatestPacakge updateLatestPacakge in messageBody.Value)
            {
                pool.AddMessage(updateLatestPacakge);
                updateLatestPacakge.ReleaseReference();
                _logger.DebugFormat("DoHandle:{0}", updateLatestPacakge.Head.LastUserCmdSeq);
                entity.network.NetworkChannel.Serializer.MessageTypeInfo.SetReplicationAckId(updateLatestPacakge.Head.LastSnapshotId);
            }
            
        
        }
    }
}