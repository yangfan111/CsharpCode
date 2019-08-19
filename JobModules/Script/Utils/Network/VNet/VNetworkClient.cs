using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Core.Network;
using Core.Utils;
using VNet.Base.Interface;
using VNet.Base.LiteNet;
using VNet.Base.Tcp;
using VNet.Base.Udp;

namespace VNet
{
    public class VNetworkClient : VNetworkService, INetworkClient, IRealTimeConnector
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VNetworkClient));
        private readonly IVNetClient _reliableClient;
        private readonly IVNetClient _realTimeClient;
        public event Action<IVNetPeer> OnReliableConnectFailedListener;
        public event Action<IVNetPeer> OnReliableConnectListener;
        public event Action<IVNetPeer> OnReliableDisconnectListener;
        public event Action<IVNetPeer, MemoryStream> OnReliableReceiveListener;
        public event Action<IVNetPeer> OnRealTimeConnectListener;

        public bool IsConnected
        {
            get { return _reliableClient != null && _reliableClient.IsConnected; }
        }

        public bool IsMultiThread { get; private set; }

        private string _ip;
      


        public int UdpPort { get; private set; }
           
        
        
        public int RemoteConnId { get; private set; }

        public bool isRealTimeConnected
        {
            get { return _realTimeClient.IsConnected; }
        }

       

        protected override void Poll()
        {
            RealTimePoll();
        }

        public void RealTimePoll()
        {
            if (_realTimeClient != null)
            {
                _realTimeClient.Poll();
            }
        }

        public VNetworkClient(string name = "default") : this(new VNet.Base.Tcp.TcpClient(false), new LiteNetClient(), false, name,
            true)
        {
        }

        public VNetworkClient(IVNetClient reliableClient, IVNetClient realTimeClient, bool littleEndian,
            string name, bool multiThread) : base(name)
        {
            _reliableClient = reliableClient;
            _realTimeClient = realTimeClient;
            LittleEndian = littleEndian;
            IsMultiThread = multiThread;
            Start(1, !IsMultiThread, IsMultiThread);
        }

        public void Connect(string ip,NetworkPortInfo portInfo)
        {
            var tcpPort = portInfo.TcpPort;
            Logger.InfoFormat("Service {2} Connect to {0}:{1}", ip, tcpPort, ServiceName);
            if (_reliableClient != null)
            {
                _reliableClient.Init();
                _reliableClient.Connect(ip, tcpPort, 0);
                _reliableClient.OnConnectListener += OnReliableConnected;
                _reliableClient.OnReceiveListener += OnReliableReceive;

                _reliableClient.OnReceiveListener += OnReliableReceiveListener;
                _reliableClient.OnConnectListener += OnReliableConnectListener;
                _reliableClient.OnConnectFailedListener += OnReliableConnectFailedListener;
                _reliableClient.OnDisconnectListener += OnDisconnect;
            }

            _ip = ip;
           
        }

        public void RealTimeConnect(int udpPort, int connId)
        {
            UdpPort = udpPort;
            RemoteConnId = connId;
            RealTimeConnect(_ip, UdpPort, connId);
        }

        private void RealTimeConnect(string ip, int port, int connId)
        {
            if (null == _realTimeClient)
            {
                Logger.Error("RealTime client is null!");
                return;
            }

            Logger.InfoFormat("RealTime Connect to {0}:{1}", ip, port);

            _realTimeClient.OnConnectListener += OnRealTimeConnect;
            _realTimeClient.OnReceiveListener += OnRealTimeReceive;

            _realTimeClient.OnConnectListener += OnRealTimeConnectListener;
            _realTimeClient.Connect(ip, port, connId);
        }

        private void OnRealTimeConnect(IVNetPeer peer)
        {
            if (null != _lastConnection)
            {
                _lastConnection.RealTimePeer = peer;
            }
        }

        protected override AbstractNetowrkChannel GetChannel(IVNetPeer peer, MemoryStream stream)
        {
            return _lastConnection;
        }

        public void SendReliable(byte[] bytes, int length, int offset)
        {
            if (null != _lastConnection)
            {
                _lastConnection.SendRawDataReliable(bytes, length, offset);
            }
        }

        public void SendRealTime(byte[] bytes, int length, int offset)
        {
            if (null != _lastConnection)
            {
                _lastConnection.SendRawDataRealTime(bytes, length, offset);
            }
        }

        protected override void OnChannelConnected(VNetworkChannel channel)
        {
            channel.SetConnecter(this);
        }

        public virtual void ReConnect()
        {
            if (!_reliableClient.IsConnected) {
                _reliableClient.ReConnect();
            }
            if (!_realTimeClient.IsConnected) {
                _realTimeClient.ReConnect();
            }
        }

        private void CloseTcp()
        {
            if (null != _reliableClient && _reliableClient.IsConnected) {
                _reliableClient.CloseConnect();
            }
        }

        private void CloseUdp()
        {
            if (null != _realTimeClient && _realTimeClient.IsConnected) {
                _realTimeClient.CloseConnect();
            }
        }

        public virtual void CloseConnect(ProtocolType protocolType)
        {
            switch (protocolType) {
                case ProtocolType.Tcp:
                    CloseTcp();
                    break;
                case ProtocolType.Udp:
                    CloseUdp();
                    break;
                case ProtocolType.Unspecified:
                    CloseTcp();
                    CloseUdp();
                    break;
            }
        }
    }
}