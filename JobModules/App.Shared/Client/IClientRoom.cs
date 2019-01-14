using Core.Network;

namespace App.Shared.Client
{
    public interface IClientRoom
    {
        void OnNetworkConnected(INetworkChannel networkChannel);
        void OnNetworkDisconnected();
        void Update();
        void RegisterCommand(string command, string desc, string usage);
    }
}