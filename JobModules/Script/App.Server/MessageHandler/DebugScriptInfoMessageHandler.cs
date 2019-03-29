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
    public class DebugScriptInfoMessageHandler : AbstractServerMessageHandler<PlayerEntity, DebugScriptInfo>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(DebugScriptInfoMessageHandler));
        private static Action<string> _handler;

        public DebugScriptInfoMessageHandler(IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, DebugScriptInfo messageBody)
        {
            if (_handler != null)
            {
                try
                {
                    _logger.DebugFormat("Receive Debug Script Message {0}", messageBody.Info);
                    _handler(messageBody.Info);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("DebugScriptInfo Process Error {0}", e);
                }
                
            }
        }

        public static void SetHandler(Action<string> handler)
        {
            _handler = handler;
        }
    }
}
