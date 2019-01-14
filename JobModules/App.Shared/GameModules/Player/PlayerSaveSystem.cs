using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components.Player;
using App.Shared.FreeFramework.framework.util;
using App.Shared.GameModules.Bullet;
using com.wd.free.@event;
using Core.Common;
using Core.EntityComponent;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerSaveSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSaveSystem));

        private Contexts _contexts;

        public PlayerSaveSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        private void StopSave(PlayerEntity playerEntity, bool isInteruptSave = false)
        {
            playerEntity.gamePlay.IsSave = false;
            playerEntity.gamePlay.SaveTime = 0;
            playerEntity.stateInterface.State.RescueEnd();
            if (playerEntity.gamePlay.SaveEnterState == (int) playerEntity.stateInterface.State.GetCurrentPostureState()
                && playerEntity.gamePlay.SaveEnterState != (int)PostureInConfig.Crouch)
            {
                playerEntity.stateInterface.State.Crouch();
            }

            if (isInteruptSave)
            {
                SetRescureInterupt(playerEntity);
            }
        }

        private void StopBeSave(PlayerEntity playerEntity, bool isInteruptSave = false)
        {
            playerEntity.gamePlay.IsBeSave = false;
            playerEntity.gamePlay.SaveTime = 0;
            if (isInteruptSave)
            {
                SetRescureInterupt(playerEntity);
            }
        }

        private float GetAngle(PlayerEntity myEntity, PlayerEntity teamEntity)
        {
            //角度
            float yaw = CommonMathUtil.TransComAngle(myEntity.orientation.Yaw);
            float angle = CommonMathUtil.GetAngle(new Vector2(teamEntity.position.Value.x, teamEntity.position.Value.z), new Vector2(myEntity.position.Value.x, myEntity.position.Value.z));
            return CommonMathUtil.GetDiffAngle(angle, yaw);
        }

        private void SetRescureInterupt(PlayerEntity entity)
        {
            entity.gamePlay.IsInteruptSave = true;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity myEntity = owner.OwnerEntity as PlayerEntity;
            if (null == myEntity) return;

            if (myEntity.gamePlay.IsSave)
            {
                if (myEntity.time.ClientTime - myEntity.gamePlay.SaveTime >= SharedConfig.SaveNeedTime)
                {
                    //正常结束
                    StopSave(myEntity);
                    //统计救援人数
                    myEntity.statisticsData.Statistics.SaveCount++;
                    return;
                }
                //打断救援（被救援者已不在）
                PlayerEntity teamEntity = _contexts.player.GetEntityWithEntityKey(myEntity.gamePlay.SavePlayerKey);
                if (null == teamEntity)
                {
                    StopSave(myEntity, true);
                    return;
                }
                //打断救援（除QE之外的任何动作 或 自己进入受伤/死亡状态）
                if (myEntity.stateInterface.State.NeedInterruptRescue((PostureInConfig)myEntity.gamePlay.SaveEnterState)
                    || !myEntity.gamePlay.IsLifeState(EPlayerLifeState.Alive))
                {
                    StopSave(myEntity, true);
                    if (teamEntity.gamePlay.IsBeSave)
                    {
                        StopBeSave(teamEntity, true);
                    }
                }
            }
            else if (myEntity.gamePlay.IsBeSave)
            {
                if (myEntity.time.ClientTime - myEntity.gamePlay.SaveTime >= SharedConfig.SaveNeedTime)
                {
                    //正常结束
                    StopBeSave(myEntity);
                    //血量恢复10%
                    myEntity.gamePlay.CurHp = (int)(myEntity.gamePlay.MaxHp * 0.1f);
                    myEntity.gamePlay.ChangeLifeState(EPlayerLifeState.Alive, myEntity.time.ClientTime);
                    //蹲下状态
//                    myEntity.stateInterface.State.Crouch();
                    return;
                }
                //打断救援（救援者已不在）
                PlayerEntity teamEntity = _contexts.player.GetEntityWithEntityKey(myEntity.gamePlay.SavePlayerKey);
                if (null == teamEntity)
                {
                    StopBeSave(myEntity, true);
                    return;
                }
                //打断救援（超出距离角度 或 自己已死亡）
                if (myEntity.gamePlay.IsDead()
                    || Vector3.Distance(myEntity.position.Value, teamEntity.position.Value) > SharedConfig.MaxSaveDistance
                    || GetAngle(teamEntity ,myEntity) > SharedConfig.MaxSaveAngle)
                {
                    StopBeSave(myEntity, true);
                    if (teamEntity.gamePlay.IsSave)
                    {
                        StopSave(teamEntity, true);
                    }
                }
            }
            else if (cmd.IsUseAction && cmd.UseType == (int) EUseActionType.Player)
            {
                //趴下不能救援
                if (myEntity.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Prone)
                {
                    //TODO 弹出提示
                    if (SharedConfig.IsServer)
                    {
                        IEventArgs args = (IEventArgs) _contexts.session.commonSession.FreeArgs;
                        args.TempUse("current", (FreeData)myEntity.freeData.FreeData);
                        FuntionUtil.Call(args, "showBottomTip", "msg", "{desc:10077}");
                        args.Resume("current");
                    }
                    else
                    {
                        myEntity.tip.TipType = ETipType.CanNotRescure;
                    }
                    return;
                }
                //实施救援
                PlayerEntity teamEntity = _contexts.player.GetEntityWithEntityKey(new EntityKey(cmd.UseEntityId, (int)EEntityType.Player));
                if (null != teamEntity)
                {
                    myEntity.gamePlay.SaveEnterState = (int)myEntity.stateInterface.State.GetCurrentPostureState();
                    myEntity.gamePlay.IsSave = true;
                    myEntity.gamePlay.SaveTime = myEntity.time.ClientTime;
                    myEntity.gamePlay.SavePlayerKey = teamEntity.entityKey.Value;
                    //救援动作
                    myEntity.stateInterface.State.Rescue();

                    teamEntity.gamePlay.IsBeSave = true;
                    teamEntity.gamePlay.SaveTime = teamEntity.time.ClientTime;
                    teamEntity.gamePlay.SavePlayerKey = myEntity.entityKey.Value;
                }
            }
        }
    }
}
