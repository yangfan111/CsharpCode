using System;
using Core.Network;
using Core.Network.ENet;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using VNet;

namespace App.Shared.Client
{
    public class LoginClient : IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(LoginClient));
        private INetworkChannel _networkChannel;
        private IClientRoom _clientRoom;
        private INetworkClient _networkClient;

        public LoginClient(
            string serverIp,
            NetworkPortInfo networkPortInfo,
            IClientRoom clientRoom)
        {
            this._clientRoom = clientRoom;

            //INetworkClientFactory clientFactory = new UNetNetworkClientFactory();
            //INetworkClientFactory clientFactory = new ENetNetworkClientFactory();
            INetworkClientFactory clientFactory = new VNetworkClientFactory();
            if (_networkClient != null) _networkChannel.Dispose();
            _networkClient = clientFactory.CreateNetworkClient(SharedConfig.IsLittleEndian, SharedConfig.MutilThread,
                "BattleClient");

            _networkClient.ChannelConnected += NetworkClientOnChannelConnected;
            _networkClient.ChannelDisconnected += NetworkClientOnChannelDisconnected;
            _networkClient.Connect(serverIp, networkPortInfo);
            SingletonManager.Get<DurationHelp>().ServerInfo = string.Format("{0} tcp:{1} udp:{2}", serverIp,
                networkPortInfo.TcpPort, networkPortInfo.UdpPort);
        }


        private void NetworkClientOnChannelConnected(INetworkChannel networkChannel)
        {
            this._networkChannel = networkChannel;

            _clientRoom.OnNetworkConnected(networkChannel);
            _logger.InfoFormat("LoginClient connected {0}", networkChannel.ToString());
        }


        private void NetworkClientOnChannelDisconnected(INetworkChannel networkChannel)
        {
            _clientRoom.OnNetworkDisconnected();
        }


        public void Dispose()
        {
            if (_networkChannel != null)
            {
                _networkChannel.Disconnect();
                ;
            }

            _networkClient.Dispose();
        }


        public bool Connected
        {
            get { return _networkClient.IsConnected; }
        }

        public void Update()
        {
            _networkClient.Update();
        }

        public void FlowTick(float time)
        {
            _networkClient.FlowTick(time);
        }
    }
}