using App.Server.StatisticData;
using App.Shared;
using App.Shared.Components;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Free;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Server
{
    public class TeamInfo : ITeamInfo
    {
        public long TeamId { get; set; }
        public int PlayerCount { get; set; }
        public int TotalRankScore { get; set; }
        public float AvgRankScore { get; set; }
        public int TotalKillCount { get; set; }
        public int TotalSaveCount { get; set; }
        public int TotalAliveMinute { get; set; }
        public float TotalMemberRankScore { get; set; }
        public bool IsChicken { get; set; }
        public int Rank { get; set; }
    }

    public class HallRoom : IHallRoom
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HallRoom));
        private static int ROOM_MAX_PLAYER_COUNT = 100;

        private int TotalPlayerCount { get; set; }
        public long HallRoomId { get; set; }
        public int ModeId { get; set; }
        public int TeamCapacity { get; set; }
        public int MapId { get; set; }
        public int RevivalTime { get; set; } // 复活时间(单位:秒)
        public bool MultiAngleStatus { get; set; } // 切换第三人称
        public bool WatchStatus { get; set; } // 是否允许观战
        public bool HelpStatus { get; set; } // 允许救援
        public bool HasFriendHarm { get; set; } // 友军伤害
        public int WaitTimeNum { get; set; } // 等待阶段时间设置(单位:秒)
        public int OverTime { get; set; } // 游戏结束时间(单位:秒)
        public int ConditionType { get; set; }
        public int ConditionValue { get; set; }
        public string ChannelName { get; set; }
        public string RoomName { get; set; }
        public int RoomDisplayId { get; set; }
        public int RoomCapacity { get; set; }
        public bool AllowReConnect { get; set; }

        public ERoomGameStatus GameStatus { get; set; }
        private long _createTime;
        private bool _isCreateComplete;

        private Dictionary<long, IPlayerInfo> _dictPlayers = new Dictionary<long, IPlayerInfo>();
        private Dictionary<long, IPlayerInfo> _dictLeavedPlayers = new Dictionary<long, IPlayerInfo>();

        private Dictionary<long, ITeamInfo> _dictTeams = new Dictionary<long, ITeamInfo>();
        private IGameStatisticData _gameStatisticData;

        private Dictionary<long, GameOverPlayer> _dictGoPlayers = new Dictionary<long, GameOverPlayer>();

        private RoomEventDispatcher _dispatcher;
        private Contexts _contexts;

        public HallRoom(RoomEventDispatcher dispatcher, Contexts contexts)
        {
            _dispatcher = dispatcher;
            _contexts = contexts;
            _dispatcher.OnRoomEvent += OnRoomEvent;
            _isCreateComplete = false;
            _createTime = DateTime.Now.Ticks;
        }

        private void OnRoomEvent(RoomEvent e)
        {
            switch (e.EventType)
            {
                case ERoomEventType.SetRoomStatus:
                    var evt = e as SetRoomStatusEvent;
                    UpdateRoomGameStatus(evt.GameStatus, evt.EnterStatus);
                    break;
                case ERoomEventType.MandatoryLogOut:
                    OnMandatoryLogOut(e as MandatoryLogOutEvent);
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            _dictPlayers.Clear();
            _dictTeams.Clear();
            _dictLeavedPlayers.Clear();
            _dictGoPlayers.Clear();

            _dispatcher.OnRoomEvent -= OnRoomEvent;
        }

        public void Init()
        {
            TotalPlayerCount = 0;
            if (ModeId == 0) ModeId = _contexts.session.commonSession.RoomInfo.ModeId;
            if (GameRules.IsChicken(ModeId))
            {
                _gameStatisticData = new SurvivalGameStatisticData(_dictTeams, _dictPlayers, _dictLeavedPlayers, _dictGoPlayers, TeamCapacity);
            }
            else
            {
                _gameStatisticData = new GroupGameStatisticData(_dictTeams, _dictPlayers, _dictLeavedPlayers, _dictGoPlayers, TeamCapacity);
            }
            /*var type = SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(ModeId);
            switch (type)
            {
                case XmlConfig.EBagType.Chicken:
                    _gameStatisticData = new SurvivalGameStatisticData(_dictTeams, _dictPlayers, _dictLeavedPlayers, _dictGoPlayers, TeamCapacity);
                    break;
                case XmlConfig.EBagType.Group:
                    _gameStatisticData = new GroupGameStatisticData(_dictTeams, _dictPlayers, _dictLeavedPlayers, _dictGoPlayers, TeamCapacity);
                    break;
            }*/
        }

        public void AddPlayer(IPlayerInfo player)
        {
            _logger.InfoFormat("player {0} {1} is trying to join hall room", player.PlayerId, player.PlayerName);
            if (_dictLeavedPlayers.ContainsKey(player.PlayerId))
                _dictLeavedPlayers.Remove(player.PlayerId);
            if (_dictGoPlayers.ContainsKey(player.PlayerId))
                _dictGoPlayers.Remove(player.PlayerId);
            if (!_dictPlayers.ContainsKey(player.PlayerId))
            {
                _dictPlayers.Add(player.PlayerId, player);
                //team
                ITeamInfo team;
                if (_dictTeams.ContainsKey(player.TeamId))
                {
                    team = _dictTeams[player.TeamId];
                }
                else
                {
                    team = new TeamInfo();
                    team.TeamId = player.TeamId;
                    _dictTeams.Add(team.TeamId, team);
                }
                team.PlayerCount++;
                team.TotalRankScore += player.RankScore;
                team.AvgRankScore = team.PlayerCount > 0 ? (float)team.TotalRankScore / team.PlayerCount : 0;
                _logger.InfoFormat("player {0} {1} joined hall room", player.PlayerId, player.PlayerName);
            }
            TotalPlayerCount = _dictPlayers.Count;
        }

        public List<IPlayerInfo> GetAllPlayers()
        {
            return _dictPlayers.Values.ToList();
        }

        public IPlayerInfo GetPlayer(long playerId)
        {
            IPlayerInfo player;
            _dictPlayers.TryGetValue(playerId, out player);
            if (null == player)
            {
                _dictLeavedPlayers.TryGetValue(playerId, out player);
            }
            return player;
        }

        public void RemovePlayer(long playerId)
        {
            if (_dictPlayers.ContainsKey(playerId))
            {
                IPlayerInfo player = _dictPlayers[playerId];
                _dictPlayers.Remove(playerId);
                _dictLeavedPlayers.Add(playerId, player);
                
                if (_dictTeams.ContainsKey(player.TeamId))
                {
                    ITeamInfo team = _dictTeams[player.TeamId];
                    team.PlayerCount--;
                    team.TotalRankScore -= player.RankScore;
                    team.AvgRankScore = team.PlayerCount > 0 ? (float)team.TotalRankScore / team.PlayerCount : 0;
                }

                _logger.InfoFormat("player {0} {1} left hall room", player.PlayerId, player.PlayerName);
            }
        }

        public bool HasPlayer(long playerId)
        {
            return _dictPlayers.ContainsKey(playerId);
        }

        public bool CanJoin()
        {
            return _dictPlayers.Count < RoomCapacity; //ROOM_MAX_PLAYER_COUNT;
        }

        public int MaxNum(long teamId)
        {
            int num = 0;
            foreach (var player in _dictPlayers.Values)
            {
                if (teamId == player.TeamId)
                    num++;
            }
            return ++num;
        }

        private void OnMandatoryLogOut(MandatoryLogOutEvent e)
        {
            if (e.PlayerId != 0 && _dictPlayers.ContainsKey(e.PlayerId))
            {
                var player = _dictPlayers[e.PlayerId];
                _logger.InfoFormat("Hall player login timeout ... Id:{0}, Name:{1}", player.PlayerId, player.PlayerName);
                RemovePlayer(player.PlayerId);
            }
        }

        public void UpdateRoomGameStatus(ERoomGameStatus status, ERoomEnterStatus enter)
        {
            _logger.InfoFormat("Update Room Status {0}=>{1}", GameStatus, status);
            GameStatus = status;

            var e = RoomEvent.AllocEvent<UpdateRoomGameStatusEvent>();
            e.HallRoomId = HallRoomId;
            e.Status = (int)status;
            e.CanEnter = (int) enter;

            _dispatcher.AddEvent(e);
        }

        public void UpdatePlayerStatus(long playerId, EPlayerGameStatus status)
        {
            var e = RoomEvent.AllocEvent<UpdatePlayerStatusEvent>();
            e.PlayerId = playerId;
            e.Status = (int)status;

            _dispatcher.AddEvent(e);
        }

        public void PlayerLoginSucc(long playerId)
        {
            _isCreateComplete = true;
            IPlayerInfo player = GetPlayer(playerId);
            if (null != player)
            {
                player.IsLogin = true;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                if (!_isCreateComplete)
                {
                    long nowTime = DateTime.Now.Ticks;
                    if (nowTime - _createTime > ((long) 600) * 1000 * 10000)
                    {
                        //room timeout
                        _logger.InfoFormat("Hall room timeout ... Id:{0}", HallRoomId);

                        return false;
                    }
                }
                
                return true;
            }
        }

       public void PlayerLeaveRoom(long playerId)
        {
            _logger.InfoFormat("player leave room {0}", playerId);
            GameOverPlayer gameOverPlayer = null;
            var evt = RoomEvent.AllocEvent<LeaveRoomEvent>();
            if (_dictPlayers.ContainsKey(playerId))
            {
                IPlayerInfo player = _dictPlayers[playerId];
                if (player.StatisticsData != null && player.StatisticsData.GameTime <= 0)
                {
                    player.StatisticsData.DeadTime += (int) (_contexts.session.commonSession.FreeArgs.Rule.ServerTime - player.StatisticsData.LastDeadTime);
                    player.StatisticsData.GameTime = (int) (_contexts.session.commonSession.FreeArgs.Rule.ServerTime - player.StatisticsData.GameJoinTime);
                }
                gameOverPlayer = GameOverPlayer.Allocate();
                SetGameOverPlayerValue(gameOverPlayer, player);
                //_gameStatisticData.SetStatisticData(gameOverPlayer, playerInfo, _contexts.session.commonSession.FreeArgs);

                if (player.Token != TestUtility.TestToken)
                {
                    evt.Token = player.Token;
                }

                RemovePlayer(playerId);
                _logger.InfoFormat("player {0} leave room, set statistics complete", playerId);
            }

            evt.PlayerId = playerId;
            evt.Player = gameOverPlayer;
            _dispatcher.AddEvent(evt);
        }

        public virtual void GameOver(bool forceOver)
        {
            var msg = GameOverMessage.Allocate();
            msg.HallRoomId = this.HallRoomId;
            if (!forceOver)
            {
                try
                {
                    foreach (var player in _dictPlayers.Values)
                    {
                        var gameOverPlayer = GameOverPlayer.Allocate();
                        SetGameOverPlayerValue(gameOverPlayer, player);
                        //_gameStatisticData.SetStatisticData(gameOverPlayer, playerInfo, _contexts.session.commonSession.FreeArgs);
                        msg.Players.Add(gameOverPlayer);
                    }

                    if (_dictGoPlayers.Count > 0)
                    {
                        foreach (var gameOverPlayer in _dictGoPlayers.Values)
                        {
                            msg.Players.Add(gameOverPlayer);
                        }
                    }

                    msg.TotalPlayer = _dictPlayers.Count + _dictGoPlayers.Count;
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("GameOver ... Error:{0}", e.StackTrace);
                }
            }
            else
            {
                Dispose();
            }

            IFreeRule rule = _contexts.session.commonSession.FreeArgs.Rule;
            msg.BattleStartTime = rule.StartTime + rule.GameStartTime;
            msg.BattleEndTime = rule.StartTime + rule.GameEndTime;
         

            var evt = RoomEvent.AllocEvent<GameOverEvent>();
            evt.HallRoomId = HallRoomId;
            evt.Message = msg;
            _dispatcher.AddEvent(evt);
        }

        private void SetGameOverPlayerValue(GameOverPlayer gameOverPlayer, IPlayerInfo player)
        {
            if (_gameStatisticData != null)
            {
                _gameStatisticData.SetStatisticData(gameOverPlayer, player, _contexts.session.commonSession.FreeArgs);
            }
        }
    }


    public class DummyHallRoom : HallRoom
    {
        private RoomEventDispatcher _dispatcher;
        private Contexts _contexts;
        public DummyHallRoom(RoomEventDispatcher dispatcher, Contexts contexts): base(dispatcher, contexts)
        {
            _dispatcher = dispatcher;
            _contexts = contexts;
            _dispatcher.OnRoomEvent += OnRoomEvent;
            AllowReConnect = false;
        }

        private void OnRoomEvent(RoomEvent e)
        {
            switch (e.EventType)
            {
                case ERoomEventType.PlayerLogin:
                    OnPlayerLogin(e as PlayerLoginEvent);
                    break;
            }
        }

        private void OnPlayerLogin(PlayerLoginEvent e)
        {
            if (e.PlayerInfo != null)
            {
                AddPlayer(e.PlayerInfo);
            }
        }

        public override bool IsValid{ get { return true; }}

        public override void GameOver(bool forceOver)
        {
            var evt = RoomEvent.AllocEvent<GameOverEvent>();
            evt.HallRoomId = HallRoomId;
            evt.Message = null;
            _dispatcher.AddEvent(evt);
        }
    }
}
