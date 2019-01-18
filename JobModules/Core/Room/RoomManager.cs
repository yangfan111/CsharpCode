using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.GameTime;
using Core.Network;
using Core.Utils;
using UnityEngine;

namespace Core.Room
{
    [Obsolete("Class deprecated - Use SingleRoomManager")]
    public class RoomManager : IRoomManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RoomManager));
        private bool _tokenOnce;

        private Dictionary<IRoomId, IRoom> _roomId2Room = new Dictionary<IRoomId, IRoom>();
        private Dictionary<string, IPlayerInfo> _token2UserInfo = new Dictionary<string, IPlayerInfo>();
        private List<IPlayerInfo> _robotUserInfos = new List<IPlayerInfo>();
        //private Stopwatch _stopwatch;
       
        public RoomManager(): this(true)
        {
            
        }
        public RoomManager(bool tokenOnce)
        {
           
            _tokenOnce = tokenOnce;
        }


        public void CreateRoomIfNotExist()
        {
            throw new NotImplementedException();
        }
     

        public void AddRoom(IRoom room)
        {
            _roomId2Room[room.RoomId] = room;
        }

        public IRoom GetNewRoom()
        {
            return _roomId2Room.Values.First();
        }


        public IRoom GetRoom(int roomId)
        {
            throw new NotImplementedException();
        }

        public void AddPlayerInfo(string token, IPlayerInfo room)
        {
            _token2UserInfo[token] = room;
        }

        public void RemovePlayerInfo(string token)
        {
            if (_token2UserInfo.ContainsKey(token))
            {
                _token2UserInfo.Remove(token);
            }
        }

        public bool HasPlayerInfo(string token)
        {
            return _token2UserInfo.ContainsKey(token);
        }

        private CalcFixTimeInterval _calcFixTimeInterval = new CalcFixTimeInterval();
        private float _compensationInterval = 0;
        public void Update()
        {
            float now = Time.time*1000;
            int interval = _calcFixTimeInterval.Update(now);
            
            foreach (var room in _roomId2Room.Values)
            {
                room.Update(interval);
            }   
        }

        public void LateUpdate()
        {
            foreach (var room in _roomId2Room.Values)
            {
                room.LateUpdate();
            }
        }
        public void SetPlayerStageRunning(string messageToken, INetworkChannel channel)
        {
            IPlayerInfo playerInfo = GetUserInfoFromToken(messageToken, true);
            if (playerInfo == null)
            {
                _logger.InfoFormat("login failed, token invalid {0}", messageToken);
                return;
            }

            IRoom room = GetRoomByRoomId(playerInfo.RoomId);
            if (room == null)
            {
                _logger.InfoFormat("login failed, roomId {0} invalid", playerInfo.RoomId);
                return;
            }
            room.SetPlayerStageRunning(playerInfo, channel);
        }
        public bool RequestRoomInfo(string messageToken, INetworkChannel channel)
        {
            IPlayerInfo playerInfo = GetUserInfoFromToken(messageToken, true);
            if (playerInfo == null)
            {
                _logger.InfoFormat("login failed, token invalid {0}", messageToken);
                return false;
            }

            IRoom room = GetRoomByRoomId(playerInfo.RoomId);
            if (room == null)
            {
                _logger.InfoFormat("login failed, roomId {0} invalid", playerInfo.RoomId);
                return false;
            }
            return room.SendLoginSucc(playerInfo, channel);
          
           
        }

        public bool RequestPlayerInfo(string messageToken, INetworkChannel channel)
        {
            
            IPlayerInfo playerInfo = GetUserInfoFromToken(messageToken,false);
            if (playerInfo == null)
            {
                _logger.InfoFormat("login failed, token invalid {0}", messageToken);
                return false;
            }

            IRoom room = GetRoomByRoomId(playerInfo.RoomId);
            if (room == null)
            {
                _logger.InfoFormat("login failed, roomId {0} invalid", playerInfo.RoomId);
                return false;
            }
            return room.LoginPlayer(playerInfo, channel);
            
        }

        

        public IRoom GetRoomByRoomId(IRoomId roomId)
        {
            IRoom room;
            _roomId2Room.TryGetValue(roomId, out room);
            return room;

        }

        private const string RandomToken = "random";
        public IPlayerInfo GetUserInfoFromToken(string token, bool isCheck)
        {
            if (RandomToken.Equals(token))
            {
                System.Random rand = new System.Random();
                return new PlayerInfo(token, GetNewRoom().RoomId,rand.Next(0, 1000) , "", 2, rand.Next(0, 1000),0,0,0,0,0,null,null,false);
            }
            IPlayerInfo rc;
            
            if (_token2UserInfo.TryGetValue(token, out rc) && _tokenOnce && !isCheck)
            {
                _token2UserInfo.Remove(token);
            }

            return rc;
        }

        public List<IPlayerInfo> GetRobotPlayerInfos()
        {
            foreach (var playerInfo in _token2UserInfo.Values)
            {
                if (playerInfo.IsRobot)
                {
                    _robotUserInfos.Add(playerInfo);
                }
            }
            

            return _robotUserInfos;
        }
    }
}