using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Shared;
using Core.Network;
using Core.Utils;

namespace App.Server.MessageHandler
{
    public class ServerGameOverMessageHandler : AbstractServerMessageHandler<PlayerEntity, GameOverMesssage>
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(ServerGameOverMessageHandler));

        public ServerGameOverMessageHandler(IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {

        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, GameOverMesssage messageBody)
        {
            _logger.InfoFormat("Server Recevie GameOver Message From  Player {0}",  entity.entityKey.Value);
            channel.Disconnect();
        }
    }
}
