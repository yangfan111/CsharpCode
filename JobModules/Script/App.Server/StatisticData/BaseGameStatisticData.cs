using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.@event;
using com.wd.free.para;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Free;
using Core.Room;
using Core.Utils;
using System.Collections.Generic;

namespace App.Server.StatisticData
{
    public class BaseGameStatisticData : IGameStatisticData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BaseGameStatisticData));
        
        protected Dictionary<long, IPlayerInfo> _dictPlayers;
        protected Dictionary<long, IPlayerInfo> _dictLeavedPlayers;
        protected Dictionary<long, GameOverPlayer> _dictGoPlayers;
        protected Dictionary<long, ITeamInfo> _dictTeams;
        protected int _teamCapacity;

        public BaseGameStatisticData(Dictionary<long, ITeamInfo> teams, Dictionary<long, IPlayerInfo> players, Dictionary<long, IPlayerInfo> leavedPlayers, Dictionary<long, GameOverPlayer> goPlayers, int teamCapacity)
        {
            _dictPlayers = players;
            _dictLeavedPlayers = leavedPlayers;
            _dictGoPlayers = goPlayers;
            _dictTeams = teams;
            _teamCapacity = teamCapacity;
        }

        /*public bool IsTeamMode()
        {
            return _teamCapacity > 1;
        }*/

        public virtual void SetStatisticData(GameOverPlayer gameOverPlayer, IPlayerInfo player, IFreeArgs freeArgs)
        {
            
        }

        protected void PlayerReportTrigger(GameOverPlayer gameOverPlayer, IPlayerInfo playerInfo, IFreeArgs freeArgs)
        {
            SimpleParable unit = new SimpleParable();
            SimpleParaList list = new SimpleParaList();
            unit.SetList(list);
            list.AddFields(new ObjectFields(gameOverPlayer));
            list.AddFields(new ObjectFields(_dictGoPlayers));
            list.AddFields(new ObjectFields(_dictPlayers));
            list.AddFields(new ObjectFields(_dictLeavedPlayers));
            IEventArgs args = freeArgs as IEventArgs;
            if (null != args)
            {
                var playerEntity = playerInfo.PlayerEntity as PlayerEntity;
                args.Trigger(FreeTriggerConstant.PLAYER_REPORT, new TempUnit("report", unit),
                    new TempUnit("current", (FreeData) playerEntity.freeData.FreeData));
            }
        }
    }
}
