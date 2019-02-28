using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Network;
using Core.Room;

namespace App.Server
{

    public enum ErrorCode
    {
        Unkown = 0,
        None = 1,
        CreateRoom_Message_Error = 10,
        CreateRoom_ServerRoom_Exist = 11,
        JoinRoom_Message_Error = 12,
        JoinRoom_HallRoom_NotFound = 13,
        JoinRoom_ServerRoom_Full = 14,
        JoinRoom_ServerRoom_Null = 15,
        JoinRoom_RepeatJoin = 16,
    }


    public class HallServerConnectEvent : RoomEvent
    {
        public HallServerConnectEvent()
        {
            EventType = ERoomEventType.HallServerConnect;
        }
    }

    public class HallServerDisconnectEvent : RoomEvent
    {
        public HallServerDisconnectEvent()
        {
            EventType = ERoomEventType.HallServerDisconnect;
        }
    }

    public class LoginServerEvent : RoomEvent
    {
        public INetworkChannel Channel;
        private LoginMessage _msg;
        public LoginMessage Message
        {
            get { return _msg; }
            set { _msg = ChangeReferenceValue(_msg, value); }
        }

        public override void Reset()
        {
            base.Reset();

            Channel = null;
            Message = null;
        }

        public LoginServerEvent()
        {
            EventType = ERoomEventType.LoginServer;
        }
    }

    public class PlayerLoginEvent : RoomEvent
    {
        public IPlayerInfo PlayerInfo;

        public PlayerLoginEvent()
        {
            EventType = ERoomEventType.PlayerLogin;
        }

        public override  void Reset()
        {
            base.Reset();
            PlayerInfo = null;
        }
    }

    public class  CreateRoomEvent : RoomEvent
    {
        public bool IsDummy;
        private RequestCreateRoomMessage _msg;

        public RequestCreateRoomMessage Message
        {
            get { return _msg; }
            set{ _msg = ChangeReferenceValue(_msg, value); }
        }

        public override void Reset()
        {
            base.Reset();

            IsDummy = false;
            Message = null;
        }

        public CreateRoomEvent()
        {
            EventType = ERoomEventType.CreateRoom;
        }
    }

    public class CreateRoomResponseEvent : RoomEvent
    {
        public bool Success;
        public int RoomId;
        public ErrorCode ErrCode;

        private ResponseCreateRoomMessage _msg;
        public ResponseCreateRoomMessage Message
        {
            get { return _msg; }
            set { _msg = ChangeReferenceValue(_msg, value); }
        }

        public CreateRoomResponseEvent()
        {
            EventType = ERoomEventType.CreateRoomResponse;
        }

        public override void Reset()
        {
            base.Reset();

            Success = false;
            RoomId = 0;
            Message = null;
        }
    }


    public class JoinRoomEvent : RoomEvent
    {
        public long HallRoomId;

        private RoomPlayer _roomPlayer;
        public RoomPlayer RoomPlayer
        {
            get { return _roomPlayer; }
            set { _roomPlayer = ChangeReferenceValue(_roomPlayer, value); }

        }

        public JoinRoomEvent()
        {
            EventType = ERoomEventType.JoinRoom;
        }

        public override void Reset()
        {
            base.Reset();

            HallRoomId = 0;
            RoomPlayer = null;
        }
    }

    public struct JoinRoomResponseInfo
    {
        public bool Success;
        public ErrorCode ErrCode;

        public long PlayerId;
        public string Token;
    }

    public class JoinRoomResponseEvent : RoomEvent
    {
        public long HallRoomId;
        public int RetCode;
        public JoinRoomResponseInfo JoinRoomResponseInfo = default(JoinRoomResponseInfo);
       
        public JoinRoomResponseEvent()
        {
            EventType = ERoomEventType.JoinRoomResponse;
        }
 
        public override void Reset()
        {
            base.Reset();
            HallRoomId = 0;
            RetCode = 0;
            JoinRoomResponseInfo = default(JoinRoomResponseInfo);
        }
    }

    public class JoinRoomListEvent : RoomEvent
    {
        public long HallRoomId;

        private RoomPlayer[] _roomPlayerList;
        public RoomPlayer[] RoomPlayerList
        {
            get { return _roomPlayerList; }
            set{ _roomPlayerList = ChangeReferenceValue(_roomPlayerList, value); }
        }

        public JoinRoomListEvent()
        {
            EventType = ERoomEventType.JointRoomList;
        }

        public override void Reset()
        {
            base.Reset();

            HallRoomId = 0;
            RoomPlayerList = null;
        }
    }

    public class JoinRoomListResponseEvent : RoomEvent
    {
        public long HallRoomId;
        public int RetCode;
        public List<JoinRoomResponseInfo> JoinRoomResponseInfoList = new List<JoinRoomResponseInfo>();

        public JoinRoomListResponseEvent()
        {
            EventType = ERoomEventType.JoinRoomListResponse;
        }

        public override void Reset()
        {
            base.Reset();
            HallRoomId = 0;
            RetCode = 0;
            JoinRoomResponseInfoList.Clear();
        }
    }


    public class MandatoryLogOutEvent : RoomEvent
    {
        public enum Reason
        {

            TimeOut,
            GameOver,
        }

        public Reason LogOutReason;
        public long PlayerId;
        public long HallRoomId;
        public int RoomId;
        public string Token;

        public MandatoryLogOutEvent()
        {
            EventType = ERoomEventType.MandatoryLogOut;
        }


        public override void Reset()
        {
            base.Reset();
            
            PlayerId = 0;
            HallRoomId = 0;
            RoomId = 0;
            Token = null;
        }
    }

    public class UpdateRoomGameStatusEvent : RoomEvent
    {
        public long HallRoomId;
        public int Status;
        public int CanEnter;

        public UpdateRoomGameStatusEvent()
        {
            EventType = ERoomEventType.UpdateRoomStatus;
        }
    }

    public class UpdatePlayerStatusEvent : RoomEvent
    {
        public long PlayerId;
        public int Status;

        public UpdatePlayerStatusEvent()
        {
            EventType = ERoomEventType.UpdatePlayerStatus;
        }
    }

    public class GameOverEvent : RoomEvent
    {
        public long HallRoomId;

        private GameOverMessage _msg;
        public GameOverMessage Message
        {
            get { return _msg; }
            set { _msg = ChangeReferenceValue(_msg, value); }
        }

        public override void Reset()
        {
            base.Reset();

            Message = null;
        }

        public GameOverEvent()
        {
            EventType = ERoomEventType.GameOver;
        }
    }

    public class GameExitEvent : RoomEvent
    {
        public GameExitEvent()
        {
            EventType = ERoomEventType.GameExit;
        }
    }

    public class GameOverMessageEvent : GameOverEvent
    {
        public GameOverMessageEvent()
        {
            EventType = ERoomEventType.GameOverMessage;
        }
    }

    public class LeaveRoomEvent : RoomEvent
    {
        public long PlayerId;
        private GameOverPlayer _player;
        public string Token;

        public GameOverPlayer Player
        {
            get { return _player; }
            set { _player = ChangeReferenceValue(_player, value); }
        }

        public override void Reset()
        {
            base.Reset();

            PlayerId = 0;
            Player = null;
            Token = null;
        }

        public LeaveRoomEvent()
        {
            EventType = ERoomEventType.LeaveRoom;
        }
    }
}
