using System.Collections.Generic;

namespace Core.Network
{
    public partial class NetworkMessageDispatcher
    {
        private class CompositeNetworkMessageHandler : INetworkMessageHandler
        {
            private List<INetworkMessageHandler> _handlers = new List<INetworkMessageHandler>();

            public void Register(INetworkMessageHandler handler)
            {
                _handlers.Add(handler);
            }

            public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
            {
                foreach (var handler in _handlers)
                {
                    handler.Handle(networkChannel, messageType, messageBody);
                }
            }

            public void UnRegister(INetworkMessageHandler handler)
            {
                _handlers.Remove(handler);
            }
        }
    }
}