using System;
using System.Net.Sockets;

namespace Core.Network
{
  
    public interface INetworkChannel : IDisposable
    {
        void SendReliable(int messageType, object messageBody);
        void SendRealTime(int messageType, object messageBody);
        FlowStatue TcpFlowStatus{get;}
        FlowStatue UdpFlowStatus{get;}
        bool IsConnected { get; }
        event Action<INetworkChannel> Disconnected;
        event Action<INetworkChannel, int, object> MessageReceived;
    
     
        void Disconnect();
        int LocalConnId { get; set; }
        int RemoteConnId { get; set; }
        int UdpPort { get; set; }
        INetworkMessageSerializer Serializer { get; set; }
        int Id { get; }
        SocketError ErrorCode { get; }
        void FlowSend(bool Type, long bytes, long ms=0);
        void FlowRecv(bool Type, long bytes, long ms=0);
     
        string IdInfo();
    }
}