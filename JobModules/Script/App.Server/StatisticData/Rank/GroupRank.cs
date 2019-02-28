using Core.Room;
using System.Collections.Generic;

namespace App.Server.StatisticData.Rank
{
    public class GroupRank : IRankData
    {
        public void RefreshRank(List<IPlayerInfo> players)
        {
            if(players.Count < 1)
            {
                return;
            }
            players.Sort((x, y) =>
            {
                if(x.StatisticsData.KillCount == y.StatisticsData.KillCount)
                {
                    if(x.StatisticsData.DeadCount == y.StatisticsData.DeadCount)
                    {
                        if(x.StatisticsData.AssistCount == y.StatisticsData.AssistCount)
                        {
                            return x.StatisticsData.LastKillTime < y.StatisticsData.LastKillTime ? -1 : 1;
                        }
                        return x.StatisticsData.AssistCount > y.StatisticsData.AssistCount ? -1 : 1;
                    }
                    return x.StatisticsData.DeadCount < y.StatisticsData.DeadCount ? -1 : 1;
                }
                return x.StatisticsData.KillCount > y.StatisticsData.KillCount ? -1 : 1;
            });
            for(int i = 0; i < players.Count; i++)
            {
                players[i].Rank = i + 1;
            }
        }
    }
}
