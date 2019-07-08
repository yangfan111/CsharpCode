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

        //private List<PlayerEntity> _playerList = new List<PlayerEntity>();

        public override void DoAction(IEventArgs args)
        {
            IParable unit = args.GetUnit("report");

            if (null != unit && unit is SimpleParable)
            {
                SimpleParable sp = (SimpleParable) unit;

                GameOverPlayer goPlayer = (GameOverPlayer) sp.GetFieldObject(0);
                Dictionary<long, GameOverPlayer> goPlayerList = (Dictionary<long, GameOverPlayer>) sp.GetFieldObject(1);
                Dictionary<long, IPlayerInfo> playerInfoList = (Dictionary<long, IPlayerInfo>) sp.GetFieldObject(2);
                Dictionary<long, IPlayerInfo> leavedInfoList = (Dictionary<long, IPlayerInfo>) sp.GetFieldObject(3);

                RankType rankType = (RankType) ranktype;

                IPlayerInfo playerInfo;
                playerInfoList.TryGetValue(goPlayer.Id, out playerInfo);

                if (null == playerInfo) return;
                //逃跑玩家为所有游戏中的玩家的最后一名
                if (playerInfo.StatisticsData.IsRunaway)
                {
                    playerInfo.StatisticsData.Rank = playerInfoList.Count;
                }
                //编辑器传来的数据
                string[] items = fields.Split(',');
                foreach(var item in items)
                {
                    string[] vs = item.Split('=');
                    if(vs.Length > 1)
                        goPlayer.Statistics[int.Parse(vs[0])] = args.GetInt(vs[1]);
                    else
                        Logger.ErrorFormat("字符串解析错误，请检查模式编辑器: {0}", fields);
                }
                //团战模式填入两项人数参数
                if (rankType == RankType.Group)
                {
                    int playerCountRate = 0;
                    int teamCountRate = 0; 
                    foreach (IPlayerInfo ipi in playerInfoList.Values)
                    {
                        if(ipi.StatisticsData.GameTime >= 3 * 60 * 1000) playerCountRate++;
                        if(ipi.Camp == playerInfo.Camp) teamCountRate++;
                    }
                    foreach (IPlayerInfo ipi in leavedInfoList.Values)
                    {
                        if(ipi.StatisticsData.GameTime >= 3 * 60 * 1000) playerCountRate++;
                    }
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
                                GroupRank(playerSortList);
                            }
                            break;
                        case RankType.Survival:
                            int modeId = args.GameContext.session.commonSession.RoomInfo.ModeId;
                            if (modeId != GameRules.SoloSurvival && modeId != GameRules.LadderSoloSurvival && modeId != GameRules.AbyssSoloSurvival)
                            {
                                SurvivalRank(playerInfoList.Values.ToList().Union(leavedInfoList.Values.ToList()).ToList(), goPlayerList, modeId, args);
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

        private void SurvivalRank(List<IPlayerInfo> players, Dictionary<long, GameOverPlayer> goPlayerList, int modeId, IEventArgs args)
        {
            if (players.Count < 1)
                return;

            long chickenTeamId = -1L;
            Dictionary<long, int> teamDeadDict = new Dictionary<long, int>();
            Dictionary<long, int> teamRankDict = new Dictionary<long, int>();

            foreach (IPlayerInfo ipi in players)
            {
                if (!ipi.StatisticsData.IsRunaway && ipi.StatisticsData.DeathOrder == 0)
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

        private void GroupRank(List<IPlayerInfo> players)
        {
            if (players.Count <= 1)
            {
                return;
            }
            players.Sort((x, y) =>
            {
                if (x.StatisticsData.KillCount < y.StatisticsData.KillCount)
                    return 1;
                if (x.StatisticsData.KillCount == y.StatisticsData.KillCount)
                {
                    if (x.StatisticsData.DeadCount > y.StatisticsData.DeadCount)
                        return 1;
                    if (x.StatisticsData.DeadCount == y.StatisticsData.DeadCount)
                    {
                        if (x.StatisticsData.AssistCount < y.StatisticsData.AssistCount)
                            return 1;
                        if (x.StatisticsData.AssistCount > y.StatisticsData.AssistCount)
                            return -1;
                        if (x.StatisticsData.LastKillTime < y.StatisticsData.LastKillTime)
                            return -1;
                        if (x.StatisticsData.LastKillTime > y.StatisticsData.LastKillTime)
                            return 1;
                        if (x.StatisticsData.LastDeadTime > y.StatisticsData.LastDeadTime && y.StatisticsData.LastDeadTime != 0)
                            return -1;
                        if (x.StatisticsData.LastDeadTime < y.StatisticsData.LastDeadTime && x.StatisticsData.LastDeadTime != 0)
                            return 1;
                        return 0;
                    }
                }
                return -1;
            });
            for (int i = 0; i < players.Count; i++)
            {
                players[i].StatisticsData.Rank = players[i].StatisticsData.IsRunaway ? players.Count : i + 1;
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerReportAction;
        }
    }
}
