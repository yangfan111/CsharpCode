using System;
using App.Protobuf;
using App.Shared;
using App.Shared.Network;
using Core.Network;
using Core.Network.ENet;
using Core.Room;
using Core.Utils;
using VNet;
using App.Server.Scripts.Config;
using UnityEngine;
using Version = Core.Utils.Version;
using Utils.Singleton;

namespace App.Server 
{
    public class LoginServer:IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(LoginServer));
        private IRoomManager _roomManager;
        private RoomEventDispatcher _dispatcher;
        private INetworkServer _server;
        private PlayerNetworkMonitor _networkMonitor;
        private INetworkMessageSerializer _defaultSerializer = new NetworkMessageSerializer(new AppMessageTypeInfo());
        public LoginServer(RoomEventDispatcher dispatcher, IRoomManager roomManager)
        {
            _dispatcher = dispatcher;
            _roomManager = roomManager;
            _networkMonitor = new PlayerNetworkMonitor(_dispatcher, _roomManager);
            _dispatcher.OnRoomEvent += OnRoomEvent;
        }

        public void Start(NetworkPortInfo networkPortInfo)
        {
            try
            {
                //INetworkServerFactory serverFactory = new UNetNetworkServerFactory();
                //INetworkServerFactory serverFactory = new EnetNetworkServerFactory();
                INetworkServerFactory serverFactory = new VNetworkServerFactory();
                _server = serverFactory.CreateNetworkServer(SharedConfig.IsLittleEndian,"LoginServer");
                _server.ChannelConnected += ServerOnChannelConnected;
                _server.Listen(networkPortInfo, SharedConfig.LoginServerThreadCount, SharedConfig.MutilThread);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Login Server start failed {0}", e);
            }
        }

        private void ServerOnChannelConnected(INetworkChannel networkChannel)
        {
            networkChannel.Serializer = _defaultSerializer;
            networkChannel.MessageReceived += NetworkChannelOnMessageReceived;
            var msg = UdpIdMessage.Allocate();
            msg.Id = networkChannel.LocalConnId;
            msg.Port = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.UdpPort;
            msg.ServerAsset = Version.Instance.LocalAsset;
            msg.ServerVersion = Version.Instance.LocalVersion;
            msg.ServerId = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Id;
            networkChannel.SendReliable((int)EServer2ClientMessage.UdpId, msg);
            msg.ReleaseReference();
        }

        private void NetworkChannelOnMessageReceived(INetworkChannel channel, int messageType, object messageBody)
        {
            
            if (messageType == (int) EClient2ServerMessage.Login)
            {
                LoginMessage message = (LoginMessage) messageBody;
                if (message == null)
                {
                    _logger.InfoFormat("Illegal login message");
                    channel.Disconnect();
                }
                else
                {
                    _logger.InfoFormat("Received Log in Message ... token:{0} {1}", message.Token, message.LoginStage);
                    var e = RoomEvent.AllocEvent<LoginServerEvent>();
                    e.Channel = channel;
                    e.Message = message;
                    _dispatcher.AddEvent(e);
                }
            }

            _networkMonitor.CheckStateOnReceivePlayerMessage();
        }

        private void OnRoomEvent(RoomEvent e)
        {
            switch (e.EventType)
            {
                case ERoomEventType.LoginServer:
                    ProcessLoginServerEvent(e as LoginServerEvent);
                    break;
            }
        }

        private void ProcessLoginServerEvent(LoginServerEvent e)
        {
            var channel = e.Channel;
            var message = e.Message;
            if (message.LoginStage == ELoginStage.PreLogin)
            {
                _roomManager.CreateRoomIfNotExist();

                if (!_roomManager.RequestRoomInfo(message.Token, channel))
                {
                    _logger.InfoFormat("illegal login token:{0}", message.Token);
                    channel.Disconnect();
                }
            }
            else if (message.LoginStage == ELoginStage.GetPlayerInfo)
            {
                if (!_roomManager.RequestPlayerInfo(message.Token, channel))
                {
                    _logger.InfoFormat("illegal login token:{0}", message.Token);
                    channel.Disconnect();
                }
            }
            else if (message.LoginStage == ELoginStage.RequestSnapshot)
            {
                _roomManager.SetPlayerStageRunning(message.Token, channel);
            }
        }

        public bool IsValid()
        {
            return _server != null;
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Stop();
                _server.Dispose();
            }
        }

        public void Update()
        {
            _networkMonitor.Update();
            _server.Update();
            _server.FlowTick(Time.time);
        }
    }
}
