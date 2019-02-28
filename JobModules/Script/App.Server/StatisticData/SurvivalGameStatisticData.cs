using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Configuration;
using Core.Room;
using Core.Statistics;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Singleton;

namespace App.Server.StatisticData
{
    public class SurvivalGameStatisticData : BaseGameStatisticData 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SurvivalGameStatisticData));
        //匹配积分结算相关
        private float _firstTeamAvgRankScore;
        private float _secondTeamAvgRankScore;
        private Dictionary<long, int> _dictDead = new Dictionary<long, int>();
        private List<IPlayerInfo> _tmpDeadPlayer = new List<IPlayerInfo>(); 

        public SurvivalGameStatisticData(Dictionary<long, ITeamInfo> dictTeam, Dictionary<long, IPlayerInfo> dictPlayers, int teamCapacity)
            : base(dictTeam, dictPlayers, teamCapacity)
        {
            _dictTeams = dictTeam;
        }

        private bool IsAlive(IPlayerInfo playerInfo)
        {
            return playerInfo.StatisticsData.DeathOrder == 0;
        }

        protected override void AddRankData(GameOverPlayer gameOverPlayer, IPlayerInfo playerInfo)
        {
            if(playerInfo.StatisticsData.IsRunaway && IsAlive(playerInfo))
            {
                gameOverPlayer.Rank = playerInfo.Rank = _dictPlayers.Count;
            }
            else
            {
                UpdateTeamInfo();
                CalcRankScore(playerInfo, gameOverPlayer);
                gameOverPlayer.Rank = playerInfo.Rank =  GetTeamRank(playerInfo.TeamId);
            }
            gameOverPlayer.Statistics[(int) EStatisticsID.Rank] = gameOverPlayer.Rank;
            gameOverPlayer.Statistics[(int) EStatisticsID.RankAce] = gameOverPlayer.Rank == 1 ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.RankTen] = gameOverPlayer.Rank <= 10 ? 1 : 0;
        }

        private void TmpSetPalyerDeadOrder(IPlayerInfo playerInfo)
        {
            playerInfo.StatisticsData.DeathOrder = _dictPlayers.Count;
            _tmpDeadPlayer.Add(playerInfo);
        }

        private void ResumeTmpPlayerDeadOrder()
        {
            foreach(var player in _tmpDeadPlayer)
            {
                player.StatisticsData.DeathOrder = 0;
            }
            _tmpDeadPlayer.Clear();
        }

        private void UpdateTeamInfo()
        {
            //rank（每个队伍最后死亡的人做排序）
            _dictDead.Clear();
            foreach (var player in _dictPlayers.Values)
            {
                if(null == player.StatisticsData)
                {
                    Logger.Error("player's statistic data is null");
                    continue;
                }
                if (player.StatisticsData.DeathOrder == 0)
                {
                    //还活着（吃鸡）
                    TmpSetPalyerDeadOrder(player);
                }
                if (_dictDead.ContainsKey(player.TeamId))
                {
                    if (player.StatisticsData.DeathOrder > _dictDead[player.TeamId])
                    {
                        _dictDead[player.TeamId] = player.StatisticsData.DeathOrder;
                    }
                }
                else
                {
                    _dictDead.Add(player.TeamId, player.StatisticsData.DeathOrder);
                }
            }

            ResumeTmpPlayerDeadOrder();
            _dictDead = _dictDead.OrderByDescending(pair => pair.Value).ToDictionary(x=>x.Key, x=>x.Value);
            int rank = 0;
            foreach(var deadTeamPair in _dictDead)
            {
                if (_dictTeams.ContainsKey(deadTeamPair.Key))
                {
                    var team = _dictTeams[deadTeamPair.Key];
                    team.Rank = ++rank;
                    if(rank == 1)
                    {
                        team.IsChicken = true;
                    }
                }
            }

            //sort
            _dictTeams = _dictTeams.OrderByDescending(o => o.Value.AvgRankScore).ToDictionary(p => p.Key, o => o.Value);
            var inx = 0;
            foreach (var team in _dictTeams.Values)
            {
                if (inx == 0)
                {
                    _firstTeamAvgRankScore = team.AvgRankScore;
                }
                else if (inx == 1)
                {
                    _secondTeamAvgRankScore = team.AvgRankScore;
                }
                else { break; }
                inx++;
            }

            //sum
            foreach (var player in _dictPlayers.Values)
            {
                if (_dictTeams.ContainsKey(player.TeamId))
                {
                    ITeamInfo team = _dictTeams[player.TeamId];
                    if (null != team)
                    {
                        team.TotalKillCount += player.StatisticsData.KillCount;
                        team.TotalSaveCount += player.StatisticsData.SaveCount;
                        team.TotalAliveMinute += player.StatisticsData.AliveTime;
                        team.TotalMemberRankScore += (51 - team.Rank) * 0.6f;
                    }
                }
            }
        }

        private void CalcRankScore(IPlayerInfo playerInfo, GameOverPlayer goPlayer)
        {
            ITeamInfo myTeamInfo = _dictTeams[playerInfo.TeamId];

            //1、胜利 2、平局 3、失败
            int result = myTeamInfo.IsChicken ? 1 : 3;

            float otherRankScore = _firstTeamAvgRankScore;
            float selfRankScore = myTeamInfo.AvgRankScore;
            if ((int)selfRankScore == (int)_firstTeamAvgRankScore)
            {
                otherRankScore = _secondTeamAvgRankScore;
            }

            //期望胜率
            float hopeWinRate = 1 / (1 + Mathf.Pow(10, (otherRankScore - selfRankScore) / 400));
            hopeWinRate = Mathf.Min(hopeWinRate, 0.995f);

            //胜利=1, 平局=0.5, 失败=0
            float realWinScore = 0;
            if (result == 1)
            {
                realWinScore = 1;
            }
            else if (result == 2)
            {
                realWinScore = 0.5f;
            }

            //查表
            float K = SingletonManager.Get<LadderRankConfigManager>().GetK((int)selfRankScore);

            //团队总表现
            float teamPoint = myTeamInfo.TotalKillCount / 0.5f * (myTeamInfo.TotalAliveMinute * 1.8f) + myTeamInfo.TotalSaveCount / 1.8f + myTeamInfo.TotalMemberRankScore;

            //个人表现
            float myPoint = playerInfo.StatisticsData.KillCount / 0.5f * (playerInfo.StatisticsData.AliveTime * 1.8f) + playerInfo.StatisticsData.SaveCount / 1.8f + (51 - myTeamInfo.Rank) * 0.6f;

            float myRankScore = 0;

            if (IsTeamMode())
            {
                float teamTotalScore = K * (realWinScore - hopeWinRate) * myTeamInfo.PlayerCount;

                if (result == 1)
                {
                    myRankScore = myPoint / teamPoint * teamTotalScore + teamTotalScore * 0.2f / 8;
                }
                else if (result == 2)
                {
                    myRankScore = myPoint / teamPoint * teamTotalScore;
                }
                else if (result == 3)
                {
                    myRankScore = (teamPoint - myPoint) / teamPoint / (myTeamInfo.PlayerCount - 1) * teamTotalScore - teamTotalScore * 0.2f / 8;
                }
            }
            else
            {
                myRankScore = K * (realWinScore - hopeWinRate);

                if (result == 1)
                {
                    myRankScore = myRankScore + myRankScore * 0.2f / 8;
                }
                else if (result == 2)
                {

                }
                else if (result == 3)
                {
                    myRankScore = myRankScore * 1.025f;
                }
            }

            goPlayer.RankScore = (int)myRankScore;

            //天梯分/王者分
            goPlayer.LadderScore = 0;
            if (playerInfo.IsKing)
            {
                //名次
                goPlayer.LadderScore = -60 + (_dictTeams.Count - myTeamInfo.Rank) * _teamCapacity;
                //吃鸡
                if (myTeamInfo.IsChicken)
                {
                    goPlayer.LadderScore += 15;
                }
                //一血
                if (playerInfo.StatisticsData.KillCount > 0)
                {
                    goPlayer.LadderScore += 6;
                }
                //击杀
                if (playerInfo.StatisticsData.KillCount > 1)
                {
                    goPlayer.LadderScore += (playerInfo.StatisticsData.KillCount - 1) * 3;
                }
                //助攻
                goPlayer.LadderScore += playerInfo.StatisticsData.AssistCount * 1;
                //1~10存活
                if (GetPersonRank(playerInfo) <= 10)
                {
                    goPlayer.LadderScore += 10;
                }
                //存活时间
                goPlayer.LadderScore += Math.Min(15, (int)(playerInfo.StatisticsData.AliveTime * 60 * 0.01f));

                //前10死亡
                if (playerInfo.StatisticsData.DeathOrder <= 10)
                {
                    goPlayer.LadderScore -= 10;
                }
                //第一个死亡
                if (playerInfo.StatisticsData.DeathOrder == 1)
                {
                    goPlayer.LadderScore -= 10;
                }
                //杀队友
                goPlayer.LadderScore -= playerInfo.StatisticsData.KillTeamCount * 30;
            }
            else
            {
                //名次
                goPlayer.LadderScore = -30 + (_dictTeams.Count - myTeamInfo.Rank) * _teamCapacity;
                //吃鸡
                if (myTeamInfo.IsChicken)
                {
                    goPlayer.LadderScore += 30;
                }
                //一血
                if (playerInfo.StatisticsData.KillCount > 0)
                {
                    goPlayer.LadderScore += 10;
                }
                //击杀
                if (playerInfo.StatisticsData.KillCount > 1)
                {
                    goPlayer.LadderScore += (playerInfo.StatisticsData.KillCount - 1) * 5;
                }
                //助攻
                goPlayer.LadderScore += playerInfo.StatisticsData.AssistCount * 2;
                //1~10存活
                if (GetPersonRank(playerInfo) <= 10)
                {
                    goPlayer.LadderScore += 15;
                }
                //存活时间
                goPlayer.LadderScore += Math.Min(30, (int)(playerInfo.StatisticsData.AliveTime * 60 * 0.02f));

                //前10死亡
                if (playerInfo.StatisticsData.DeathOrder <= 10)
                {
                    goPlayer.LadderScore -= 15;
                }
                //第一个死亡
                if (playerInfo.StatisticsData.DeathOrder == 1)
                {
                    goPlayer.LadderScore -= 20;
                }
                //杀队友
                goPlayer.LadderScore -= playerInfo.StatisticsData.KillTeamCount * 20;
            }
        }

        private int GetTeamRank(long teamId)
        {
            if (_dictTeams.ContainsKey(teamId))
            {
                return _dictTeams[teamId].Rank;
            }
            return 0;
        }

        private int GetPersonRank(IPlayerInfo playerInfo)
        {
            return Math.Max(1, _dictPlayers.Count - playerInfo.StatisticsData.DeathOrder + 1);
        }
    }
}
