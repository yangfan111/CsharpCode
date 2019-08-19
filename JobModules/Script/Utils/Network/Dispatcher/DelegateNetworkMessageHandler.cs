using System;

namespace Core.Network
{
    public class DelegateNetworkMessageHandler: INetworkMessageHandler
    {
        private Action<INetworkChannel, int, object> _target;

        public DelegateNetworkMessageHandler(Action<INetworkChannel, int, object> target)
        {
            _target = target;
        }

        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            _target.Invoke(networkChannel, messageType, messageBody);
        }
    }
}