using System.Net.Sockets;

namespace VNet.Base.Interface
{
    public interface IVNetPeer
    {
        void Send(byte[] bytes, int length, int offset);
        void OnDisconnect();
        int ConnectId { get; }
        SocketError ErrorCode { get; }
        string DebugInfo { get; }
        string Ip { get; }
    }
}
