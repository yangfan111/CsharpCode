using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Core.Network;

namespace App.Server.Robot
{
    class RobotFakeNetworkMessageSerializer : INetworkMessageSerializer
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public long Serialize(Stream outStream, int messageType, object message)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(Stream inStream, int messageType)
        {
            throw new NotImplementedException();
        }

        public IMessageTypeInfo MessageTypeInfo { get; private set; }
    }
    class RobotFakeNetworkChannel:INetworkChannel

    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event Action<INetworkChannel> Disconnected;
        public event Action<INetworkChannel, int, object> MessageReceived;
        public void SendReliable(int messageType, object messageBody)
        {
            
        }

        public void SendRealTime(int messageType, object messageBody)
        {
            
        }

        public bool IsConnected { get; private set; }
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public int LocalConnId { get; set; }
        public int RemoteConnId { get; set; }
        public int UdpPort { get; set; }
        public INetworkMessageSerializer Serializer { get; set; }
        public int Id { get; private set; }
        public SocketError ErrorCode { get; set; }
        public void FlowSend(bool Type, long bytes, long ms =0)
        {
            throw new NotImplementedException();
        }

        public void FlowRecv(bool Type, long bytes, long ms =0)
        {
            throw new NotImplementedException();
        }

        public FlowStatue TcpFlowStatus { get; private set; }
        public FlowStatue UdpFlowStatus { get; private set; }
        public string IdInfo()
        {
            throw new NotImplementedException();
        }
    }
}
