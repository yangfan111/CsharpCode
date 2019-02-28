using System;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Room;
using Core.Statistics;
using Core.Utils;
using System.Collections.Generic;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components.Player;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.@event;
using com.wd.free.para;
using Core.Free;

namespace App.Server.StatisticData
{
    public class BaseGameStatisticData : IGameStatisticData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BaseGameStatisticData));
        /// <summary>
        /// 游戏场次固定为1
        /// </summary>
        private const int GameCount = 1;
        protected Dictionary<long, IPlayerInfo> _dictPlayers;
        protected Dictionary<long, ITeamInfo> _dictTeams;
        protected int _teamCapacity;

        public BaseGameStatisticData(Dictionary<long, ITeamInfo> teams, Dictionary<long, IPlayerInfo> players, int teamCapacity)
        {
            _dictPlayers = players;
            _dictTeams = teams;
            _teamCapacity = teamCapacity;
        }

        public bool IsTeamMode()
        {
            return _teamCapacity > 1;
        }

        public void SetStatisticData(GameOverPlayer gameOverPlayer, IPlayerInfo player, IFreeArgs freeArgs)
        {
            gameOverPlayer.Id = player.PlayerId;
            if(null == player.StatisticsData)
            {
                Logger.Error("player's statisticsData is null ");
                return;
            }
            gameOverPlayer.Score = player.StatisticsData.KillCount;
            //AddRankData(gameOverPlayer, player);
            AddStatisticsData(player, gameOverPlayer);
            AddHonorData(player, gameOverPlayer);

            SimpleParable unit = new SimpleParable();
            SimpleParaList list = new SimpleParaList();
            unit.SetList(list);
            list.AddFields(new ObjectFields(gameOverPlayer));
            IEventArgs args = freeArgs as IEventArgs;
            if (null != args)
            {
                var playerEntity = player.PlayerEntity as PlayerEntity;

                args.Trigger(FreeTriggerConstant.PLAYER_REPORT, new TempUnit[] {
                    new TempUnit("report", unit),
                    new TempUnit("current", (FreeData)playerEntity.freeData.FreeData) });
            }
        }

        protected virtual void AddRankData(GameOverPlayer gameOverPlayer, IPlayerInfo playerInfo)
        {

        }

        private void AddStatisticsData(IPlayerInfo playerInfo, GameOverPlayer gameoverPlayer)
        {
            foreach (EStatisticsID eId in Enum.GetValues(typeof(EStatisticsID)))
            {
                gameoverPlayer.Statistics.Add((int) eId, 0);
            }
            gameoverPlayer.Statistics[(int) EStatisticsID.KillCount] = playerInfo.StatisticsData.KillCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.HitDownCount] = playerInfo.StatisticsData.HitDownCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.PlayerDamage] = (int)playerInfo.StatisticsData.PlayerDamage;
            gameoverPlayer.Statistics[(int) EStatisticsID.TotalDamage] = (int)playerInfo.StatisticsData.TotalDamage;
            //gameoverPlayer.Statistics[(int) EStatisticsID.AliveTime] = playerInfo.StatisticsData.AliveTime;
            gameoverPlayer.Statistics[(int) EStatisticsID.ShootingCount] = playerInfo.StatisticsData.ShootingCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.ShootingSuccCount] = playerInfo.StatisticsData.ShootingSuccCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.ShootingPlayerCount] = playerInfo.StatisticsData.ShootingPlayerCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.CritCount] = playerInfo.StatisticsData.CritCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.TotalMoveDistance] = (int)playerInfo.StatisticsData.TotalMoveDistance;
            gameoverPlayer.Statistics[(int) EStatisticsID.VehicleMoveDistance] = (int)playerInfo.StatisticsData.VehicleMoveDistance;
            gameoverPlayer.Statistics[(int) EStatisticsID.AssistCount] = playerInfo.StatisticsData.AssistCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.CureVolume] = (int)playerInfo.StatisticsData.CureVolume;
            gameoverPlayer.Statistics[(int) EStatisticsID.AccSpeedTime] = playerInfo.StatisticsData.AccSpeedTime;
            gameoverPlayer.Statistics[(int) EStatisticsID.SaveCount] = playerInfo.StatisticsData.SaveCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.TotalBeDamage] = (int)playerInfo.StatisticsData.TotalBeDamage;
            gameoverPlayer.Statistics[(int) EStatisticsID.DefenseDamage] = (int)playerInfo.StatisticsData.DefenseDamage;
            gameoverPlayer.Statistics[(int) EStatisticsID.DeadCount] = playerInfo.StatisticsData.DeadCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillDistance] = (int)playerInfo.StatisticsData.MaxKillDistance;
            gameoverPlayer.Statistics[(int) EStatisticsID.DestroyVehicle] = playerInfo.StatisticsData.DestroyVehicle;
            gameoverPlayer.Statistics[(int) EStatisticsID.UseThrowingCount] = playerInfo.StatisticsData.UseThrowingCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.IsFullArmed] = playerInfo.StatisticsData.IsFullArmed ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.EvenKillCount] = playerInfo.StatisticsData.MaxEvenKillCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.SwimTime] = playerInfo.StatisticsData.SwimTime;
            /*if (_dictTeams.ContainsKey(playerInfo.TeamId))
            {
                gameoverPlayer.Statistics[(int) EStatisticsID.TeamKillCount] = _dictTeams[playerInfo.TeamId].TotalKillCount;
            }*/
            gameoverPlayer.Statistics[(int) EStatisticsID.Drown] = playerInfo.StatisticsData.Drown ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.PoisionDead] = playerInfo.StatisticsData.PoisionDead ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.DropDead] = playerInfo.StatisticsData.DropDead ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByVehicle] = playerInfo.StatisticsData.KillByVehicle ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByPlayer] = playerInfo.StatisticsData.KillByPlayer ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByAirBomb] = playerInfo.StatisticsData.KillByAirBomb ? 1 : 0;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByRifle] = playerInfo.StatisticsData.KillWithRifle;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByMachineGun] = playerInfo.StatisticsData.KillWithMachineGun;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillBySubMachineGun] = playerInfo.StatisticsData.KillWithSubmachineGun;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByThrowWeapon] = playerInfo.StatisticsData.KillWithThrowWeapon;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByMelee] = playerInfo.StatisticsData.KillWithMelee;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByPistol] = playerInfo.StatisticsData.KillWithPistol;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillBySniper] = playerInfo.StatisticsData.KillWithSniper;
            gameoverPlayer.Statistics[(int) EStatisticsID.KillByShotGun] = playerInfo.StatisticsData.KillWithShotGun;
            gameoverPlayer.Statistics[(int) EStatisticsID.GameTime] = playerInfo.StatisticsData.GameTime;
            gameoverPlayer.Statistics[(int) EStatisticsID.GameCount] = playerInfo.StatisticsData.IsRunaway ? 0 : GameCount;
            gameoverPlayer.Statistics[(int) EStatisticsID.CritKillCount] = playerInfo.StatisticsData.CritKillCount;
            ProcessPlayerDeadTime(gameoverPlayer, playerInfo);
            /*StringBuilder sb = new StringBuilder();
            foreach(var data in gameoverPlayer.Statistics)
            {
                sb.Append(data.Key);
                sb.Append(" : ");
                sb.Append(data.Value);
                sb.AppendLine();
            }
            UnityEngine.Debug.Log(sb.ToString());*/
        }

        private void ProcessPlayerDeadTime(GameOverPlayer gameOverPlayer, IPlayerInfo playerInfo)
        {
            if(((PlayerEntity) playerInfo.PlayerEntity).gamePlay.LifeState != (int) EPlayerLifeState.Alive)
            {
                playerInfo.StatisticsData.DeadTime += (int) System.DateTime.Now.Ticks / 10000 - playerInfo.StatisticsData.LastDeadTime;
            }
            gameOverPlayer.Statistics[(int) EStatisticsID.DeadTime] = playerInfo.StatisticsData.DeadTime;
        }

        private void AddHonorData(IPlayerInfo playerInfo, GameOverPlayer goPlayer)
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
