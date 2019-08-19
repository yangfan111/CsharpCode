using System;
using System.Net.Sockets;
using Utils.Replay;

namespace Core.Network
{
  
    public interface INetworkChannel : IDisposable
    {
        void SendReliable(int messageType, object messageBody);
        void SendRealTime(int messageType, object messageBody);
        FlowStatue TcpFlowStatus{get;}
        FlowStatue UdpFlowStatus{get;}
        bool IsConnected { get; }
        bool IsUdpConnected { get; }
        event Action<INetworkChannel> Disconnected;
        event Action<INetworkChannel, int, object> MessageReceived;
    
     
        void Disconnect();
        int LocalConnId { get; }
        int RemoteConnId { get;  }
        
        INetworkMessageSerializer Serializer { get; set; }
        IRecordManager Recoder { get; set; }
        int Id { get; }
        SocketError ErrorCode { get; }
        void FlowSend(bool Type, long bytes, long ms=0);
        void FlowRecv(bool Type, long bytes, long ms=0);
     
        string IdInfo();
       
        void RealTimeConnect(int udpPort, int remoteUdpId);
    }
}