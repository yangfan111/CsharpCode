using System;

namespace Core.Network
{
    public struct NetworkPortInfo
    {
        public NetworkPortInfo(int udpPort)
        {
            UdpPort = udpPort;
            TcpPort = 0;
        }

        public NetworkPortInfo(int tcpPort, int udpPort)
        {
            UdpPort = udpPort;
            TcpPort = tcpPort;
        }

        public int UdpPort;
        public int TcpPort;

        public override string ToString()
        {
            return string.Format("TcpPort {0}, UdpPort: {1}",TcpPort, UdpPort);
        }
    }
    public interface INetworkServer : IDisposable, INetworkService
    {

        void Listen(NetworkPortInfo networkPortInfo, int threadCount = 2, bool muitilThread = false);
        void Update();
        void FlowTick(float time);
        void Stop();
    }
}