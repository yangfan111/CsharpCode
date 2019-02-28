using System;
using App.Shared;
using Core.Network;
using Core.Utils;

namespace App.Server.MessageHandler
{
    public abstract class AbstractServerMessageHandler<TPlayer, TMessage> : INetworkMessageHandler
    {
        protected static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractServerMessageHandler<TPlayer, TMessage>));
        private IPlayerEntityDic<TPlayer> _converter;
        protected AbstractServerMessageHandler(IPlayerEntityDic<TPlayer> converter)
        {
            _converter = converter;
        }

        public void Handle(INetworkChannel networkChannel, int eClient2ServerMessage, object messageBody)
        {
            if (!(messageBody is TMessage))
                throw new Exception("error type");

            var player = _converter.GetPlayer(networkChannel);
            if (player == null)
            {
                _logger.ErrorFormat("illegal ChannelOnDisonnected event received {0}", networkChannel);
            }
            else
            {
                DoHandle(networkChannel, player, (EClient2ServerMessage)eClient2ServerMessage, (TMessage) messageBody);
            }
        }

        public abstract void DoHandle(INetworkChannel channel, TPlayer entity, EClient2ServerMessage eClient2ServerMessage, TMessage messageBody);
    }
}