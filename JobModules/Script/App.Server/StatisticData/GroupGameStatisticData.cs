using App.Server.GameModules.GamePlay.free.player;
using App.Server.StatisticData.Rank;
using App.Shared.Components.Player;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.@event;
using com.wd.free.para;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Free;
using Core.Room;
using Core.Statistics;
using Core.Utils;
using System;
using System.Collections.Generic;

namespace App.Server.StatisticData
{
    public class GroupGameStatisticData : BaseGameStatisticData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GroupGameStatisticData));

        /*private List<IPlayerInfo> _teammates = new List<IPlayerInfo>();
        private IRankData rankData = new GroupRank();*/

        public GroupGameStatisticData(Dictionary<long, ITeamInfo> dictTeam, Dictionary<long, IPlayerInfo> dictPlayers, Dictionary<long, IPlayerInfo> leavedPlayers, Dictionary<long, GameOverPlayer> goPlayers, int teamCapacity)
            : base(dictTeam, dictPlayers, leavedPlayers, goPlayers, teamCapacity)
        {
            
        }

        public override void SetStatisticData(GameOverPlayer gameOverPlayer, IPlayerInfo playerInfo, IFreeArgs freeArgs)
        {
            gameOverPlayer.Id = playerInfo.PlayerId;
            if (null == playerInfo.StatisticsData)
            {
                Logger.Error("player's statisticsData is null ");
                return;
            }
            gameOverPlayer.Score = playerInfo.StatisticsData.KillCount;
            foreach (EStatisticsID eId in Enum.GetValues(typeof(EStatisticsID)))
            {
                gameOverPlayer.Statistics.Add((int) eId, 0);
            }
            gameOverPlayer.Statistics[(int) EStatisticsID.KillCount] = playerInfo.StatisticsData.KillCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.HitDownCount] = playerInfo.StatisticsData.HitDownCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.PlayerDamage] = (int)playerInfo.StatisticsData.PlayerDamage;
            gameOverPlayer.Statistics[(int) EStatisticsID.TotalDamage] = (int)playerInfo.StatisticsData.TotalDamage;
            gameOverPlayer.Statistics[(int) EStatisticsID.ShootingCount] = playerInfo.StatisticsData.ShootingCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.ShootingSuccCount] = playerInfo.StatisticsData.ShootingSuccCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.ShootingPlayerCount] = playerInfo.StatisticsData.ShootingPlayerCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.CritCount] = playerInfo.StatisticsData.CritCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.TotalMoveDistance] = (int)playerInfo.StatisticsData.TotalMoveDistance;
            gameOverPlayer.Statistics[(int) EStatisticsID.AssistCount] = playerInfo.StatisticsData.AssistCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.SaveCount] = playerInfo.StatisticsData.SaveCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.TotalBeDamage] = (int)playerInfo.StatisticsData.TotalBeDamage;
            gameOverPlayer.Statistics[(int) EStatisticsID.DefenseDamage] = (int)playerInfo.StatisticsData.DefenseDamage;
            gameOverPlayer.Statistics[(int) EStatisticsID.DeadCount] = playerInfo.StatisticsData.DeadCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillDistance] = (int)playerInfo.StatisticsData.MaxKillDistance;
            gameOverPlayer.Statistics[(int) EStatisticsID.UseThrowingCount] = playerInfo.StatisticsData.UseThrowingCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.EvenKillCount] = playerInfo.StatisticsData.MaxEvenKillCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByRifle] = playerInfo.StatisticsData.KillWithRifle;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByMachineGun] = playerInfo.StatisticsData.KillWithMachineGun;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillBySubMachineGun] = playerInfo.StatisticsData.KillWithSubmachineGun;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByThrowWeapon] = playerInfo.StatisticsData.KillWithThrowWeapon;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByMelee] = playerInfo.StatisticsData.KillWithMelee;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByPistol] = playerInfo.StatisticsData.KillWithPistol;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillBySniper] = playerInfo.StatisticsData.KillWithSniper;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByShotGun] = playerInfo.StatisticsData.KillWithShotGun;
            gameOverPlayer.Statistics[(int) EStatisticsID.GameTime] = playerInfo.StatisticsData.GameTime;
            gameOverPlayer.Statistics[(int) EStatisticsID.GameCount] = playerInfo.StatisticsData.IsRunaway ? 0 : 1;
            gameOverPlayer.Statistics[(int) EStatisticsID.CritKillCount] = playerInfo.StatisticsData.CritKillCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.C4SetCount] = playerInfo.StatisticsData.C4PlantCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.C4DefuseCount] = playerInfo.StatisticsData.C4DefuseCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.GetFirstBlood] = playerInfo.StatisticsData.GetFirstBlood ? 1 : 0;
            if(((PlayerEntity) playerInfo.PlayerEntity).gamePlay.LifeState != (int) EPlayerLifeState.Alive)
            {
                playerInfo.StatisticsData.DeadTime += (int) System.DateTime.Now.Ticks / 10000 - playerInfo.StatisticsData.LastDeadTime;
            }
            gameOverPlayer.Statistics[(int) EStatisticsID.DeadTime] = playerInfo.StatisticsData.DeadTime;
            AddHonorData(gameOverPlayer, playerInfo);
            PlayerReportTrigger(gameOverPlayer, playerInfo, freeArgs);
        }

        private void AddHonorData(GameOverPlayer goPlayer, IPlayerInfo playerInfo)
        {
            if (playerInfo.StatisticsData == null)
            {
                return;
            }
            /*if (playerInfo.StatisticsData.KillCount >= 10)
            {
                goPlayer.Honors.Add(EHonorID.HuntingMaster);
            }*/
            if (playerInfo.StatisticsData.GetFirstBlood)
            {
                goPlayer.Honors.Add(EHonorID.FirstBlood);
            }
            /*if (playerInfo.StatisticsData.PlayerDamage >= 700)
            {
                goPlayer.Honors.Add(EHonorID.OutputExpert);
            }
            if (playerInfo.StatisticsData.SaveCount >= 5)
            {
                goPlayer.Honors.Add(EHonorID.Nanny);
            }
            if (playerInfo.StatisticsData.PistolKillCount >= 3)
            {
                goPlayer.Honors.Add(EHonorID.WestCowboy);
            }
            if (playerInfo.StatisticsData.GrenadeKillCount >= 2)
            {
                goPlayer.Honors.Add(EHonorID.ThunderGod);
            }*/
        }
    }
}