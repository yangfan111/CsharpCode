using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Statistics;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerStatisticsSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerStatisticsSystem));
        private Vector2 _startPos = Vector2.zero;
        private Vector2 _endPos = Vector2.zero;

        public PlayerStatisticsSystem()
        {
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            if (!player.hasStatisticsData) return;
            StatisticsData statisticsData = player.statisticsData.Statistics;

            if (player.time.ClientTime - statisticsData.LastSamplingTime < 1000)
            {
                return;
            }

            _startPos.Set(statisticsData.LastPosition.x, statisticsData.LastPosition.z);
            _endPos.Set(player.position.Value.x, player.position.Value.z);
            float moveDis = Vector2.Distance(_startPos, _endPos);

            //总移动距离
            statisticsData.TotalMoveDistance += moveDis;
            //载具移动距离
            if (player.IsOnVehicle())
            {
                statisticsData.VehicleMoveDistance += moveDis;
            }
            statisticsData.LastPosition = player.position.Value;
            //游泳时间
            if (player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Swim
                || player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Dive)
            {
                if (statisticsData.LastIsSwimState)
                {
                    statisticsData.SwimTime += player.time.ClientTime - statisticsData.LastSwimTime;
                }
                statisticsData.LastIsSwimState = true;
                statisticsData.LastSwimTime = player.time.ClientTime;
            }
            else
            {
                statisticsData.LastIsSwimState = false;
            }
            //治疗量（模式中获取）
            //statisticsData.CureVolume = 0;
            //加速时间（模式中获取）
            //statisticsData.AccSpeedTime = 0;
            //全副武装（模式中获取）
            //statisticsData.IsFullArmed = false;

            //最后统计时间
            statisticsData.LastSamplingTime = player.time.ClientTime;
        }
    }
}
