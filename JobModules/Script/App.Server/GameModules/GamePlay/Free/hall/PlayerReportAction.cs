using App.Shared;
using App.Shared.Components;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Free;
using Core.Room;
using Core.Statistics;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Server.GameModules.GamePlay.Free.hall
{
    [Serializable]
    class PlayerReportAction : AbstractPlayerAction, IRule
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerReportAction));

        private enum RankType
        {
            NONE,
            Survival,
            Group,
        }

        private string fields;
        private int ranktype;
        private bool issend;

        public override void DoAction(IEventArgs args)
        {
            IParable unit = args.GetUnit("report");

            if (null != unit && unit is SimpleParable)
            {
                SimpleParable sp = (SimpleParable) unit;

                GameOverPlayer goPlayer = (GameOverPlayer) sp.GetFieldObject(0);
                Dictionary<long, GameOverPlayer> goPlayerList = (Dictionary<long, GameOverPlayer>) sp.GetFieldObject(1);
                Dictionary<long, IPlayerInfo> playerInfoList = new Dictionary<long, IPlayerInfo>();
                Dictionary<long, IPlayerInfo> leavedInfoList = new Dictionary<long, IPlayerInfo>();
                Dictionary<long, IPlayerInfo> infoList = (Dictionary<long, IPlayerInfo>) sp.GetFieldObject(2);
                Dictionary<long, IPlayerInfo> leftList = (Dictionary<long, IPlayerInfo>) sp.GetFieldObject(3);
                int TeamCapacity = (int) sp.GetFieldObject(4);
                Logger.InfoFormat("reporting player {0}, totoal player {1}, left player {2}", goPlayer.Id, infoList.Count, leftList.Count);
                RankType rankType = (RankType) ranktype;

                IPlayerInfo playerInfo;
                infoList.TryGetValue(goPlayer.Id, out playerInfo);
                if (null == playerInfo)
                {
                    Logger.InfoFormat("no IPlayerInfo of player {0}", goPlayer.Id);
                    return;
                }

                //移除缺少数据的玩家
                IPlayerInfo availablePlayer;
                List<long> keys = infoList.Keys.ToList();
                foreach (long key in keys)
                {
                    infoList.TryGetValue(key, out availablePlayer);
                    if (availablePlayer == null || availablePlayer.StatisticsData == null)
                    {
                        Logger.InfoFormat("player {0} info not available", key);
                        continue;
                    }
                    playerInfoList.Add(key, availablePlayer);
                }
                keys = leftList.Keys.ToList();
                foreach (long key in keys)
                {
                    leftList.TryGetValue(key, out availablePlayer);
                    if (availablePlayer == null || availablePlayer.StatisticsData == null)
                    {
                        Logger.InfoFormat("left player {0} info not available", key);
                        continue;
                    }
                    leavedInfoList.Add(key, availablePlayer);
                }

                //逃跑玩家为所有游戏中的玩家的最后一名
                if (playerInfo.StatisticsData.IsRunaway) playerInfo.StatisticsData.Rank = playerInfoList.Count;

                //编辑器传来的数据
                if (!string.IsNullOrEmpty(fields))
                {
                    string[] items = fields.Split(',');
                    foreach(var item in items)
                    {
                        string[] vs = item.Split('=');
                        if(vs.Length > 1)
                            goPlayer.Statistics[int.Parse(vs[0])] = args.GetInt(vs[1]);
                        else
                            Logger.ErrorFormat("字符串解析错误，请检查模式编辑器: {0}", fields);
                    }
                }
                //团战模式填入两项人数参数
                if (rankType == RankType.Group)
                {
                    int playerCountRate = 0;
                    int teamCountRate = 0; 
                    foreach (IPlayerInfo ipi in playerInfoList.Values)
                    {
                        if (ipi.StatisticsData.GameJoinTime > 0 && args.Rule.ServerTime - ipi.StatisticsData.GameJoinTime >= 180000L) playerCountRate++;
                        if (ipi.Camp == playerInfo.Camp) teamCountRate++;
                        Logger.InfoFormat("player id {0} camp {1} game join time {2} played time {3}", ipi.PlayerId, ipi.Camp, ipi.StatisticsData.GameJoinTime, args.Rule.ServerTime - ipi.StatisticsData.GameJoinTime);
                    }
                    Logger.InfoFormat("game total player count {0} report send {1}", playerInfoList.Count, issend);
                    goPlayer.Statistics[(int) EStatisticsID.ModePlayerCount] = teamCountRate;
                    goPlayer.Statistics[(int) EStatisticsID.SectionPlayerCount] = playerCountRate;
                }

                if (issend)
                {
                    switch (rankType)
                    {
                        case RankType.Group:
                            if (!playerInfo.StatisticsData.IsRunaway)
                            {
                                List<IPlayerInfo> playerSortList = playerInfoList.Values.ToList();
                                GroupRank(playerSortList, playerInfo, TeamCapacity > 1);
                            }
                            break;
                        case RankType.Survival:
                            int modeId = args.GameContext.session.commonSession.RoomInfo.ModeId;
                            if (modeId != GameRules.SoloSurvival && modeId != GameRules.LadderSoloSurvival && modeId != GameRules.AbyssSoloSurvival)
                            {
                                SurvivalRank(playerInfoList.Values.ToList().Union(leavedInfoList.Values.ToList()).ToList(), goPlayerList, args, playerInfo);
                            }
                            break;
                        default:
                            break;
                    }
                    goPlayer.Statistics[(int) EStatisticsID.Rank] = playerInfo.StatisticsData.Rank;
                    goPlayer.Statistics[(int) EStatisticsID.RankAce] = playerInfo.StatisticsData.Rank == 1 ? 1 : 0;
                    goPlayer.Statistics[(int) EStatisticsID.RankTen] = playerInfo.StatisticsData.Rank <= 10 ? 1 : 0;
                }
                else
                {
                    //不算逃跑的离开玩家的数据保存，否则丢弃数据
                    if (!playerInfo.StatisticsData.IsRunaway && rankType == RankType.Survival)
                    {
                        GameOverPlayer savedGoPlayer = goPlayer.Clone();
                        if (goPlayerList.ContainsKey(savedGoPlayer.Id))
                            Logger.ErrorFormat("房间GameOverPlayer列表中出现重复ID {0}", savedGoPlayer.Id);
                        else
                            goPlayerList.Add(savedGoPlayer.Id, savedGoPlayer);
                    }
                    //不报告的情况下，将GameOverPlayer ID设为0
                    goPlayer.Id = 0L;
                }
            }
        }

        private void SurvivalRank(List<IPlayerInfo> players, Dictionary<long, GameOverPlayer> goPlayerList, IEventArgs args, IPlayerInfo playerInfo)
        {
            if (players.Count < 1) return;

            long chickenTeamId = -1L;
            Dictionary<long, int> teamDeadDict = new Dictionary<long, int>();
            Dictionary<long, int> teamRankDict = new Dictionary<long, int>();

            foreach (IPlayerInfo ipi in players)
            {
                if (!ipi.StatisticsData.IsRunaway && ipi.StatisticsData.LastDeadTime <= 0L)
                {
                    //和谐模式下，胜出5人不参与排序
                    if (!SharedConfig.IsHXMod && FreeUtil.ReplaceBool("{hasWin}", args))
                    {
                        //没有逃跑且没有死亡->吃鸡
                        chickenTeamId = ipi.TeamId;
                    }
                }
                if (teamDeadDict.ContainsKey(ipi.TeamId))
                {
                    if (ipi.StatisticsData.LastDeadTime > teamDeadDict[ipi.TeamId])
                    {
                        teamDeadDict[ipi.TeamId] = (int) ipi.StatisticsData.LastDeadTime;
                    }
                }
                else
                {
                    teamDeadDict.Add(ipi.TeamId, (int) ipi.StatisticsData.LastDeadTime);
                }
            }
            //队伍总数
            int teamCount = teamDeadDict.Count;

            teamRankDict.Add(chickenTeamId, 1);
            if (chickenTeamId != -1)
            {
                teamDeadDict.Remove(chickenTeamId);//去掉吃鸡队伍
            }

            int rank = 1; //剩下队伍按最后死亡时间排序
            teamDeadDict = teamDeadDict.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            foreach (var deadPair in teamDeadDict)
            {
                teamRankDict.Add(deadPair.Key, ++rank);
            }

            foreach (IPlayerInfo ipi in players)
            {
                if (ipi.StatisticsData.Rank != 1)
                {
                    if (ipi.StatisticsData.IsRunaway)
                    {
                        ipi.StatisticsData.Rank = teamCount;
                    }
                    else
                    {
                        if (FreeUtil.ReplaceBool("{hasWin}", args))
                        {
                            ipi.StatisticsData.Rank = teamRankDict[ipi.TeamId];
                        }
                        else if (ipi.StatisticsData.LastDeadTime != 0L)
                        {
                            ipi.StatisticsData.Rank = teamRankDict[ipi.TeamId] - 1;
                        }
                    }
                }

                Logger.InfoFormat("player id {0} should be rank {1} in total team {2}, game has win ? {3}", ipi.PlayerId, ipi.StatisticsData.Rank, teamCount, FreeUtil.ReplaceBool("{hasWin}", args));

                if (ipi.PlayerId == playerInfo.PlayerId) playerInfo.StatisticsData.Rank = ipi.StatisticsData.Rank;

                GameOverPlayer gop;
                goPlayerList.TryGetValue(ipi.PlayerId, out gop);
                if (gop != null)
                {
                    gop.Statistics[(int) EStatisticsID.Rank] = ipi.StatisticsData.Rank;
                    gop.Statistics[(int) EStatisticsID.RankAce] = ipi.StatisticsData.Rank == 1 ? 1 : 0;
                    gop.Statistics[(int) EStatisticsID.RankTen] = ipi.StatisticsData.Rank <= 10 ? 1 : 0;
                }
            }
        }

        private void GroupRank(List<IPlayerInfo> players, IPlayerInfo playerInfo, bool rescue)
        {
            if (players.Count <= 1)
            {
                return;
            }
            players.Sort((x, y) =>
            {
                var xStat = x.StatisticsData;
                var yStat = y.StatisticsData;
                if (rescue)
                {
                    if (xStat.HitDownCount < yStat.HitDownCount) return 1;
                    if (xStat.HitDownCount > yStat.HitDownCount) return -1;
                    if (xStat.DeadCount > yStat.DeadCount) return 1;
                    if (xStat.DeadCount < yStat.DeadCount) return -1;
                    if (xStat.SaveCount > yStat.SaveCount) return -1;
                    if (xStat.SaveCount < yStat.SaveCount) return 1;
                    if (xStat.TotalDamage > yStat.TotalDamage) return -1;
                    if (xStat.TotalDamage < yStat.TotalDamage) return 1;
                    if (xStat.LastKillTime < yStat.LastKillTime) return -1;
                    if (xStat.LastKillTime > yStat.LastKillTime) return 1;
                    if ((xStat.LastDeadTime == 0 && yStat.LastDeadTime != 0) || (xStat.LastDeadTime > yStat.LastDeadTime && yStat.LastDeadTime != 0)) return -1;
                    if ((xStat.LastDeadTime != 0 && yStat.LastDeadTime == 0) || (xStat.LastDeadTime < yStat.LastDeadTime && xStat.LastDeadTime != 0)) return 1;
                    return 0;
                }
                else
                {
                    if (xStat.KillCount < yStat.KillCount) return 1;
                    if (xStat.KillCount > yStat.KillCount) return -1;
                    if (xStat.DeadCount > yStat.DeadCount) return 1;
                    if (xStat.DeadCount < yStat.DeadCount) return -1;
                    if (xStat.AssistCount > yStat.AssistCount) return -1;
                    if (xStat.AssistCount < yStat.AssistCount) return 1;
                    if (xStat.TotalDamage > yStat.TotalDamage) return -1;
                    if (xStat.TotalDamage < yStat.TotalDamage) return 1;
                    if (xStat.LastKillTime < yStat.LastKillTime) return -1;
                    if (xStat.LastKillTime > yStat.LastKillTime) return 1;
                    if ((xStat.LastDeadTime == 0 && yStat.LastDeadTime != 0) || (xStat.LastDeadTime > yStat.LastDeadTime && yStat.LastDeadTime != 0)) return -1;
                    if ((xStat.LastDeadTime != 0 && yStat.LastDeadTime == 0) || (xStat.LastDeadTime < yStat.LastDeadTime && xStat.LastDeadTime != 0)) return 1;
                    return 0;
                }
            });

            for (int i = 0; i < players.Count; i++)
            {
                players[i].StatisticsData.Rank = players[i].StatisticsData.IsRunaway ? players.Count : i + 1;
                if (players[i].PlayerId == playerInfo.PlayerId) playerInfo.StatisticsData.Rank = players[i].StatisticsData.Rank;
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerReportAction;
        }
    }
}
