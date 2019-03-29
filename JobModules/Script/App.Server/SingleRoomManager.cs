using Core.GameTime;
using Core.Network;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.DebugSystem;
using App.Shared;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Room;
using UnityEngine;
using Utils.Singleton;

namespace App.Server
{
    public class SingleRoomManager : IRoomManager, IServerDebugInfoAccessor
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SingleRoomManager));

 
        public ServerDebugInfo GetDebugInfo()
        {
            var debugInfo = new  ServerDebugInfo();
            if (_room != null)
            {
                var serverRoom = _room as ServerRoom;
                debugInfo.RoomDebugInfo = serverRoom.GetRoomDebugInfo();
                debugInfo.RoomDebugInfo.HasHallServer = _hasHallServer;
                debugInfo.PlayerDebugInfo = serverRoom.GetPlayerDebugInfo();
            }

            return debugInfo;
        }

        private IRoom _room;
        private bool _isGameOverMsgSend;
        private PlayerInfoManager _playerInfoManager;
        private RoomEventDispatcher _dispatcher;
        private ServerRoomFactory _roomFactory;
        private IRoomListener _roomListener;
        private bool _hasHallServer;

        private ServerDebugInfoSystem _debugSystem;

        //private Stopwatch _stopwatch;

        public SingleRoomManager(ServerRoomFactory roomFactory, RoomEventDispatcher dispatcher, IRoomListener roomListener) : this(roomFactory, dispatcher, true, roomListener)
        {

        }
        public SingleRoomManager(ServerRoomFactory roomFactory, RoomEventDispatcher dispatcher, bool tokenOnce, IRoomListener roomListener)
        {
            _dispatcher = dispatcher;
            _dispatcher.OnRoomEvent += OnRoomEvent;
            _playerInfoManager = new PlayerInfoManager(tokenOnce);

            _roomFactory = roomFactory;
            roomFactory.TokenGenerator = _playerInfoManager;

            _roomListener = roomListener;

            _debugSystem = new ServerDebugInfoSystem(this);
        }

        private void OnRoomEvent(RoomEvent e)
        {
            switch (e.EventType)
            {
                case ERoomEventType.HallServerConnect:
                    _hasHallServer = true;
                    break;
                case ERoomEventType.CreateRoom:
                    CreateRoom(e as CreateRoomEvent);
                    break;
                case ERoomEventType.JoinRoom:
                    JoinRoom(e as JoinRoomEvent);
                    break;
                case ERoomEventType.JointRoomList:
                    JoinRoomList(e as JoinRoomListEvent);
                    break;
                case ERoomEventType.LeaveRoom:
                    OnLeaveRoom(e as LeaveRoomEvent);
                    break;
                case ERoomEventType.GameExit:
                    OnGameExit(e as GameExitEvent);
                    break;
                case ERoomEventType.MandatoryLogOut:
                    OnMandatoryLogOut(e as MandatoryLogOutEvent);
                    break;
                case ERoomEventType.HallServerDisconnect:
                    _hasHallServer = false;
                    break;
            }
        }

        //In the absence of connection to Hall Server, we use a dummy CreateRoomEvent to create  a  new room.
        //Usually, it means the game is running in Edit Mode. 
        public void CreateRoomIfNotExist()
        {
            if (_room == null && !_hasHallServer)
            {
                var e = RoomEvent.AllocEvent<CreateRoomEvent>();
                e.IsDummy = true;
                e.Message = null;
                CreateRoom(e);
                RoomEvent.FreeEvent(e);
            }
        }

        private void CreateRoom(CreateRoomEvent e)
        {
            if (_room != null || (!e.IsDummy  && e.Message == null))
            {
                var evt = RoomEvent.AllocEvent<CreateRoomResponseEvent>();
                evt.Success = false;
                evt.ErrCode = _room != null ? ErrorCode.CreateRoom_ServerRoom_Exist : ErrorCode.CreateRoom_Message_Error;
                _dispatcher.AddEvent(evt);
            }
            else
            {
                _room = _roomFactory.Create(e.Message);
                FinishRoomCreate(e.Message);
            }
        }

        private void FinishRoomCreate(RequestCreateRoomMessage message)
        {
            CreateTestPlayer();
            SendCreateRoomResponse(message);
            _room.Start();
            if(_roomListener != null)
                _roomListener.OnRoomCreated(_room);
            _isGameOverMsgSend = false;
            _logger.InfoFormat("CreateRoom Successfully");
        }

        private void CreateTestPlayer()
        {
            //roomManager.AddRoom(r);
            AddPlayerInfo(TestUtility.TestToken, TestUtility.CreateTestPlayer(_room.RoomId));
            AddPlayerInfo(TestUtility.RobotToken, TestUtility.CreateTestPlayer(_room.RoomId));
        }

        private void SendCreateRoomResponse(RequestCreateRoomMessage message)
        {
            if (message == null)
                return;

            var resMsg = ResponseCreateRoomMessage.Allocate();
            resMsg.RetCode = 0; //Success
            resMsg.HallRoomId = message.HallRoomId;

            var evt = RoomEvent.AllocEvent<CreateRoomResponseEvent>();
            evt.Success = true;
            evt.Message = resMsg;
            evt.RoomId = _room.RoomId.Id;

            _dispatcher.AddEvent(evt);
        }

        private void JoinRoom(JoinRoomEvent e)
        {
            var evt = RoomEvent.AllocEvent<JoinRoomResponseEvent>();
            evt.HallRoomId = e.HallRoomId;
            evt.RetCode = 0;
            evt.JoinRoomResponseInfo =  JoinRoom(e.HallRoomId, e.RoomPlayer);
            
            _dispatcher.AddEvent(evt);
        }

        private void JoinRoomList(JoinRoomListEvent e)
        {
            if (e.RoomPlayerList != null)
            {
                var evt = RoomEvent.AllocEvent<JoinRoomListResponseEvent>();
                evt.HallRoomId = e.HallRoomId;
                evt.RetCode = 0;
                foreach (var roomPlayer in e.RoomPlayerList)
                {
                    evt.JoinRoomResponseInfoList.Add(JoinRoom(e.HallRoomId, roomPlayer));
                }

                _dispatcher.AddEvent(evt);
            }
            
        }

        private JoinRoomResponseInfo JoinRoom(long hallRoomId, RoomPlayer roomPlayer)
        {
            var  responseInfo = new JoinRoomResponseInfo();

            if (roomPlayer == null)
            {
                responseInfo.ErrCode = ErrorCode.JoinRoom_Message_Error;
            }
            else if (_room == null || _room.IsDiposed)
            {
                responseInfo.ErrCode = ErrorCode.JoinRoom_ServerRoom_Null;
            }
            else
            {
                int errCode = 0;
                var playerInfo = _room.PlayerJoin(hallRoomId, roomPlayer, out errCode);
                if (playerInfo != null)
                {
                    responseInfo.Success = true;
                    responseInfo.Token = playerInfo.Token;
                    responseInfo.ErrCode = ErrorCode.None;
                    _playerInfoManager.AddPlayerInfo(playerInfo.Token, playerInfo);

                    _logger.InfoFormat("Player({0}), Id:{1}, Name:{2}, ModelId:{3}, TeamId:{4}, Token:{5}", 0,
                        roomPlayer.Id, roomPlayer.Name, roomPlayer.RoleModelId, roomPlayer.TeamId, playerInfo.Token);
                }
                else
                {
                    responseInfo.ErrCode = (ErrorCode)errCode;
                }
            }
            
            responseInfo.PlayerId = roomPlayer != null ? roomPlayer.Id : 0;
            return responseInfo;
        }

        private void OnLeaveRoom(LeaveRoomEvent e)
        {
            var token = e.Token;
            _playerInfoManager.RemovePlayerInfo(token);

            _logger.InfoFormat("Remove Player Info {0}", token);
        }

        private void OnGameExit(GameExitEvent e)
        {
            _playerInfoManager.Clear();
            SingletonManager.Dispose();

            if (_roomListener != null)
                _roomListener.OnRoomDispose(_room);

            _room.Dispose();
        }

        private void OnMandatoryLogOut(MandatoryLogOutEvent e)
        {
            if(e.PlayerId != 0 && e.Token != null)
                _playerInfoManager.RemovePlayerInfo(e.Token);
        }

        public IRoom GetRoom(int roomId)
        {
            if (_room != null)
            {
                return _room.RoomId.Id == roomId ? _room : null;
            }

            return null;
        }

        private CalcFixTimeInterval _calcFixTimeInterval = new CalcFixTimeInterval();
        public void Update()
        {
            if (_room != null)
            {
                if (_room.IsDiposed)
                {
                    _room = null;

                    var evt = RoomEvent.AllocEvent<RoomDisposedEvent>();
                    _dispatcher.AddEvent(evt);
                }
                else
                {
                    float now = Time.time * 1000;
                    int interval = _calcFixTimeInterval.Update(now);
                    _room.Update(interval);
                }
            }
         
            _debugSystem.Update();
        }

        public void LateUpdate()
        {
            if(_room  != null)
            {
                _room.LateUpdate();
            }
        }

        public void SetPlayerStageRunning(string messageToken, INetworkChannel channel)
        {
            var playerInfo = GetPlayerInfo(messageToken, true);
            if (playerInfo != null)
                _room.SetPlayerStageRunning(playerInfo, channel);
        }

        public bool RequestRoomInfo(string messageToken, INetworkChannel channel)
        {
            var playerInfo = GetPlayerInfo(messageToken, true);
            if (playerInfo != null)
                return _room.SendLoginSucc(playerInfo, channel);

            return false;
        }


        public bool RequestPlayerInfo(string messageToken, INetworkChannel channel)
        {

            var playerInfo = GetPlayerInfo(messageToken, false);
            if (playerInfo != null)
            {
                bool sucess = _room.LoginPlayer(playerInfo, channel);
                if (!_hasHallServer && sucess)
                {
                    var evt = RoomEvent.AllocEvent<PlayerLoginEvent>();
                    evt.PlayerInfo = playerInfo;
                    _dispatcher.AddEvent(evt);
                }
                return sucess;
            }

            return false;
        }

        private IPlayerInfo GetPlayerInfo(string messageToken, bool isCheck)
        {
            if (_room == null)
            {
                _logger.InfoFormat("login failed, Room is null");
                return null;
            }

            return _playerInfoManager.GetPlayerInfo(_room.RoomId, messageToken, isCheck);
        }

        public void AddPlayerInfo(string token, IPlayerInfo playerInfo)
        {
            _playerInfoManager.AddPlayerInfo(token, playerInfo);
        }

        public void RemovePlayerInfo(string token)
        {
            _playerInfoManager.RemovePlayerInfo(token);
        }

        public bool HasPlayerInfo(string token)
        {
            return _playerInfoManager.HasPlayerInfo(token);
        }

        public List<IPlayerInfo> GetRobotPlayerInfos()
        {
            return _playerInfoManager.GetRobotPlayerInfos();
        }

        public void AddRoom(IRoom room)
        {
            throw new NotImplementedException();
        }

        public IRoom GetNewRoom()
        {
            throw new NotImplementedException();
        }
    }
}
