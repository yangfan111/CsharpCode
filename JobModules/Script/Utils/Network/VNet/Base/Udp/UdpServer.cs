using System;
using System.Net;
using System.Net.Sockets;
using VNet.Base.Interface;
using VNet.Base.LiteNet;

namespace VNet.Base.Udp
{
    public class UdpServer : UdpService, IVNetServer
    {
        public event Action<IVNetPeer> OnAcceptListener;
        public event Action<IVNetPeer> OnDisconnectListener;
        public void Init()
        {

        }

        public void Poll()
        {

        }

        public void Stop()
        {
#if NET_4_6 && UNITY_2017
            RealTimeSocket.Dispose();
#else
            RealTimeSocket.Close();
#endif
        }

        public void Bind(int port)
        {
            try
            {
                var point = new IPEndPoint(IPAddress.Any, port);
                RealTimeSocket = new Socket(point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                RealTimeSocket.Bind(point);
                StartReceive();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
