using App.Server.MessageHandler.Hall;
using App.Server.Scripts.Config;
using App.Shared;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Network;
using Core.ObjectPool;
using Core.Room;
using Core.Utils;
using Google.Protobuf;
using RpcNetwork.RpcNetwork;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using Utils.Singleton;
using VNet;

namespace App.Server
{
    public class HallServerNetwork
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HallServerNetwork));

        private class HallServerNetStatistic
        {
            public bool IsGameOverMandateLogOutSend;
            public bool IsGameOverMsgSend;

            public void Reset()
            {
                IsGameOverMandateLogOutSend = false;
                IsGameOverMsgSend = false;
            }
        }

        private INetworkClient _client;
        private INetworkChannel _clientChannel;
        private INetworkServer _server;
        private INetworkChannel _serverChannel;
        private IRpcNetworkMessageManager _rpcMgr = new RpcNetworkMessageManager();
        private NetworkPortInfo _networkPortInfo;
        private bool _clientOpen = false;
        private INetworkClientFactory _clientFactory = new VNetworkClientFactory();

        private Stopwatch _stopwatch;
        private int _checkReconnectTime = 15000;
        private long _lastConnectTime;
        private int _checkHeartBeatTime = 1000;
        private long _lastHeartBeatTime;

        private RoomEventDispatcher _dispatcher;

        private HallServerNetStatistic _statistic = new HallServerNetStatistic();

        public HallServerNetwork(RoomEventDispatcher dispatcher)
        {
            _stopwatch = Stopwatch.StartNew();
            _dispatcher = dispatcher;
            _dispatcher.Intercept += InterceptRoomEvent;
            _dispatcher.OnRoomEvent += OnRoomEvent;
            _client = _clientFactory.CreateTcpNetworkClient(SharedConfig.IsLittleEndian, SharedConfig.MutilThread, "Allocation");
            _client.ChannelConnected += ClientOnChannelConnected;
            _client.ChannelDisconnected += ClientOnChannelDisConnected;
            RegisterCommand();
        }

        private void RegisterCommand()
        {
            _rpcMgr.RegisterLater(RpcMessageType.RequestCreateRoom, new RequestCreateRoomMessageHandler(_dispatcher));
            _rpcMgr.RegisterLater(RpcMessageType.RequestJoinRoom, new RequestJoinRoomMessageHandler(_dispatcher));
            _rpcMgr.RegisterLater(RpcMessageType.RequestJoinRoomList, new RequestJoinRoomListMessageHandler(_dispatcher));
            _rpcMgr.RegisterLater(RpcMessageType.ResponseRegisterBattleServer, new ResponseRegisterBattleServerMessageHandler(_dispatcher));
        }

        private void InterceptRoomEvent(RoomEventArg eventArg)
        {
            var e = eventArg.Event;
            switch (e.EventType)
            {
                case ERoomEventType.MandatoryLogOut:
                    var filtered = FilterMandatoryLogOutEvent(e as MandatoryLogOutEvent);
                    eventArg.Filtered = filtered;
                    break;
            }
        }

        private bool FilterMandatoryLogOutEvent(MandatoryLogOutEvent e)
        {
            if (e.LogOutReason == MandatoryLogOutEvent.Reason.GameExit)
            {
                //dispose mandatory logout event on gameover if the correspoding message have been sent
                if (_statistic.IsGameOverMandateLogOutSend)
                {
                    e.IsDisposed = true;
                    return true;
                }

                //never send mandaotry logout message on gameover until the gameover message have been sent
                if (!_statistic.IsGameOverMsgSend)
                {
                    return true;
                }
            }

            return false;
        }

        #region HallServer

        public void StartServer(NetworkPortInfo networkPortInfo)
        {
            try
            {
                INetworkServerFactory serverFactory = new VNetworkServerFactory();
                _server = serverFactory.CreateTcpNetworkServer(SharedConfig.IsLittleEndian, "HallRoom");
                _server.ChannelConnected += ServerOnChannelConnected;
                _server.Listen(networkPortInfo, 1, SharedConfig.MutilThread);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("HallRoom Server start failed {0}", e);
            }
        }

        private void ServerOnChannelConnected(INetworkChannel networkChannel)
        {
            _serverChannel = networkChannel;
            networkChannel.Serializer = new NetworkMessageSerializer(new AppMessageTypeInfo(new ProtoBufSerializeInfo<Apc>(Apc.Parser)));
            networkChannel.MessageReceived += _rpcMgr.NetworkChannelOnMessageReceived;
            SendHallClientConnectedResposne();
            _logger.InfoFormat("Connect ... Hall Room Server Local {0} Remote {1} Connected {2}", 
                networkChannel.LocalConnId, networkChannel.RemoteConnId, networkChannel.IsConnected);
        }

        public bool IsServerValid()
        {
            return _server != null;
        }

        private void DisposeServer()
        {
            DisposeChannel(_serverChannel);
            _serverChannel = null;

            if (_server != null)
            {
                _server.Stop();
                _server.Dispose();
            }
        }

        #endregion


        #region AllocationClient
        public void ClientConnect(NetworkPortInfo networkPortInfo)
        {
            try
            {
                _clientOpen = true;
                _networkPortInfo = networkPortInfo;
                DoClientConnect();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("HallRoom Server start failed {0}", e);
            }
        }

        private void DoClientConnect()
        {
            _lastConnectTime = _stopwatch.ElapsedMilliseconds;

            var connectIp = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.AllocationServer.ConnectIp;
            _logger.InfoFormat("Connection to Allocation Server {0}  HearBeatMessage HashCode {1}", connectIp, RpcMessageType.ServerHeartBeat.GetHashCode());
            _client.Connect(connectIp, _networkPortInfo);
        }

        private void CheckClientReconnect()
        {
            if (_clientOpen && (_client == null || !_client.IsConnected)
                && _stopwatch.ElapsedMilliseconds - _lastConnectTime > _checkReconnectTime)
            {
                _logger.Info("Reconnect to Allocation Server...");
                DoClientConnect();
            }
        }

        private void CheckHeartBeat()
        {
            if (_clientChannel != null && _stopwatch.ElapsedMilliseconds - _lastHeartBeatTime > _checkHeartBeatTime)
            {
                var msg = HeartBeatMessage.Allocate();
                msg.Id = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Id;
                SendAndReleaseMessage(_clientChannel, RpcMessageType.ServerHeartBeat, msg);
                _lastHeartBeatTime = _stopwatch.ElapsedMilliseconds;
            }
        }

        private void ClientOnChannelDisConnected(INetworkChannel networkChannel)
        {
            _logger.ErrorFormat("Disconnect from Allocation Server");
            DisposeClientConnect();
        }

        private void ClientOnChannelConnected(INetworkChannel networkChannel)
        {
            _logger.InfoFormat("Allocation Server Connected Result {0}", networkChannel.ErrorCode);
            if (networkChannel.ErrorCode == SocketError.Success)
            {
                networkChannel.Serializer = new NetworkMessageSerializer(new AppMessageTypeInfo(new ProtoBufSerializeInfo<Apc>(Apc.Parser)));
                networkChannel.MessageReceived += _rpcMgr.NetworkChannelOnMessageReceived;
                RegistBattleServer(networkChannel);
            }
            else
            {
                ClientOnChannelDisConnected(networkChannel);

                _logger.InfoFormat("Disconnect from allocation server");
            }
        }

        private void RegistBattleServer(INetworkChannel networkChannel)
        {
            _clientChannel = networkChannel;
            var msg = RegisterBattleServerMessage.Allocate();
            msg.Id = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Id;
            msg.Ip = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Ip;
            msg.GameMapId = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.MapId;
            msg.RoomPort = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.HallRoomServer.ListenPort;
            msg.BattleTcpPort = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.TcpPort;
            msg.BattleUdpPort = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.UdpPort;

            _logger.InfoFormat("Register  Battle Server, Id {0} Ip {1} GameMap Id {2} Room Port {3}, Tcp {4} Udp {5}",
                msg.Id, msg.Ip, msg.GameMapId, msg.RoomPort, msg.BattleTcpPort, msg.BattleUdpPort);

            SendRegisterBattleServerMessage(msg);
            //空闲状态
            //            UpdateBattleServerStatus(0);
        }

  
        private void DisposeClientConnect()
        {
            DisposeChannel(_clientChannel);
            _clientChannel = null;

            var evt = RoomEvent.AllocEvent<HallServerDisconnectEvent>();
            _dispatcher.AddEvent(evt);

            _logger.ErrorFormat("Disconnect From Allocation Server.");
        }

        public bool IsClientValid()
        {
            return _client != null && _client.IsConnected;
        }


        #endregion

        private void DisposeChannel(INetworkChannel channel)
        {
            if (channel != null)
            {
                channel.Dispose();
            }
            if (channel != null)
            {
                channel.Disconnect();
                channel.Dispose();
            }
        }

        public void Dispose()
        {
            DisposeServer();
            DisposeClientConnect();
        }

        public void Update()
        {
            CheckClientReconnect();
            CheckHeartBeat();
            _rpcMgr.DriveDispatch();
            if (_client != null)
            {
                _client.Update();
            }

            _server.Update();
        }

        #region EventProcessor
        public void OnRoomEvent(RoomEvent e)
        {
            switch (e.EventType)
            {
                case ERoomEventType.UpdatePlayerStatus:
                    SendPlayerStatus(e as UpdatePlayerStatusEvent);
                    break;
                case ERoomEventType.UpdateRoomStatus:
                    SendRoomGameStatus(e as UpdateRoomGameStatusEvent);
                    break;
                case ERoomEventType.CreateRoomResponse:
                    SendCreateRoomResponse(e as CreateRoomResponseEvent);
                    break;
                case ERoomEventType.JoinRoomResponse:
                    SendJoinRoomResponse(e as JoinRoomResponseEvent);
                    break;
                case ERoomEventType.JoinRoomListResponse:
                    SendJoinRoomListResponse(e as JoinRoomListResponseEvent);
                    break;
                case ERoomEventType.LeaveRoom:
                    SendLeaveRoom(e as LeaveRoomEvent);
                    break;
                case ERoomEventType.GameOver:
                    SendGameOverMessage(e as GameOverEvent);
                    break;
                case ERoomEventType.RoomDisposed:
                    SendServerStatusOnRoomDisposed();
                    break;
                case ERoomEventType.MandatoryLogOut:
                    SendMandateLogOutMessage(e as MandatoryLogOutEvent);
                    break;
            }
        }

        private void SendCreateRoomResponse(CreateRoomResponseEvent e)
        {
            if (e.Success)
                SendCreateRoomSuccess(e.Message);
            else
                SendCreateRoomFail(e.ErrCode);

        }

        private void SendHallClientConnectedResposne()
        {
            var msg = ResponseHallClientConnectMessage.Allocate();
            msg.RetCode = 0;
            SendAndReleaseMessage(RpcMessageType.ResponseHallClientConnect, msg);

        }

        private void SendCreateRoomSuccess(ResponseCreateRoomMessage message)
        {
            _logger.InfoFormat("Create Room Successfully, HallRoomId {0}!", message.HallRoomId);
            SendAndReleaseMessage(RpcMessageType.ResponseCreateRoom, message);
            SendServerStatus(1);

            _statistic.Reset();
        }

        private void SendCreateRoomFail(ErrorCode retCode)
        {
            _logger.ErrorFormat("CreateRoom Error ... Code:{0}", retCode);
            var resMsg = ResponseCreateRoomMessage.Allocate();
            resMsg.RetCode = (int)retCode;
            SendAndReleaseMessage(RpcMessageType.ResponseCreateRoom, resMsg);
        }

        private void SendJoinRoomResponse(JoinRoomResponseEvent e)
        {
            if (e.JoinRoomResponseInfo.Success)
            {
                SendJoinRoomSuccess(e);
            }
            else
            {
                SendJoinRoomFail(e);
            }
        }

        private void SendJoinRoomListResponse(JoinRoomListResponseEvent e)
        {
            var msg = ResponseJoinRoomListMessage.Allocate();
            msg.HallRoomId = e.HallRoomId;
            msg.RetCode = e.RetCode;
            foreach (var info in e.JoinRoomResponseInfoList)
            {
                var player = JoinPlayer.Allocate();
                player.RetCode = (int)info.ErrCode;
                player.Id = info.PlayerId;
                player.Token = info.Token;
                msg.Players.Add(player);
            }

            SendAndReleaseMessage(_serverChannel, RpcMessageType.ResponseJoinRoomList, msg);
        }

        private void SendRegisterBattleServerMessage(RegisterBattleServerMessage msg)
        {
            SendAndReleaseMessage(_clientChannel, RpcMessageType.RegisterBattleServer, msg);
        }

        private void SendServerStatus(int status)
        {
            var msg = BattleServerStatusMessage.Allocate();
            msg.Status = status;
            SendAndReleaseMessage(_clientChannel, RpcMessageType.UpdateBattleServerStatus, msg);
            _logger.InfoFormat("UpdateBattleServerStatus...{0}", status);
        }

        private void SendJoinRoomSuccess(JoinRoomResponseEvent e)
        {
            _logger.InfoFormat("JoinRoom Successfully Player Id {0} Token {1}", e.JoinRoomResponseInfo.PlayerId, e.JoinRoomResponseInfo.Token);
            var loginPlayer = LoginPlayer.Allocate();
            loginPlayer.Id = e.JoinRoomResponseInfo.PlayerId;
            loginPlayer.Token = e.JoinRoomResponseInfo.Token;

            var rCode = IntData.Allocate();
            rCode.Value = e.RetCode;
            var rHallRoomId = LongData.Allocate();
            rHallRoomId.Value = e.HallRoomId;

            SendAndReleaseMessage(RpcMessageType.ResponseJoinRoom, rCode, rHallRoomId, loginPlayer);
        }

        private void SendJoinRoomFail(JoinRoomResponseEvent e)
        {
            _logger.ErrorFormat("JoinRoom Error ... Code:{0}", e.JoinRoomResponseInfo.ErrCode);
            var rCode = IntData.Allocate();
            rCode.Value = (int) e.JoinRoomResponseInfo.ErrCode;
            var rHallRoomId = LongData.Allocate();
            rHallRoomId.Value = e.HallRoomId;
            var loginPlayer = LoginPlayer.Allocate();
            loginPlayer.Id = e.JoinRoomResponseInfo.PlayerId;
            loginPlayer.Token = "";
            SendAndReleaseMessage(RpcMessageType.ResponseJoinRoom, rCode, rHallRoomId, loginPlayer);
        }
        
        private void SendRoomGameStatus(UpdateRoomGameStatusEvent e)
        {
            var hallRoomId = LongData.Allocate();
            hallRoomId.Value = e.HallRoomId;
            var rStatus = IntData.Allocate();
            rStatus.Value = e.Status;

            var canEnter = IntData.Allocate();
            canEnter.Value = e.CanEnter;
            SendAndReleaseMessage(RpcMessageType.UpdateRoomGameState, hallRoomId, rStatus, canEnter);
        }

        private void SendPlayerStatus(UpdatePlayerStatusEvent e)
        {
            var msg = SyncPlayerStatusMessage.Allocate();
            msg.Id = e.PlayerId;
            msg.Status = e.Status;

            SendAndReleaseMessage(RpcMessageType.SyncPlayerStatus, msg);
        }

        private void SendLeaveRoom(LeaveRoomEvent e)
        {
            _logger.InfoFormat("Leave Room Player Id {0}", e.PlayerId);
            var msg = SingleGameOverMessage.Allocate();
            msg.HallRoomId = e.PlayerId;
            msg.Players = e.Player;
            msg.ShowReportFlag = e.Player.Id != 0L ? 1 : 0;
            SendAndReleaseMessage(RpcMessageType.SingleGameOver, msg);
        }

        private void SendGameOverMessage(GameOverEvent e)
        {
            SendAndReleaseMessage(RpcMessageType.GameOver, e.Message);

            _statistic.IsGameOverMsgSend = true;
            _logger.InfoFormat("Send GameOver Message to Hall Server");
        }

        private void SendServerStatusOnRoomDisposed()
        {
            SendServerStatus(0);
        }

        private void SendMandateLogOutMessage(MandatoryLogOutEvent e)
        {
            _logger.InfoFormat("Mandatory Logout Message HallRoom Id {0} Room Id {1} PlayerEntityId {2}", e.HallRoomId, e.RoomId,  e.PlayerId);

            LongData hallRoomId = LongData.Allocate();
            hallRoomId.Value = e.HallRoomId;
            IntData roomId = IntData.Allocate();
            roomId.Value = e.RoomId;
            LongData playerId = LongData.Allocate();
            playerId.Value = e.PlayerId;

            SendAndReleaseMessage(RpcMessageType.MandateLogOut, hallRoomId, roomId, playerId);

            if (e.LogOutReason == MandatoryLogOutEvent.Reason.GameExit)
            {
                _statistic.IsGameOverMandateLogOutSend = true;
            }
        }

        private void SendAndReleaseMessage( string messageName, params IMessage[] messageBodys)
        {
            SendAndReleaseMessage(_serverChannel, messageName, messageBodys);
        }

        private bool EnableSendLogging(string messageName)
        {
            if (RpcMessageType.ServerHeartBeat.Equals(messageName))
            {
                return false;
            }

            return true;
        }

        public void SendAndReleaseMessage(INetworkChannel channel, string messageName, params IMessage[] messageBodys)
        {
            if (channel != null && channel.IsConnected)
            {
                _rpcMgr.Send(channel, EnableSendLogging(messageName), messageName, messageBodys);
            }
            foreach (var msg in messageBodys)
            {
                if (msg is BaseRefCounter)
                    ((BaseRefCounter)msg).ReleaseReference();
            }
        }


    #endregion
    }
}
