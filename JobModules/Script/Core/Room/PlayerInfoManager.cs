using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;

namespace Core.Room
{
    public interface IPlayerTokenGenerator
    {
        string GenerateToken(long playerId);
    }

    public class PlayerInfoManager : IPlayerTokenGenerator
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerInfoManager));

        private bool _tokenOnce;

        private Dictionary<string, IPlayerInfo> _token2UserInfo = new Dictionary<string, IPlayerInfo>();
        private List<IPlayerInfo> _robotUserInfos = new List<IPlayerInfo>();

        public PlayerInfoManager(bool tokenOnce)
        {
            _tokenOnce = tokenOnce;
        }

        public void AddPlayerInfo(string token, IPlayerInfo playerInfo)
        {
            _token2UserInfo[token] = playerInfo;
        }

        public void RemovePlayerInfo(string token)
        {
            if(null == token)
            {
                _logger.Error("token to remove is null");
                return;
            }
            if (_token2UserInfo.ContainsKey(token))
            {
                _token2UserInfo.Remove(token);
            }
        }

        public bool HasPlayerInfo(string token)
        {
            return _token2UserInfo.ContainsKey(token);
        }

        public void Clear()
        {
            _token2UserInfo.Clear();
            _robotUserInfos.Clear();
        }

        private const string RandomToken = "random";
        public IPlayerInfo GetUserInfoFromToken(IRoomId roomId, string token, bool isCheck)
        {
            if (RandomToken.Equals(token))
            {
                System.Random rand = new System.Random();
                return new PlayerInfo(token, roomId, rand.Next(0, 1000), "", 2, rand.Next(0, 1000), 0, 0, 0, 0, 0, null, null, false);
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


        public IPlayerInfo GetPlayerInfo(IRoomId roomId, string messageToken, bool isCheck)
        {
            var playerInfo = GetUserInfoFromToken(roomId, messageToken, isCheck);
            if (playerInfo == null)
            {
                _logger.InfoFormat("login failed, token invalid {0}", messageToken);
                return null;
            }
            
            if (roomId != playerInfo.RoomId)
            {
                _logger.InfoFormat("login failed, roomId {0} invalid", playerInfo.RoomId);
                return null;
            }
            return playerInfo;
        }

        public string GenerateToken(long playerId)
        {
            string token = "";
            int cnt = 0;
            while (cnt < 5)
            {
                cnt++;
                token = String.Format("{0}{1}{2}", RandomString(10), playerId, DateTime.Now.ToString("hhmmss"));
                if (!HasPlayerInfo(token))
                    break;
            }
            return token;
        }

        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }



    }
}
