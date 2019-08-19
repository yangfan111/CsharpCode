using Core.Network;

namespace App.Shared.Client
{
    public interface IClientNetwork
    {
        void OnNetworkConnected(INetworkChannel networkChannel);
        void NetworkChannelOnMessageReceived(INetworkChannel networkChannel, int messageType, object messageBody);
        void OnNetworkDisconnected();
        void Init();
        INetworkChannel Channel { get; }
    }
}