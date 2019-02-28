using App.Server.StatisticData.Rank;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Room;
using Core.Statistics;
using System.Collections.Generic;

namespace App.Server.StatisticData
{
    public class GroupGameStatisticData : BaseGameStatisticData
    {
        private List<IPlayerInfo> _teammates = new List<IPlayerInfo>();
        private IRankData rankData = new GroupRank();

        public GroupGameStatisticData(Dictionary<long, ITeamInfo> dictTeam, Dictionary<long, IPlayerInfo> dictPlayers, int teamCapacity)
            : base(dictTeam, dictPlayers, teamCapacity)
        {
            
        }

        protected override void AddRankData(GameOverPlayer gameOverPlayer, IPlayerInfo playerInfo)
        {
            _teammates.Clear();
            var teamId = playerInfo.TeamId;
            foreach(var player in _dictPlayers)
            {
                if(player.Value.TeamId == teamId)
                {
                    _teammates.Add(player.Value);
                }
            }
            if(playerInfo.StatisticsData.IsRunaway)
            {
                gameOverPlayer.Rank = _teammates.Count;

            }
            else
            {
                rankData.RefreshRank(_teammates);
                gameOverPlayer.Rank = playerInfo.Rank;
            }
            gameOverPlayer.Statistics[(int) EStatisticsID.Rank] = gameOverPlayer.Rank;
            gameOverPlayer.Statistics[(int) EStatisticsID.RankAce] = gameOverPlayer.Rank == 1 ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.RankTen] = gameOverPlayer.Rank <= 10 ? 1 : 0;
        }
    }
}