using App.Protobuf;
using App.Server.GameModules.GamePlay;
using App.Shared.Components.Player;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.@event;
using Core.EntityComponent;
using Core.Enums;
using Core.Free;
using Core.Statistics;
using Core.Utils;
using Free.framework;
using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;
using Vector3 = UnityEngine.Vector3;

namespace App.Shared.GameModules.Weapon
{
    public class BulletPlayerUtil
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletPlayerUtil));
        public static Dictionary<string, EBodyPart> Joint2BodyPart = new Dictionary<string, EBodyPart>()
        {
            {"Bip01 Head_hitbox", EBodyPart.Head},
            {"Bip01 Neck_hitbox", EBodyPart.Neck},
            {"Bip01 Spine1_hitbox", EBodyPart.Chest},
            {"Bip01 Spine_hitbox", EBodyPart.Stomach},
            {"Bip01 Pelvis_hitbox", EBodyPart.Pelvis},
            {"Bip01 L UpperArm_hitbox", EBodyPart.UpperArm},
            {"Bip01 R UpperArm_hitbox", EBodyPart.UpperArm},
            {"Bip01 L Forearm_hitbox", EBodyPart.ForeArm},
            {"Bip01 R Forearm_hitbox", EBodyPart.ForeArm},
            {"Bip01 L Hand_hitbox", EBodyPart.Hand},
            {"Bip01 R Hand_hitbox", EBodyPart.Hand},
            {"Bip01 L Thigh_hitbox", EBodyPart.Thigh},
            {"Bip01 R Thigh_hitbox", EBodyPart.Thigh},
            {"Bip01 L Calf_hitbox", EBodyPart.Calf},
            {"Bip01 R Calf_hitbox", EBodyPart.Calf},
            {"Bip01 L Foot_hitbox", EBodyPart.Foot},
            {"Bip01 R Foot_hitbox", EBodyPart.Foot},
        };

        public static EBodyPart GetBodyPartByHitBoxName(Collider collider)
        {
            EBodyPart part;
            if (Joint2BodyPart.TryGetValue(collider.name, out part))
            {
                return part;
            }
            return EBodyPart.Length;
        }

        public static void ProcessPlayerHealthDamage(Contexts contexts, IPlayerDamager damager, PlayerEntity srcPlayer, PlayerEntity playerEntity, PlayerDamageInfo damage)
        {
            IGameRule gameRule = null != damager ? damager.GameRule : null;
            DoProcessPlayerHealthDamage(contexts, gameRule, srcPlayer, playerEntity, damage);
        }

        public static void DoProcessPlayerHealthDamage(Contexts contexts, IGameRule gameRule, PlayerEntity srcPlayer,
            PlayerEntity playerEntity, PlayerDamageInfo damage)
        {
            List<PlayerEntity> teamList = OnePlayerHealthDamage(contexts, gameRule, srcPlayer, playerEntity, damage, false);
            if (null != teamList)
            {
                //队友
                foreach (PlayerEntity other in teamList)
                {
                    PlayerDamageInfo damageInfo = new PlayerDamageInfo(other.gamePlay.InHurtedHp, (int)EUIDeadType.NoHelp, (int)EBodyPart.Chest, 0, false, false, true);
                    OnePlayerHealthDamage(contexts, gameRule, null, other, damageInfo, true);
                }
            }
        }

        private static List<PlayerEntity> OnePlayerHealthDamage(Contexts contexts, IGameRule gameRule, PlayerEntity srcPlayer, PlayerEntity playerEntity, PlayerDamageInfo damage, bool isTeam)
        {
            GamePlayComponent gamePlay = playerEntity.gamePlay;
            if (gamePlay.IsDead()) return null;

            float realDamage = gameRule == null ? damage.damage : gameRule.HandleDamage(srcPlayer, playerEntity, damage);

            WeaponResConfigItem weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(damage.weaponId);

            if (srcPlayer != null)
            {
                try
                {
                    //受伤梯形标记
                    if (SharedConfig.IsServer)
                    {
                        if (damage.type == (int)EUIDeadType.Weapon || damage.type == (int)EUIDeadType.Unarmed)
                        {
                            if (weaponConfig != null && weaponConfig.SubType != (int)EWeaponSubType.Grenade)
                            {
                                BulletStatisticsUtil.SetPlayerDamageInfoS(srcPlayer, playerEntity, realDamage, (EBodyPart)damage.part);
                            }
                        }
                    }
                    else
                    {
                        BulletStatisticsUtil.SetPlayerDamageInfoC(srcPlayer, playerEntity, realDamage, (EBodyPart)damage.part);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("受伤梯形标记", e);
                }
            }

            if (!SharedConfig.IsOffline && !SharedConfig.IsServer)
                return null;
            
            if (!playerEntity.hasStatisticsData)
                return null;
            
            StatisticsData statisticsData = playerEntity.statisticsData.Statistics;
            

            var now = gameRule == null ? playerEntity.time.ClientTime : gameRule.ServerTime;

            if (now - statisticsData.LastHitDownTime <= 1000 && !damage.InstantDeath)
                return null;

            damage.damage = gamePlay.DecreaseHp(realDamage);

            //玩家状态
            List<PlayerEntity> teamList = CheckUpdatePlayerStatus(playerEntity, damage, isTeam ? null : contexts, (int) now);

            //保存最后伤害来源
            if (statisticsData.DataCollectSwitch)
            {
                try
                {
                    if (gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
                    {
                        //statisticsData.IsHited = true;
                        statisticsData.LastHurtKey = null != srcPlayer ? srcPlayer.entityKey.Value : EntityKey.Default;
                        statisticsData.LastHurtType = damage.type;
                        statisticsData.LastHurtPart = damage.part;
                        statisticsData.LastHurtWeaponId = damage.weaponId;
                    }

                    //击倒人头
                    if ((gamePlay.IsLifeState(EPlayerLifeState.Dying) || gamePlay.IsDead()) &&
                        (damage.type == (int)EUIDeadType.NoHelp || damage.type == (int)EUIDeadType.Poison || damage.type == (int)EUIDeadType.Bomb ||
                        damage.type == (int)EUIDeadType.Drown || damage.type == (int)EUIDeadType.Bombing || damage.type == (int)EUIDeadType.Fall))
                    {
                        PlayerEntity lastEntity = contexts.player.GetEntityWithEntityKey(statisticsData.LastHurtKey);
                        if (null != lastEntity)
                        {
                            srcPlayer = lastEntity;
                            if (srcPlayer.playerInfo.TeamId == playerEntity.playerInfo.TeamId)
                            {
                                damage.type = statisticsData.LastHurtType;
                                damage.part = statisticsData.LastHurtPart;
                                damage.weaponId = statisticsData.LastHurtWeaponId;
                            }
                            else
                            {
                                damage.type = (int)EUIDeadType.NoHelp;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("计算玩家战报信息失败", e);
                }

                if (gamePlay.IsHitDown())
                {
                    try
                    {
                        statisticsData.LastHitDownTime = now;
                        SimpleProto message = FreePool.Allocate();
                        message.Key = FreeMessageConstant.ScoreInfo;
                        int feedbackType = 0;
                        message.Ks.Add(3);
                        message.Bs.Add(true);
                        if (null != srcPlayer)
                        {
                            if (srcPlayer.playerInfo.TeamId != playerEntity.playerInfo.TeamId)
                            {
                                feedbackType |= 1 << (int)EUIKillFeedbackType.Hit;
                            }
                            message.Ss.Add(srcPlayer.playerInfo.PlayerName);
                            message.Ds.Add(srcPlayer.playerInfo.TeamId);
                            message.Ins.Add(damage.weaponId);
                        }
                        else
                        {
                            message.Ss.Add("");
                            message.Ds.Add(-1);
                            message.Ins.Add(0);
                        }
                        message.Ins.Add((int)EUIKillType.Hit);
                        message.Ins.Add(feedbackType);
                        message.Ss.Add(playerEntity.playerInfo.PlayerName);
                        message.Ds.Add(playerEntity.playerInfo.TeamId);
                        message.Ins.Add(damage.type);
                        SendMessageAction.sender.SendMessage(contexts.session.commonSession.FreeArgs as IEventArgs, message, 4, string.Empty);
                    }
                    catch (Exception e)
                    {
                        _logger.Error("计算玩家ScoreInfo信息失败", e);
                    }
                }

                if (gamePlay.IsDead())
                {
                    try
                    {
                        //UI击杀信息
                        int killType = 0;
                        if (damage.part == (int)EBodyPart.Head)
                        {
                            killType |= (int)EUIKillType.Crit;
                        }
                        damage.KillType = killType;
                        playerEntity.playerInfo.SpecialFeedbackType = 0;
                        //UI击杀反馈
                        if (null != srcPlayer && srcPlayer.playerInfo.TeamId != playerEntity.playerInfo.TeamId)
                        {
                            int feedbackType = 0;
                            if (damage.part == (int)EBodyPart.Head)
                            {
                                //爆头
                                feedbackType |= 1 << (int)EUIKillFeedbackType.CritKill;
                            }
                            if (damage.IsOverWall)
                            {
                                //穿墙击杀
                                feedbackType |= 1 << (int)EUIKillFeedbackType.ThroughWall;
                            }
                            if (SharedConfig.IsServer && null != gameRule && contexts.session.serverSessionObjects.DeathOrder == 0 && srcPlayer.playerInfo.TeamId != playerEntity.playerInfo.TeamId)
                            {
                                //一血
                                feedbackType |= 1 << (int)EUIKillFeedbackType.FirstBlood;
                            }
                            if (playerEntity.playerInfo.PlayerId == srcPlayer.statisticsData.Statistics.RevengeKillerId)
                            {
                                //复仇
                                feedbackType |= 1 << (int)EUIKillFeedbackType.Revenge;
                                srcPlayer.statisticsData.Statistics.RevengeKillerId = 0L;
                            }
                            if (srcPlayer.gamePlay.JobAttribute == (int)EJobAttribute.EJob_Hero)
                            {
                                //英雄击杀
                                feedbackType |= 1 << (int)EUIKillFeedbackType.HeroKO;
                                playerEntity.playerInfo.SpecialFeedbackType = (int)EUIKillFeedbackType.HeroKO;
                            }
                            //武器
                            if (null != weaponConfig)
                            {
                                switch ((EWeaponSubType) weaponConfig.SubType)
                                {
                                    case EWeaponSubType.Melee:
                                        feedbackType |= 1 << (int)EUIKillFeedbackType.MeleeWeapon;
                                        playerEntity.playerInfo.SpecialFeedbackType = (int)EUIKillFeedbackType.MeleeWeapon;
                                        break;
                                    case EWeaponSubType.BurnBomb:
                                        feedbackType |= 1 << (int)EUIKillFeedbackType.Burning;
                                        break;
                                    case EWeaponSubType.Grenade:
                                        feedbackType |= 1 << (int)EUIKillFeedbackType.Grenade;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (feedbackType == 0)
                            {
                                //普通击杀
                                feedbackType = 1 << (int)EUIKillFeedbackType.Normal;
                            }
                            damage.KillFeedbackType = feedbackType;
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.Error("计算玩家战报信息失败", e);
                    }
                }

                //数据统计
                ProcessDamageStatistics(contexts, gameRule, srcPlayer, playerEntity, damage);
            }

            //击杀|击倒
            if (gamePlay.IsDead())
            {
                gameRule.KillPlayer(srcPlayer, playerEntity, damage);
                try
                {
                    if (damage.HitPoint != Vector3.zero && damage.HitDirection != Vector3.zero && playerEntity.hasRagDoll)
                    {
                        playerEntity.ragDoll.ForceAtPosition = Vector3.zero;
                        switch (damage.type)
                        {
                            case (int) EUIDeadType.Weapon:
                                var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(damage.weaponId);
                                if (weaponConfig.Type == (int) EWeaponType_Config.ThrowWeapon)
                                {
                                    playerEntity.ragDoll.RigidBodyTransformName = "Bip01 Spine1";
                                    playerEntity.ragDoll.Impulse = (damage.HitDirection.normalized + new Vector3(0, 1.732f)).normalized *
                                        config.S_Ragdoll.RagdollForce * DamageFactor(realDamage);
                                }
                                else
                                {
                                    foreach (var pair in Joint2BodyPart)
                                    {
                                        if (pair.Value == (EBodyPart) damage.part)
                                        {
                                            playerEntity.ragDoll.RigidBodyTransformName = pair.Key.Substring(0, pair.Key.LastIndexOf("_"));
                                            break;
                                        }
                                    }

                                    playerEntity.ragDoll.Impulse = damage.HitDirection.normalized * config.S_Ragdoll.RagdollForce *
                                        DamageFactor(realDamage);
                                }

                                break;
                            default:
                                var explosionConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(46);
                                playerEntity.ragDoll.RigidBodyTransformName = "Bip01 Spine1";
                                playerEntity.ragDoll.Impulse = (damage.HitDirection.normalized + new Vector3(0, 1.732f)).normalized *
                                    explosionConfig.S_Ragdoll.RagdollForce * DamageFactor(realDamage);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("计算玩家死亡ragdoll运动信息失败", e);
                }
            }

            //_logger.DebugFormat("change player hp entityId:{0}, health {1}->{2}, state {3}, srcPlayerId:{4}, playerId:{5}, hurtType:{6}, weaponId:{7}",
            //    playerEntity.entityKey.Value.EntityId, playerEntity.gamePlay.CurHp, playerEntity.gamePlay.CurHp, playerEntity.gamePlay.LifeState, (srcPlayer != null) ? srcPlayer.playerInfo.PlayerId : 0, playerEntity.playerInfo.PlayerId, damage.type, damage.weaponId);
            return teamList;
        }

        private static List<PlayerEntity> CheckUpdatePlayerStatus(PlayerEntity player, PlayerDamageInfo damage, Contexts contexts, int now)
        {
            GamePlayComponent gamePlay = player.gamePlay;
            if (contexts == null && damage.InstantDeath)
            {
                gamePlay.ChangeLifeState(EPlayerLifeState.Dead, now);
                return null;
            }

            if (contexts.session.commonSession.RoomInfo.TeamCapacity <= 1)
            {
                if (damage.InstantDeath || gamePlay.CurHp <= 0) gamePlay.ChangeLifeState(EPlayerLifeState.Dead, now);
                return null;
            }
            else
            {
                switch (gamePlay.LifeState)
                {
                    case (int) EPlayerLifeState.Alive:
                        if (gamePlay.CurHp <= 0 || damage.InstantDeath)
                        {
                            int aliveCount = 0;
                            List<PlayerEntity> teamList = new List<PlayerEntity>();
                            foreach (PlayerEntity other in contexts.player.GetInitializedPlayerEntities())
                            {
                                if (!other.hasGamePlay) continue;
                                if (other.playerInfo.PlayerId != player.playerInfo.PlayerId && other.playerInfo.TeamId == player.playerInfo.TeamId)
                                {
                                    switch (other.gamePlay.LifeState)
                                    {
                                        case (int) EPlayerLifeState.Alive:
                                            aliveCount++;
                                            break;
                                        case (int) EPlayerLifeState.Dying:
                                            teamList.Add(other);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            if (damage.InstantDeath || damage.type == (int) EUIDeadType.Bomb
                                || damage.type == (int) EUIDeadType.Drown
                                || player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Swim
                                || player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Dive)
                            {
                                gamePlay.ChangeLifeState(EPlayerLifeState.Dead, now);
                                return aliveCount == 0 ? teamList : null;
                            }

                            if(aliveCount == 0) gamePlay.ChangeLifeState(EPlayerLifeState.Dead, now);
                            else gamePlay.ChangeLifeState(EPlayerLifeState.Dying, now);
                            return aliveCount == 0 ? teamList : null;
                        }
                        break;
                    case (int) EPlayerLifeState.Dying:
                        if (damage.InstantDeath || gamePlay.InHurtedHp <= 0) gamePlay.ChangeLifeState(EPlayerLifeState.Dead, now);
                        break;
                    default:
                        break;
                }

                return null;
            }
        }

        public static void ProcessDamageStatistics(Contexts contexts, IGameRule gameRule, PlayerEntity srcPlayer, PlayerEntity targetPlayer, PlayerDamageInfo damage)
        {
            if (null == targetPlayer) return;

            bool isTargetDead = targetPlayer.gamePlay.IsDead();

            if (isTargetDead)
            {
                targetPlayer.isFlagCompensation = false;
                targetPlayer.AudioController().PlayDeadAudio();
            }

            var now = gameRule == null ? targetPlayer.time.ClientTime : gameRule.ServerTime;
            //攻击者
            if (null != srcPlayer)
            {
                bool isTeammate = srcPlayer.playerInfo.TeamId == targetPlayer.playerInfo.TeamId;
                
                bool isKill = isTargetDead;
                bool isHitDown = targetPlayer.gamePlay.IsHitDown();
                bool isCrit = damage.part == (int) EBodyPart.Head;
                var srcStat = srcPlayer.statisticsData.Statistics;

                targetPlayer.statisticsData.AddOtherInfo(srcPlayer.entityKey.Value, damage.weaponId, isKill, isHitDown, damage.damage, srcPlayer.playerInfo, now);

                //添加别人对自己的伤害记录（受伤不算）
                if (targetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Alive) && !isTeammate)
                {
                    //添加自己对别人的伤害记录
                    srcPlayer.statisticsData.AddOpponentInfo(targetPlayer.entityKey.Value, damage.weaponId, isKill, isHitDown, damage.damage, targetPlayer.playerInfo, targetPlayer.statisticsData.Statistics.DeadCount);
                    //总伤害量
                    srcStat.TotalDamage += damage.damage;
                    //有效伤害
                    srcStat.PlayerDamage += damage.damage;
                }

                if (isTargetDead)
                {
                    if (!isTeammate)
                    {
                        //击杀数
                        srcStat.KillCount++;
                        srcStat.LastKillTime = now;
                        //爆头击杀（不包括近战）
                        if (isCrit && !damage.IsKnife) srcStat.CritKillCount++;

                        //连杀数
                        srcStat.EvenKillCount++;
                        if (srcStat.EvenKillCount > srcStat.MaxEvenKillCount) srcStat.MaxEvenKillCount = srcStat.EvenKillCount;

                        //最大击杀距离
                        srcStat.MaxKillDistance = Mathf.Max(srcStat.MaxKillDistance, Vector3.Distance(srcPlayer.position.Value, targetPlayer.position.Value));

                        if (damage.WeaponType == EWeaponSubType.Pistol) srcStat.PistolKillCount++; //手枪击杀
                        else if (damage.WeaponType == EWeaponSubType.Grenade) srcStat.GrenadeKillCount++;//手雷击杀

                        if (targetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Alive)) srcStat.HitDownCount++;
                    }
                    else
                    {
                        srcStat.KillTeamCount++;//击杀队友
                    }
                }

                //击倒数
                if (isHitDown && !isTeammate) srcStat.HitDownCount++;

                //总爆头数（不包括近战）
                if (isCrit && !damage.IsKnife && !isTeammate) srcStat.CritCount++;
            }

            //受击者
            var tarStat = targetPlayer.statisticsData.Statistics;
            //承受伤害量
            tarStat.TotalBeDamage += damage.damage;
            if (isTargetDead)
            {
                //死亡次数
                tarStat.DeadCount++;
                tarStat.LastDeadTime = now;
                if (null == srcPlayer)
                {
                    targetPlayer.statisticsData.SetDeadType(damage.type);
                }
                else
                {
                    targetPlayer.statisticsData.AddKillerInfo(srcPlayer.entityKey.Value, damage.weaponId, damage.type, srcPlayer.playerInfo);
                    //tarStat.AliveTime = (int)(gameRule.ServerTime - gameRule.GameStartTime) / 1000;
                    if (srcPlayer.playerInfo.TeamId != targetPlayer.playerInfo.TeamId)
                    {
                        contexts.session.serverSessionObjects.DeathOrder++;
                        tarStat.DeathOrder = contexts.session.serverSessionObjects.DeathOrder;
                        if (tarStat.DeathOrder == 1)
                        {
                            srcPlayer.statisticsData.Statistics.GetFirstBlood = true;
                        }
                    }
                    else
                    {
                        tarStat.DeathOrder = -1;
                    }
                }

                foreach (var other in targetPlayer.statisticsData.Battle.OtherDict.Values)
                {
                    PlayerEntity otherEntity = contexts.player.GetEntityWithEntityKey(other.PlayerKey);
                    if (null != otherEntity)
                    {
                        if (otherEntity.playerInfo.TeamId == targetPlayer.playerInfo.TeamId)
                            continue;

                        bool assistFlag = false;
                        if (null == srcPlayer)
                        {
                            if (now - other.timestamp <= 10000)
                                assistFlag = true;
                        }
                        else
                        {
                            if (other.PlayerKey == srcPlayer.entityKey.Value)
                                continue;

                            if (srcPlayer.playerInfo.TeamId == otherEntity.playerInfo.TeamId)
                            {
                                if (now - other.timestamp <= 10000)
                                    assistFlag = true;
                            }
                        }

                        if (assistFlag)
                        {
                            otherEntity.statisticsData.Statistics.AssistCount++;
                            int feedbackType = 0;
                            feedbackType |= 1 << (int) EUIKillFeedbackType.Cooperate;
                            SimpleProto message = FreePool.Allocate();
                            message.Key = FreeMessageConstant.ScoreInfo;
                            message.Ks.Add(3);
                            message.Bs.Add(false);
                            message.Ss.Add(otherEntity.playerInfo.PlayerName);
                            message.Ins.Add(0);
                            message.Ins.Add(0);
                            message.Ins.Add(feedbackType);
                            FreeMessageSender.SendMessage(otherEntity, message);
                        }
                    }
                }

                //Send Message
                if (SharedConfig.IsServer)
                {
                    SendStatisticsMessage(targetPlayer);
                }
                else if (SharedConfig.IsOffline)
                {
                    targetPlayer.statisticsData.IsShow = true;
                }
            }
        }

        public static void SendStatisticsMessage(PlayerEntity targetPlayer)
        {
            BattleStatisticsMessage message = BattleStatisticsMessage.Allocate();

            if (targetPlayer.hasStatisticsData)
            {
                BattleData data = targetPlayer.statisticsData.Battle;

                KillerInfo killer = KillerInfo.Allocate();
                killer.PlayerLv = data.Killer.PlayerLv;
                killer.PlayerName = data.Killer.PlayerName;
                killer.BackId = data.Killer.BackId;
                killer.TitleId = data.Killer.TitleId;
                killer.BadgeId = data.Killer.BadgeId;
                killer.WeaponId = data.Killer.WeaponId;
                killer.DeadType = (int)data.Killer.DeadType;
                message.Killer = killer;

                foreach (var opponent in data.OpponentList)
                {
                    OpponentInfo info = OpponentInfo.Allocate();
                    info.PlayerLv = opponent.PlayerLv;
                    info.PlayerName = opponent.PlayerName;
                    info.BackId = opponent.BackId;
                    info.TitleId = opponent.TitleId;
                    info.BadgeId = opponent.BadgeId;
                    info.WeaponId = opponent.WeaponId;
                    info.IsKill = opponent.IsKill;
                    info.Damage = opponent.Damage;
                    message.Opponents.Add(info);
                }
            }
            targetPlayer.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.Statistics, message);
        }

        private static float DamageFactor(float damage)
        {
            if (damage > 0 && damage < 31)
                return 0.75f;
            if (damage >= 31)
                return 1;
            return 0;
        }
    }
}