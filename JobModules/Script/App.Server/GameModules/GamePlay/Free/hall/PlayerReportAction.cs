using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Statistics;
using Sharpen;
using Core.Utils;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.Free.hall
{
    [Serializable]
    class PlayerReportAction : AbstractPlayerAction
    {
        private enum RankType
        {
            NONE,
            Survival,
            Group,
        }

        private string fields;
        private int ranktype;
        private List<PlayerEntity> _playerList = new List<PlayerEntity>();
        private int playerCountRate;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerReportAction));

        public override void DoAction(IEventArgs args)
        {
            IParable unit = args.GetUnit("report");

            var contexts = args.GameContext;
            var entities = contexts.player.GetInitializedPlayerEntities();

            _playerList.Clear();
            if (unit != null && unit is SimpleParable)
            {
                int playerCamp = 0;
                GameOverPlayer p = (GameOverPlayer)((SimpleParable)unit).GetFieldObject(0);
                foreach (PlayerEntity entity in entities)
                {
                    RankType rt = (RankType) ranktype;
                    //逃跑不排序，直接最后一名，且无需计算其他玩家的排名；不逃跑则参与排序
                    if (p.Id == entity.playerInfo.PlayerId)
                    {
                        playerCamp = entity.playerInfo.Camp;
                        if (entity.statisticsData.Statistics.IsRunaway)
                        {
                            if(rt.Equals(RankType.Group) || (rt.Equals(RankType.Survival) && entity.statisticsData.Statistics.LastDeadTime == 0))
                            {
                                entity.statisticsData.Statistics.Rank = entities.Length;
                                _playerList.Clear();
                                break;
                            }
                        }
                    }
                    _playerList.Add(entity);
                }

                if (!_playerList.IsEmpty())
                {
                    switch((RankType)ranktype)
                    {
                        case RankType.Group:
                            GroupRank(_playerList);
                            break;
                        case RankType.Survival:
                            //SurvivalRank(_playerList);
                            break;
                        default:
                            break;
                    }
                }

                string[] items = fields.Split(',');
                foreach(var item in items)
                {
                    string[] vs = item.Split('=');
                    if(vs.Length > 1)
                    {
                        int v = args.GetInt(vs[1]);
                        p.Statistics[int.Parse(vs[0])] = v;
                    }
                    else
                    {
                        Logger.ErrorFormat("illegal fields for report action : {0}", fields);
                    }
                }

                int teamCountRate = 0;
                foreach (PlayerEntity entity in entities)
                {
                    if (p.Id == entity.playerInfo.PlayerId)
                    {
                        p.Statistics[(int) EStatisticsID.Rank] = entity.statisticsData.Statistics.Rank;
                        p.Statistics[(int) EStatisticsID.RankAce] = entity.statisticsData.Statistics.Rank == 1 ? 1 : 0;
                        p.Statistics[(int) EStatisticsID.RankTen] = entity.statisticsData.Statistics.Rank <= 10 ? 1 : 0;
                        p.Statistics[(int) EStatisticsID.GetFirstBlood] = entity.statisticsData.Statistics.GetFirstBlood ? 1 : 0;
                    }

                    if (entity.playerInfo.Camp == playerCamp)
                    {
                        teamCountRate++;
                    }
                }
                p.Statistics[(int) EStatisticsID.ModePlayerCount] = teamCountRate;
                p.Statistics[(int) EStatisticsID.SectionPlayerCount] = playerCountRate;

                /*StringBuilder sb = new StringBuilder();
                foreach (var pair in p.Statistics)
                {
                    sb.Append(pair.Key).Append(":").Append(pair.Value);
                    sb.Append(",");
                }
                UnityEngine.Debug.Log(sb.ToString());*/
            }
        }

        private void SurvivalRank(List<PlayerEntity> players)
        {
            if (players.Count < 1)
            {
                return;
            }

            long chickenTeamId = -1L;

            Dictionary<long, int> teamDeadDict = new Dictionary<long, int>();
            Dictionary<long, int> teamRankDict = new Dictionary<long, int>();

            foreach (PlayerEntity entity in players)
            {
                if (!entity.statisticsData.Statistics.IsRunaway && entity.statisticsData.Statistics.DeathOrder == 0)
                {
                    //没有逃跑且没有死亡->吃鸡
                    chickenTeamId = entity.playerInfo.TeamId;
                }
                if (teamDeadDict.ContainsKey(entity.playerInfo.TeamId))
                {
                    if (entity.statisticsData.Statistics.LastDeadTime > teamDeadDict[entity.playerInfo.TeamId])
                    {
                        teamDeadDict[entity.playerInfo.TeamId] = entity.statisticsData.Statistics.LastDeadTime;
                    }
                }
                else
                {
                    teamDeadDict.Add(entity.playerInfo.TeamId, entity.statisticsData.Statistics.LastDeadTime);
                }
            }
            //队伍总数
            int teamCount = teamDeadDict.Count;

            teamRankDict.Add(chickenTeamId, 1);
            teamDeadDict.Remove(chickenTeamId);//去掉吃鸡队伍

            int rank = 1; //剩下队伍按最后死亡时间排序
            teamDeadDict = teamDeadDict.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            foreach (var deadPair in teamDeadDict)
            {
                teamRankDict.Add(deadPair.Key, ++rank);
            }

            foreach (PlayerEntity entity in players)
            {
                entity.statisticsData.Statistics.Rank = entity.statisticsData.Statistics.IsRunaway && entity.statisticsData.Statistics.DeathOrder == 0
                    ? teamCount : teamRankDict[entity.playerInfo.TeamId];
            }
        }

        private void GroupRank(List<PlayerEntity> players)
        {
            if (players.Count < 1)
            {
                return;
            }

            players.Sort((x, y) =>
            {
                if (x.statisticsData.Statistics.KillCount == y.statisticsData.Statistics.KillCount)
                {
                    if (x.statisticsData.Statistics.DeadCount == y.statisticsData.Statistics.DeadCount)
                    {
                        if (x.statisticsData.Statistics.AssistCount == y.statisticsData.Statistics.AssistCount)
                        {
                            if (x.statisticsData.Statistics.LastKillTime == y.statisticsData.Statistics.LastKillTime)
                            {
                                return 0;
                            }
                            return x.statisticsData.Statistics.LastKillTime < y.statisticsData.Statistics.LastKillTime ? -1 : 1;
                        }
                        return x.statisticsData.Statistics.AssistCount > y.statisticsData.Statistics.AssistCount ? -1 : 1;
                    }
                    return x.statisticsData.Statistics.DeadCount < y.statisticsData.Statistics.DeadCount ? -1 : 1;
                }
                return x.statisticsData.Statistics.KillCount.CompareTo(y.statisticsData.Statistics.KillCount);
            });

            playerCountRate = 0;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].statisticsData.Statistics.Rank = i + 1;
                if (players[i].statisticsData.Statistics.GameTime > 3 * 60 * 1000)
                {
                    playerCountRate++;
                }
            }
        }
    }
}
