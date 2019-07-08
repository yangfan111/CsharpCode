using System;
using System.IO;
using System.Net.Sockets;

namespace VNet.Base.Interface
{
    public interface IVNetClient
    {
        void Init();
        void Connect(string ip, int port, long connId);
        void ReConnect();
        void CloseConnect();
        void Poll();
        event Action<IVNetPeer> OnConnectFailedListener;
        event Action<IVNetPeer> OnConnectListener;
        event Action<IVNetPeer> OnDisconnectListener;
        event Action<IVNetPeer, MemoryStream> OnReceiveListener;
        bool IsConnected { get; }
    }
}
