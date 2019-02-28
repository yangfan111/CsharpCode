using System;
using System.Diagnostics;
using System.Net.Sockets;
using App.Server.Scripts.Config;
using App.Shared;
using Core.Network;
using Core.Utils;
using VNet;
using RpcNetwork.RpcNetwork;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Utils.Singleton;

namespace App.Server
{
  /*  public class AllocationClient : IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AllocationClient));
        private INetworkClient _client;
        private INetworkChannel _channel;
        private INetworkMessageSerializer _defaultSerializer;
        private IRpcNetworkMessageManager _rpcMgr = new RpcNetworkMessageManager();
        private NetworkPortInfo _networkPortInfo;
        private INetworkClientFactory _clientFactory = new VNetworkClientFactory();

        private Stopwatch _stopwatch;
        private int _checkReconnectTime = 15000;
        private long _lastConnectTime;
        private int _checkHeartBeatTime = 1000;
        private long _lastHeartBeatTime;

        public static AllocationClient Instance = new AllocationClient();
        private AllocationClient()
        {
            _stopwatch = Stopwatch.StartNew();
            _defaultSerializer = _rpcMgr.GetRpcMessageSerializer();
            RegisterCommand();
        }

        private void RegisterCommand()
        {

        }

        public void Connect(NetworkPortInfo networkPortInfo)
        {
            try
            {
                _networkPortInfo = networkPortInfo;
                Doconnect();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("HallRoom Server start failed {0}", e);
            }
        }

        private void Doconnect()
        {
            _lastConnectTime = _stopwatch.ElapsedMilliseconds;
            _client = _clientFactory.CreateTcpNetworkClient(SharedConfig.IsLittleEndian,"Allocation");
            _client.ChannelConnected += ClientOnChannelConnected;
            _client.ChannelDisconnected += ClientOnChannelDisConnected;
            _client.Connect(SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.AllocationServer.ConnectIp, _networkPortInfo, SharedConfig.MutilThread);
        }

        private void CheckReconnect()
        {
            if ((_client == null || !_client.IsConnected)
                && _stopwatch.ElapsedMilliseconds - _lastConnectTime > _checkReconnectTime)
            {
                Doconnect();
            }
        }

        private void CheckHeartBeat()
        {
            if (_channel != null && _stopwatch.ElapsedMilliseconds - _lastHeartBeatTime > _checkHeartBeatTime)
            {
                var msg = HeartBeatMessage.Allocate();
                msg.Id = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Id;
                _rpcMgr.Send(_channel, RpcMessageType.BattleServerHeartBeat, msg);
                msg.ReleaseReference();
                _lastHeartBeatTime = _stopwatch.ElapsedMilliseconds;
            }
        }

        private void ClientOnChannelDisConnected(INetworkChannel networkChannel)
        {
            DisposeConnect();
        }

        private void ClientOnChannelConnected(INetworkChannel networkChannel)
        {
            if (networkChannel.ErrorCode == SocketError.Success)
            {
                networkChannel.Serializer = _defaultSerializer;
                networkChannel.MessageReceived += _rpcMgr.NetworkChannelOnMessageReceived;
                RegistBattleServer(networkChannel);
            }
            else
            {
                ClientOnChannelDisConnected(networkChannel);
            }
        }

        private void RegistBattleServer(INetworkChannel networkChannel)
        {
            _channel = networkChannel;
            var msg = RegisterBattleServerMessage.Allocate();
            msg.Id = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Id;
            msg.Ip = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Ip;
            msg.GameMapId = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.MapId;
            msg.RoomPort = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.HallRoomServer.ListenPort;
            msg.BattleTcpPort = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.TcpPort;
            msg.BattleUdpPort = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.UdpPort;
            _rpcMgr.Send(networkChannel, RpcMessageType.RegisterBattleServer, msg);
            msg.ReleaseReference();
            //空闲状态
//            UpdateBattleServerStatus(0);
        }

        public void UpdateBattleServerStatus(int status)
        {
            if (_channel != null)
            {
                var msg = BattleServerStatusMessage.Allocate();
                msg.Status = status;
                _rpcMgr.Send(_channel, RpcMessageType.UpdateBattleServerStatus, msg);
                msg.ReleaseReference();
                _logger.InfoFormat("UpdateBattleServerStatus...{0}", status);
            }
        }

        private void DisposeConnect()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
            if (_channel != null)
            {
                _channel.Disconnect();
                _channel.Dispose();
                _channel = null;
            }
        }

        public bool IsValid()
        {
            return _client != null && _client.IsConnected;
        }

        public void Dispose()
        {
            DisposeConnect();
        }

        public void Update()
        {
            CheckReconnect();
            CheckHeartBeat();
            _rpcMgr.DriveDispatch();
            if (_client != null)
            {
                _client.Update();
            }
        }
    }*/
}
