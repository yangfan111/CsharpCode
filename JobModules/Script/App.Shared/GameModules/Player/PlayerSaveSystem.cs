using App.Server.GameModules.GamePlay.Free.player;
using App.Shared.Components.Player;
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

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity myEntity = owner.OwnerEntity as PlayerEntity;
            if (null == myEntity) return;
            if (myEntity.gamePlay.IsSave || myEntity.gamePlay.IsBeSave)
            {
                PlayerEntity teamEntity = _contexts.player.GetEntityWithEntityKey(myEntity.gamePlay.SavePlayerKey);
                if (null == teamEntity)
                {
                    StopSave(myEntity, true);
                    return;
                }

                if (myEntity.time.ClientTime - myEntity.gamePlay.SaveTime >= SharedConfig.SaveNeedTime)
                {
                    if (myEntity.gamePlay.IsSave)
                    {
                        myEntity.statisticsData.Statistics.SaveCount++;
                    }else{
                        myEntity.statisticsData.Statistics.BeSaveCount++;
                        myEntity.gamePlay.CurHp = myEntity.gamePlay.MaxHp * 0.1f;
                        myEntity.gamePlay.ChangeLifeState(EPlayerLifeState.Alive, myEntity.time.ClientTime);
                    }
                    StopSave(myEntity, false);
                    return;
                }
                if (Vector3.Distance(myEntity.position.Value, teamEntity.position.Value) > SharedConfig.MaxSaveDistance)
                {
                    StopSave(myEntity, true);
                    StopSave(teamEntity, true);
                    return;
                }
                if (myEntity.gamePlay.IsSave)
                {
                    if (myEntity.stateInterface.State.NeedInterruptRescue((PostureInConfig) myEntity.gamePlay.SaveEnterState)
                        || !myEntity.gamePlay.IsLifeState(EPlayerLifeState.Alive) || GetAngle(myEntity, teamEntity) > SharedConfig.MaxSaveAngle)
                    {
                        StopSave(myEntity, true);
                        StopSave(teamEntity, true);
                        return;
                    }
                }
                else
                {
                    if (myEntity.gamePlay.IsDead())
                    {
                        StopSave(myEntity, true);
                        StopSave(teamEntity, true);
                        return;
                    }
                }
            }
            if (cmd.IsUseAction && cmd.UseType == (int) EUseActionType.Player)
            {
                PlayerEntity saveEntity = _contexts.player.GetEntityWithEntityKey(new EntityKey(cmd.UseEntityId, (int)EEntityType.Player));
                if (saveEntity != null && SharedConfig.IsServer)
                {
                    PlayerAnimationAction.DoAnimation(_contexts, PlayerAnimationAction.Rescue, myEntity, true);
                    myEntity.gamePlay.IsSave = true;
                    myEntity.gamePlay.SaveEnterState = (int) myEntity.stateInterface.State.GetCurrentPostureState();
                    myEntity.gamePlay.SaveTime = myEntity.time.ClientTime;
                    myEntity.gamePlay.SavePlayerKey = saveEntity.entityKey.Value;
                    saveEntity.gamePlay.IsBeSave = true;
                    saveEntity.gamePlay.SaveTime = saveEntity.time.ClientTime;
                    saveEntity.gamePlay.SavePlayerKey = myEntity.entityKey.Value;
                }
            }
        }

        private void StopSave(PlayerEntity playerEntity, bool isInterrupted)
        {
            if (SharedConfig.IsServer)
            {
                playerEntity.gamePlay.SaveTime = 0;
                if (playerEntity.gamePlay.IsSave)
                {
                    PlayerAnimationAction.DoAnimation(_contexts, PlayerAnimationAction.RescueEnd, playerEntity, true);
                    playerEntity.gamePlay.IsSave = false;
                }
                if (playerEntity.gamePlay.IsBeSave)
                {
                    if (!isInterrupted)
                    {
                        PlayerAnimationAction.DoAnimation(_contexts, PlayerAnimationAction.Revive, playerEntity, true);
                    }
                    playerEntity.gamePlay.IsBeSave = false;
                }
            }
            playerEntity.gamePlay.IsInteruptSave = isInterrupted;
        }

        private float GetAngle(PlayerEntity myEntity, PlayerEntity teamEntity)
        {
            //角度
            float yaw = CommonMathUtil.TransComAngle(myEntity.orientation.Yaw);
            float angle = CommonMathUtil.GetAngle(new Vector2(teamEntity.position.Value.x, teamEntity.position.Value.z), new Vector2(myEntity.position.Value.x, myEntity.position.Value.z));
            return CommonMathUtil.GetDiffAngle(angle, yaw);
        }
    }
}
