using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Shared;
using Core.Network;
using Core.Room;
using Core.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace App.Server
{
    public class PlayerNetworkMonitor
    {
        private class NetworkState
        {
            private static readonly float MaxLoginWaitTime = 480.0f;

            public NetworkState(long playerId)
            {
                _initTime = Time.time;
                PlayerId = playerId;
            }

            private float _initTime;
            public long PlayerId;
            public bool IsLoginTimeOut(float currentTime)
            {
                return currentTime - _initTime > MaxLoginWaitTime;
            }
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerNetworkMonitor));

        private static readonly float MAX_HEARTBEAT_INTERVAL = 10;
        private float _lastHeartBeatTime = 0;
        private HashSet<INetworkChannel> _connectedChannels = new HashSet<INetworkChannel>();
        private RoomEventDispatcher _dispatcher;
        private IRoomManager _roomManager;

        private bool _hasHallServer;
        private long _hallRoomId;
        private int _roomId;

        private Dictionary<string, NetworkState> _loginStates = new Dictionary<string, NetworkState>();
        public PlayerNetworkMonitor(RoomEventDispatcher dispatcher, IRoomManager roomManager)
        {
            _dispatcher = dispatcher;
            _roomManager = roomManager;
            dispatcher.OnRoomEvent += OnRoomEvent;
        }

        private void OnRoomEvent(RoomEvent e)
        {
            switch (e.EventType)
            {
                case ERoomEventType.CreateRoomResponse:
                    OnCreateRoomResponse(e as CreateRoomResponseEvent);
                    break;
                case ERoomEventType.JoinRoomResponse:
                    OnJoinRoomResponse(e as JoinRoomResponseEvent);
                    break;
                case ERoomEventType.JoinRoomListResponse:
                    OnJoinRoomListResponse(e as JoinRoomListResponseEvent);
                    break;
                case ERoomEventType.LoginServer:
                    OnLoginServer(e as LoginServerEvent);
                    break;
                case ERoomEventType.HallServerConnect:
                    _hasHallServer = true;
                    break;
                case ERoomEventType.HallServerDisconnect:
                    _hasHallServer = false;
                    break;
            }
        }

        private void OnCreateRoomResponse(CreateRoomResponseEvent e)
        {
            if (e.Success && _hasHallServer)
            {
                _hallRoomId = e.Message.HallRoomId;
                _roomId = e.RoomId;

                var players = e.Message.Players;
                foreach (var player in players)
                {
                    if(player.Token != null)
                        _loginStates[player.Token] = new NetworkState(player.Id);
                    else
                        _logger.ErrorFormat("The token for player {0} on creating room is null", player.Id);
                }
            }
        }

        private void OnJoinRoomResponse(JoinRoomResponseEvent e)
        {
            OnJoinRoom(e.JoinRoomResponseInfo);
        }

        private void OnJoinRoomListResponse(JoinRoomListResponseEvent e)
        {
            foreach (var info in e.JoinRoomResponseInfoList)
            {
                OnJoinRoom(info);
            }
        }

        private void OnJoinRoom(JoinRoomResponseInfo info)
        {
            if (info.Success && _hasHallServer)
            {
                if (info.Token != null)
                    _loginStates[info.Token] = new NetworkState(info.PlayerId);
                else
                    _logger.ErrorFormat("The token for player {0} joining room is null", info.PlayerId);
            }
        }

        private void OnLoginServer(LoginServerEvent e)
        {
            if (e.Message.LoginStage == ELoginStage.PreLogin)
            {
                _connectedChannels.Add(e.Channel);
            }

            if (_hasHallServer &&  e.Message.LoginStage == ELoginStage.RequestSnapshot)
            {
                var token = e.Message.Token;
                if (_loginStates.ContainsKey(token))
                {
                    _loginStates.Remove(token);
                }
            }
        }

        public void CheckStateOnReceivePlayerMessage()
        {
            if (_hasHallServer)
            {
                if (IsRoomGameOver())
                {
                    var e = RoomEvent.AllocEvent<MandatoryLogOutEvent>();
                    e.LogOutReason = MandatoryLogOutEvent.Reason.GameOver;
                    e.PlayerId = 0;
                    e.HallRoomId = _hallRoomId;
                    e.RoomId = _roomId;

                    _dispatcher.AddEvent(e);
                    _logger.Info("The room is disposed!");
                }
            }
        }

        private List<string> _loginTimeOutList = new List<string>();
        public void Update()
        {
            SendHeartBeat();
            UpdateLoginTimer();
        }

        private List<INetworkChannel> _disconnectedChannel = new List<INetworkChannel>();
        private void SendHeartBeat()
        {
            var time = Time.time;

            if (time - _lastHeartBeatTime > MAX_HEARTBEAT_INTERVAL)
            {
                _lastHeartBeatTime = time;
                foreach (var channel in _connectedChannels)
                {
                    if (channel.IsConnected)
                    {
                        var msg = HeartBeatMessage.Allocate();
                        channel.SendReliable((int)EServer2ClientMessage.HeartBeat, msg);
                        msg.ReleaseReference();
                       
                    }
                    else
                    {
                        _disconnectedChannel.Add(channel);
                    }
                }

                if (_disconnectedChannel.Count > 0)
                {
                    foreach (var channel in _disconnectedChannel)
                    {
                        _connectedChannels.Remove(channel);
                    }

                    _disconnectedChannel.Clear();
                }
            }
        }

        private void UpdateLoginTimer()
        {
            if (_hasHallServer && _loginStates.Count > 0)
            {
                var time = Time.time;
                foreach (var state in _loginStates)
                {
                    if (IsRoomGameOver() || state.Value.IsLoginTimeOut(time))
                    {
                        _loginTimeOutList.Add(state.Key);
                        _logger.InfoFormat("Login Time Out Player Token {0}", state.Key);

                        var e = RoomEvent.AllocEvent<MandatoryLogOutEvent>();
                        e.LogOutReason = IsRoomGameOver()
                            ? MandatoryLogOutEvent.Reason.GameOver
                            : MandatoryLogOutEvent.Reason.TimeOut;
                        e.PlayerId = state.Value.PlayerId;
                        e.HallRoomId = _hallRoomId;
                        e.RoomId = _roomId;
                        e.Token = state.Key;

                        _dispatcher.AddEvent(e);
                    }
                }

                foreach (var key in _loginTimeOutList)
                {
                    _loginStates.Remove(key);
                }

                _loginTimeOutList.Clear();
            }
        }

        private bool IsRoomGameOver()
        {
            var room = _roomManager.GetRoom(_roomId);
            return room == null || room.IsGameOver;
        }

        public void Reset()
        {
            _loginStates.Clear();
        }
    }
}
