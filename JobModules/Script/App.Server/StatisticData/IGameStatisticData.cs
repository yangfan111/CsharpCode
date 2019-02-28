using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Free;
using Core.Room;

namespace App.Server.StatisticData
{
    interface IGameStatisticData
    {
        void SetStatisticData(GameOverPlayer gameOverPlayer, IPlayerInfo player, IFreeArgs freeArgs);
    }
}
