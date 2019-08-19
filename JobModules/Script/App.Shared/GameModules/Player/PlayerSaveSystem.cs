using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.player;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.FreeFramework.framework.trigger;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.@event;
using Core;
using Core.EntityComponent;
using Core.Enums;
using Core.Free;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Free.framework;
using System;
using UnityEngine;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerSaveSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSaveSystem));

        private Contexts _contexts;
        private int saveTime;

        public PlayerSaveSystem(Contexts contexts)
        {
            _contexts = contexts;
            if (GameRules.IsChicken(contexts.session.commonSession.RoomInfo.ModeId))
            {
                saveTime = SharedConfig.ChickenSaveNeedTime;
            }else
            {
                saveTime = SharedConfig.CommonSaveNeedTime;
            }
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity myEntity = owner.OwnerEntity as PlayerEntity;
            if (null == myEntity) return;

            var myState = myEntity.stateInterface.State;
            if (myEntity.gamePlay.IsSave || myEntity.gamePlay.IsBeSave)
            {
                PlayerEntity teamEntity = _contexts.player.GetEntityWithEntityKey(myEntity.gamePlay.SavePlayerKey);
                if (null == teamEntity)
                {
                    StopSave(myEntity, true);
                    return;
                }

                if (myEntity.time.ClientTime - myEntity.gamePlay.SaveTime >= saveTime)
                {
                    if (myEntity.gamePlay.IsSave)
                    {
                        myEntity.statisticsData.Statistics.SaveCount++;
                    }
                    else
                    {
                        myEntity.statisticsData.Statistics.BeSaveCount++;
                        myEntity.gamePlay.CurHp = myEntity.gamePlay.MaxHp * 0.1f;
                        if(SharedConfig.IsServer)
                            myEntity.gamePlay.ChangeLifeState(EPlayerLifeState.Alive, myEntity.time.ClientTime);
                    }

                    try
                    {
                        var args = _contexts.session.commonSession.FreeArgs as IEventArgs;
                        if (args != null && SharedConfig.IsServer)
                        {
                            TriggerArgs ta = new TriggerArgs();
                            ta.AddUnit("savor", (FreeData) (myEntity.gamePlay.IsSave ? myEntity.freeData.FreeData : teamEntity.freeData.FreeData));
                            ta.AddUnit("saved", (FreeData) (myEntity.gamePlay.IsSave ? teamEntity.freeData.FreeData : myEntity.freeData.FreeData));
                            args.Trigger(FreeTriggerConstant.PLAYER_RESCUE_SUCCESS, ta);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("player rescue success trigger failed", e);
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
                    if (!cmd.IsF || myState.NeedInterruptRescue() || !myEntity.gamePlay.IsLifeState(EPlayerLifeState.Alive)
                        || GetAngle(myEntity, teamEntity) > SharedConfig.MaxSaveAngle)
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

            if (cmd.IsUseAction && cmd.UseType == (int) EUseActionType.Player && myEntity.gamePlay.IsSave == false
                && myState.GetCurrentMovementState() == MovementInConfig.Idle
                && myState.GetCurrentPostureState() != PostureInConfig.Land && myState.GetCurrentPostureState() != PostureInConfig.Prone)
            {
                PlayerEntity saveEntity = _contexts.player.GetEntityWithEntityKey(new EntityKey(cmd.UseEntityId, (int)EEntityType.Player));
                if (saveEntity != null && SharedConfig.IsServer)
                {
                    PlayerAnimationAction.DoAnimation(_contexts, PlayerAnimationAction.Rescue, myEntity, true);
                    myEntity.gamePlay.IsSave = true;
                    myEntity.gamePlay.SaveTime = myEntity.time.ClientTime;
                    myEntity.gamePlay.SavePlayerKey = saveEntity.entityKey.Value;
                    saveEntity.gamePlay.IsBeSave = true;
                    saveEntity.gamePlay.SaveTime = saveEntity.time.ClientTime;
                    saveEntity.gamePlay.SavePlayerKey = myEntity.entityKey.Value;

                    SimpleProto data = FreePool.Allocate();
                    data.Key = FreeMessageConstant.CountDown;
                    data.Bs.Add(true);
                    data.Fs.Add(saveTime / 1000);
                    FreeMessageSender.SendMessage(myEntity, data);
                    FreeMessageSender.SendMessage(saveEntity, data);
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
                        playerEntity.gamePlay.IsStandPosture = false;
                        //PlayerAnimationAction.DoAnimation(_contexts, PlayerAnimationAction.Revive, playerEntity, true);
                    }
                    playerEntity.gamePlay.IsBeSave = false;
                }

                SimpleProto data = FreePool.Allocate();
                data.Key = FreeMessageConstant.CountDown;
                data.Bs.Add(false);
                data.Fs.Add(0);
                data.Bs.Add(!isInterrupted);
                if (isInterrupted) data.Ins.Add((int) ETipType.CanNotRescure);
                FreeMessageSender.SendMessage(playerEntity, data);
            }
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
