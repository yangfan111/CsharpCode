using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Free;
using Core.Room;
using Core.Statistics;
using Core.Utils;
using System;
using System.Collections.Generic;

namespace App.Server.StatisticData
{
    public class SurvivalGameStatisticData : BaseGameStatisticData 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SurvivalGameStatisticData));
        //匹配积分结算相关
        /*private float _firstTeamAvgRankScore;
        private float _secondTeamAvgRankScore;
        private Dictionary<long, int> _dictDead = new Dictionary<long, int>();
        private List<IPlayerInfo> _tmpDeadPlayer = new List<IPlayerInfo>(); */

        public SurvivalGameStatisticData(Dictionary<long, ITeamInfo> dictTeam, Dictionary<long, IPlayerInfo> dictPlayers, Dictionary<long, IPlayerInfo> leavedPlayers, Dictionary<long, GameOverPlayer> goPlayers, int teamCapacity)
            : base(dictTeam, dictPlayers, leavedPlayers, goPlayers, teamCapacity)
        {
            _dictTeams = dictTeam;
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
            gameOverPlayer.Statistics[(int) EStatisticsID.PlayerDamage] = Convert.ToInt32(playerInfo.StatisticsData.PlayerDamage);
            gameOverPlayer.Statistics[(int) EStatisticsID.TotalDamage] = Convert.ToInt32(playerInfo.StatisticsData.TotalDamage);
            gameOverPlayer.Statistics[(int) EStatisticsID.ShootingCount] = playerInfo.StatisticsData.ShootingCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.ShootingSuccCount] = playerInfo.StatisticsData.ShootingSuccCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.ShootingPlayerCount] = playerInfo.StatisticsData.ShootingPlayerCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.CritCount] = playerInfo.StatisticsData.CritCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.TotalMoveDistance] = Convert.ToInt32(playerInfo.StatisticsData.TotalMoveDistance);
            gameOverPlayer.Statistics[(int) EStatisticsID.VehicleMoveDistance] = Convert.ToInt32(playerInfo.StatisticsData.VehicleMoveDistance);
            gameOverPlayer.Statistics[(int) EStatisticsID.AssistCount] = playerInfo.StatisticsData.AssistCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.CureVolume] = Convert.ToInt32(playerInfo.StatisticsData.CureVolume);
            gameOverPlayer.Statistics[(int) EStatisticsID.AccSpeedTime] = playerInfo.StatisticsData.AccSpeedTime;
            gameOverPlayer.Statistics[(int) EStatisticsID.SaveCount] = playerInfo.StatisticsData.SaveCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.TotalBeDamage] = Convert.ToInt32(playerInfo.StatisticsData.TotalBeDamage);
            gameOverPlayer.Statistics[(int) EStatisticsID.DefenseDamage] = Convert.ToInt32(playerInfo.StatisticsData.DefenseDamage);
            gameOverPlayer.Statistics[(int) EStatisticsID.DeadCount] = playerInfo.StatisticsData.DeadCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillDistance] = Convert.ToInt32(playerInfo.StatisticsData.MaxKillDistance);
            gameOverPlayer.Statistics[(int) EStatisticsID.DestroyVehicle] = playerInfo.StatisticsData.DestroyVehicle;
            gameOverPlayer.Statistics[(int) EStatisticsID.UseThrowingCount] = playerInfo.StatisticsData.UseThrowingCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.IsFullArmed] = playerInfo.StatisticsData.IsFullArmed ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.EvenKillCount] = playerInfo.StatisticsData.MaxEvenKillCount;
            gameOverPlayer.Statistics[(int) EStatisticsID.SwimTime] = playerInfo.StatisticsData.SwimTime;
            gameOverPlayer.Statistics[(int) EStatisticsID.Drown] = playerInfo.StatisticsData.Drown ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.PoisionDead] = playerInfo.StatisticsData.PoisionDead ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.DropDead] = playerInfo.StatisticsData.DropDead ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByVehicle] = playerInfo.StatisticsData.KillByVehicle ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByPlayer] = playerInfo.StatisticsData.KillByPlayer ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.KillByAirBomb] = playerInfo.StatisticsData.KillByAirBomb ? 1 : 0;
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
            gameOverPlayer.Statistics[(int) EStatisticsID.GetFirstBlood] = playerInfo.StatisticsData.GetFirstBlood ? 1 : 0;
            gameOverPlayer.Statistics[(int) EStatisticsID.TeamCount] = _dictTeams.Count;
            gameOverPlayer.Statistics[(int) EStatisticsID.DeadTime] = playerInfo.StatisticsData.DeadTime;
            gameOverPlayer.Statistics[(int) EStatisticsID.AliveTime] = (playerInfo.StatisticsData.GameTime - playerInfo.StatisticsData.DeadTime) / 1000;
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
            }
            if (playerInfo.StatisticsData.VehicleMoveDistance >= 3000)
            {
                goPlayer.Honors.Add(EHonorID.GoodDriver);
            }*/
            if (playerInfo.StatisticsData.GetFirstBlood)
            {
                goPlayer.Honors.Add(EHonorID.FirstBlood);
            }
            /*if (playerInfo.StatisticsData.SwimTime > 7 * 60000)
            {
                goPlayer.Honors.Add(EHonorID.GoodSwimmer);
            }
            if (playerInfo.StatisticsData.IsFullArmed)
            {
                goPlayer.Honors.Add(EHonorID.FullArmed);
            }
            if (playerInfo.StatisticsData.PlayerDamage >= 700)
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
