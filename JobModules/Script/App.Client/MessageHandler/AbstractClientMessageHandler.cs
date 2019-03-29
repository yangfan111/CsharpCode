using System;
using Core.Network;

namespace App.Client.Console.MessageHandler
{
    public abstract class AbstractClientMessageHandler<TMessage> : INetworkMessageHandler
    {
        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            if (!(messageBody is TMessage))
                throw new Exception("error type");
           
            DoHandle(messageType, (TMessage) messageBody);
            
        }

        public abstract void DoHandle(int messageType, TMessage messageBody);
    }
}