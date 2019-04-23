using System;
using System.IO;
using VNet.Base.Interface;

namespace Core.Network
{
    public interface INetworkService
    {
        event Action<INetworkChannel> ChannelConnected;
        event Action<INetworkChannel> ChannelDisconnected;
    }
    public interface INetworkClient: IDisposable, INetworkService
    {
        event Action<IVNetPeer> OnReliableConnectFailedListener;
        event Action<IVNetPeer> OnReliableConnectListener;
        event Action<IVNetPeer> OnReliableDisconnectListener;
        event Action<IVNetPeer, MemoryStream> OnReliableReceiveListener;

        void Connect(string ip, NetworkPortInfo portInfo);
        void Update();
        bool IsConnected { get; }
        bool IsMultiThread { get; }
        void FlowTick(float time);
    }
}