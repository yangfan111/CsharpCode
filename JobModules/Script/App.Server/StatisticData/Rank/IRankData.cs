using Core.Room;
using System.Collections.Generic;

namespace App.Server.StatisticData.Rank
{
    public interface IRankData
    {
        void RefreshRank(List<IPlayerInfo> players);
    }
}