using System;
using Core.Network;
using VNet.Base.Interface;
using VNet.Base.LiteNet;
using VNet.Base.Tcp;
using VNet.Base.Udp;

namespace VNet
{
    public class VNetworkServer : VNetworkService, INetworkServer
    {
        private readonly IVNetServer _reliableServer;
        private readonly IVNetServer _realTimeServer;

        public VNetworkServer(string name="default") : this(new TcpServer(false), new LiteNetServer(), false,name)
        {
            
        }

        public VNetworkServer(IVNetServer reliable, IVNetServer realtime, bool littleEndian,string name="default"):base(name)
        {
            _reliableServer = reliable;
            _realTimeServer = realtime;
            if (_reliableServer != null)
            {
                _reliableServer.Init();
            }
            InitMemoryCache();
            LittleEndian = littleEndian;
        }

        private void InitMemoryCache()
        {
            var preallocCount = 1024;
            var array = new VNetPacketMemSteam[preallocCount];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = VNetPacketMemSteam.Allocate();
            }
            for (var i = 0; i < array.Length; i++)
            {
                array[i].ReleaseReference();
            }
        }

        protected override void Poll()
        {
            RealTimePoll();
        }

        public void RealTimePoll()
        {
            if (_realTimeServer != null)
            {
                _realTimeServer.Poll();
            }
        }

        public void Listen(NetworkPortInfo port, int threadCount=2,bool muitilThread=false)
        {
            ReliableListen(port);
            RealTimeListen(port);
            Start(threadCount,!muitilThread,muitilThread);
        }

        public void ReliableListen(NetworkPortInfo port)
        {
            if (_reliableServer != null)
            {
                _reliableServer.OnAcceptListener += OnReliableConnected;
                _reliableServer.OnReceiveListener += OnReliableReceive;
                _reliableServer.OnDisconnectListener += OnDisconnect;
                _reliableServer.Bind(port.TcpPort);
            }
        }

        public void RealTimeListen(NetworkPortInfo port)
        {
            if (_realTimeServer != null)
            {
                _realTimeServer.Init();
                _realTimeServer.Bind(port.UdpPort);
                _realTimeServer.OnReceiveListener += OnRealTimeReceive;
                _realTimeServer.OnAcceptListener += OnRealTimeConnected;
                _realTimeServer.OnDisconnectListener += OnRealTimeDisconnect;
            }
        }

        public void Stop()
        {
            if (_reliableServer != null)
            {
                _reliableServer.Stop();
            }
            if(_realTimeServer != null)
            {
                _realTimeServer.Stop();
            }
        }

    }
}
