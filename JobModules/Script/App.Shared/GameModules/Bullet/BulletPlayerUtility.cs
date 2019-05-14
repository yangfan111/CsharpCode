using App.Protobuf;
using App.Server.GameModules.GamePlay;
using App.Shared.Components.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.@event;
using com.wd.free.util;
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

namespace App.Shared.GameModules.Bullet
{
    public class BulletPlayerUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletPlayerUtility));
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
            DoProcessPlayerHealthDamage(contexts, damager, srcPlayer, playerEntity, damage, null);
        }

        public static void ProcessPlayerHealthDamage(Contexts contexts, IPlayerDamager damager, PlayerEntity srcPlayer, PlayerEntity playerEntity, PlayerDamageInfo damage, IDamageInfoCollector damageInfoCollector)
        {
            if(null == damageInfoCollector)
            {
                _logger.Error("damageInfoCollector is null");
            }
            DoProcessPlayerHealthDamage(contexts, damager, srcPlayer, playerEntity, damage, damageInfoCollector);
        }

        public static void DoProcessPlayerHealthDamage(Contexts contexts, IPlayerDamager damager, PlayerEntity srcPlayer, PlayerEntity playerEntity, PlayerDamageInfo damage, IDamageInfoCollector damageInfoCollector)
        {
            IGameRule gameRule = null != damager ? damager.GameRule : null;
            DoProcessPlayerHealthDamage(contexts, gameRule, srcPlayer, playerEntity, damage, damageInfoCollector);
        }

        public static void DoProcessPlayerHealthDamage(Contexts contexts, IGameRule gameRule, PlayerEntity srcPlayer,
            PlayerEntity playerEntity, PlayerDamageInfo damage, IDamageInfoCollector damageInfoCollector)
        {
            List<PlayerEntity> teamList = OnePlayerHealthDamage(contexts, gameRule, srcPlayer, playerEntity, damage, damageInfoCollector, false);
            if (null != teamList)
            {
                //队友
                foreach (PlayerEntity other in teamList)
                {
                    PlayerDamageInfo damageInfo = new PlayerDamageInfo(other.gamePlay.InHurtedHp, (int)EUIDeadType.NoHelp, (int)EBodyPart.Chest, 0);
                    OnePlayerHealthDamage(contexts, gameRule, null, other, damageInfo, damageInfoCollector, true);
                }
            }
        }

        private static List<PlayerEntity> OnePlayerHealthDamage(Contexts contexts, IGameRule gameRule, PlayerEntity srcPlayer, PlayerEntity playerEntity, PlayerDamageInfo damage, IDamageInfoCollector damageInfoCollector, bool isTeam)
        {
            if (playerEntity.gamePlay.IsDead())
                return null;

            if ((DateTime.Now.Ticks/10000L) - playerEntity.statisticsData.Statistics.LastHitDownTime <= 1000 && !damage.InstantDeath)
                return null;

            float curHp = playerEntity.gamePlay.CurHp;
            float realDamage = damage.damage;

            if(gameRule != null)
            {
                realDamage = gameRule.HandleDamage(srcPlayer, playerEntity, damage);
            }
            if(null != damageInfoCollector)
            {
                damageInfoCollector.SetPlayerDamageInfo(srcPlayer, playerEntity, realDamage, (EBodyPart)damage.part);
            }
            if (!SharedConfig.IsOffline && !SharedConfig.IsServer)
            {
                return null;
            }
            
            float ret = playerEntity.gamePlay.DecreaseHp(realDamage);
            _logger.InfoFormat("[hit] after CurrHp :"+playerEntity.gamePlay.CurHp);
            damage.damage = ret;

            //玩家状态
            List<PlayerEntity> teamList = CheckUpdatePlayerStatus(playerEntity, damage, !isTeam && gameRule != null ? gameRule.Contexts : null);

            //保存最后伤害来源
            StatisticsData statisticsData = playerEntity.statisticsData.Statistics;
            if (statisticsData.DataCollectSwitch)
            {
                if (playerEntity.gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
                {
                    statisticsData.IsHited = true;
                    if (null != srcPlayer)
                    {
                        statisticsData.LastHurtKey = srcPlayer.entityKey.Value;
                    }
                    else
                    {
                        statisticsData.LastHurtKey = EntityKey.Default;
                    }
                    statisticsData.LastHurtType = damage.type;
                    statisticsData.LastHurtPart = damage.part;
                    statisticsData.LastHurtWeaponId = damage.weaponId;
                }

                //击倒人头
                if (statisticsData.IsHited && (damage.type == (int) EUIDeadType.NoHelp || damage.type == (int) EUIDeadType.Poison || damage.type == (int) EUIDeadType.Bomb ||
                    damage.type == (int) EUIDeadType.Drown || damage.type == (int) EUIDeadType.Bombing || damage.type == (int) EUIDeadType.Fall))
                {
                    if (gameRule != null)
                    {
                        PlayerEntity lastEntity = gameRule.Contexts.player.GetEntityWithEntityKey(statisticsData.LastHurtKey);
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
                                damage.type = (int) EUIDeadType.NoHelp;
                            }
                        }
                    }
                }

                if (playerEntity.gamePlay.IsHitDown())
                {
                    statisticsData.LastHitDownTime = (DateTime.Now.Ticks / 10000L);
                    SimpleProto message = FreePool.Allocate();
                    message.Key = FreeMessageConstant.ScoreInfo;
                    int feedbackType = 0;
                    message.Ks.Add(3);
                    message.Bs.Add(true);
                    if (null != srcPlayer)
                    {
                        if (srcPlayer.playerInfo.TeamId != playerEntity.playerInfo.TeamId)
                        {
                            feedbackType |= 1 << (int) EUIKillFeedbackType.Hit;
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
                    message.Ins.Add((int) EUIKillType.Hit);
                    message.Ins.Add(feedbackType);
                    message.Ss.Add(playerEntity.playerInfo.PlayerName);
                    message.Ds.Add(playerEntity.playerInfo.TeamId);
                    message.Ins.Add(damage.type);
                    SendMessageAction.sender.SendMessage(contexts.session.commonSession.FreeArgs as IEventArgs, message, 4, string.Empty);
                }

                if (playerEntity.gamePlay.IsDead())
                {
                    //UI击杀信息
                    int killType = 0;
                    if (damage.part == (int) EBodyPart.Head)
                    {
                        killType |= (int) EUIKillType.Crit;
                    }
                    damage.KillType = killType;
                    playerEntity.playerInfo.SpecialFeedbackType = 0;
                    //UI击杀反馈
                    if (null != srcPlayer && srcPlayer.playerInfo.TeamId != playerEntity.playerInfo.TeamId)
                    {
                        int feedbackType = 0;
                        if (damage.part == (int) EBodyPart.Head)
                        {
                            //爆头
                            feedbackType |= 1 << (int) EUIKillFeedbackType.CritKill;
                        }
                        if (damage.IsOverWall)
                        {
                            //穿墙击杀
                            feedbackType |= 1 << (int) EUIKillFeedbackType.ThroughWall;
                        }
                        if (SharedConfig.IsServer && null != gameRule && gameRule.Contexts.session.serverSessionObjects.DeathOrder == 0 && srcPlayer.playerInfo.TeamId != playerEntity.playerInfo.TeamId)
                        {
                            //一血
                            feedbackType |= 1 << (int) EUIKillFeedbackType.FirstBlood;
                        }
                        if (playerEntity.playerInfo.PlayerId == srcPlayer.statisticsData.Statistics.RevengeKillerId)
                        {
                            //复仇
                            feedbackType |= 1 << (int) EUIKillFeedbackType.Revenge;
                            srcPlayer.statisticsData.Statistics.RevengeKillerId = 0L;
                        }
                        if (srcPlayer.playerInfo.JobAttribute == (int)EJobAttribute.EJob_Hero) {
                            //英雄击杀
                            feedbackType |= 1 << (int)EUIKillFeedbackType.HeroKO;
                            playerEntity.playerInfo.SpecialFeedbackType = (int)EUIKillFeedbackType.HeroKO;
                        }
                        //武器
                        WeaponResConfigItem newConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(damage.weaponId);
                        if (null != newConfig)
                        {
                            if (newConfig.SubType == (int)EWeaponSubType.Melee)
                            {
                                feedbackType |= 1 << (int)EUIKillFeedbackType.MeleeWeapon;
                                playerEntity.playerInfo.SpecialFeedbackType = (int)EUIKillFeedbackType.MeleeWeapon;
                            }
                            else if (newConfig.SubType == (int)EWeaponSubType.BurnBomb)
                                feedbackType |= 1 << (int)EUIKillFeedbackType.Burning;
                            else if (newConfig.SubType == (int)EWeaponSubType.Grenade)
                                feedbackType |= 1 << (int)EUIKillFeedbackType.Grenade;
                        }
                        if (feedbackType == 0)
                        {
                            //普通击杀
                            feedbackType = 1 << (int) EUIKillFeedbackType.Normal;
                        }
                        damage.KillFeedbackType = feedbackType;
                    }
                }

                //数据统计
                ProcessDamageStatistics(contexts, gameRule, srcPlayer, playerEntity, damage);
            }

            //击杀|击倒
            if (null != gameRule && playerEntity.gamePlay.IsDead())
            {
                gameRule.KillPlayer(srcPlayer, playerEntity, damage);
            }

            _logger.DebugFormat("change player hp entityId:{0}, health {1}->{2}, state {3}, srcPlayerId:{4}, playerId:{5}, hurtType:{6}, weaponId:{7}", playerEntity.entityKey.Value.EntityId, curHp, playerEntity.gamePlay.CurHp, playerEntity.gamePlay.LifeState, (srcPlayer != null) ? srcPlayer.playerInfo.PlayerId : 0, playerEntity.playerInfo.PlayerId, damage.type, damage.weaponId);

            return teamList;
        }

        private static int GetTeamCapacity(Contexts contexts)
        {
            return contexts.session.commonSession.RoomInfo.TeamCapacity;
        }

        private static List<PlayerEntity> CheckUpdatePlayerStatus(PlayerEntity player, PlayerDamageInfo damage, Contexts contexts)
        {
            GamePlayComponent gamePlay = player.gamePlay;
            if (damage.InstantDeath)
            {
                gamePlay.ChangeLifeState(EPlayerLifeState.Dead, player.time.ClientTime);
                return null;
            }
            if (gamePlay.IsLifeState(EPlayerLifeState.Alive))
            {
                if (gamePlay.CurHp <= 0)
                {
                    //存活队友
                    int aliveCount = 0;
                    //存活队伍
                    HashSet<long> aliveTeams = new HashSet<long>();
                    if (null != contexts)
                    {
                        foreach (PlayerEntity other in contexts.player.GetEntities())
                        {
                            if (other.hasPlayerInfo && other != player && other.gamePlay.IsLifeState(EPlayerLifeState.Alive))
                            {
                                aliveTeams.Add(other.playerInfo.TeamId);
                                if (other.playerInfo.TeamId == player.playerInfo.TeamId)
                                    aliveCount++;
                            }
                        }
                    }
                    if (damage.type == (int) EUIDeadType.Drown /*|| damage.type == (int) EUIDeadType.VehicleBomb*/
                        || player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Swim
                        || player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Dive)
                    {
                        //直接死亡
                        gamePlay.ChangeLifeState(EPlayerLifeState.Dead, player.time.ClientTime);
                    }
                    else if (aliveCount == 0)
                    {
                        //自己
                        gamePlay.ChangeLifeState(EPlayerLifeState.Dead, player.time.ClientTime);
                        //全队阵亡
                        if (null != contexts)
                        {
                            List<PlayerEntity> teamList = new List<PlayerEntity>();
                            foreach (PlayerEntity other in contexts.player.GetEntities())
                            {
                                if (other.hasPlayerInfo && other != player &&
                                    other.playerInfo.TeamId == player.playerInfo.TeamId
                                    && other.gamePlay.IsLifeState(EPlayerLifeState.Dying))
                                {
                                    teamList.Add(other);
                                }
                            }
                            return teamList;
                        }
                    }
                    else if (GetTeamCapacity(contexts) > 1 && aliveTeams.Count > 1)
                    {
                        //受伤状态
                        gamePlay.ChangeLifeState(EPlayerLifeState.Dying, player.time.ClientTime);
                    }
                    else
                    {
                        gamePlay.ChangeLifeState(EPlayerLifeState.Dead, player.time.ClientTime);
                    }
                }
            }
            else if (gamePlay.IsLifeState(EPlayerLifeState.Dying))
            {
                if (gamePlay.InHurtedHp <= 0)
                {
                    gamePlay.ChangeLifeState(EPlayerLifeState.Dead, player.time.ClientTime);
                }
                else
                {
                    int aliveCount = 0;
                    if (null != contexts)
                    {
                        foreach (PlayerEntity other in contexts.player.GetEntities())
                        {
                            if (other.hasPlayerInfo && other != player && other.gamePlay.IsLifeState(EPlayerLifeState.Alive) && other.playerInfo.TeamId == player.playerInfo.TeamId)
                            {
                                aliveCount++;
                            }
                        }
                    }
                    if (aliveCount == 0)
                    {
                        gamePlay.ChangeLifeState(EPlayerLifeState.Dead, player.time.ClientTime);
                    }
                }
            }
            return null;
        }

        public static void ProcessDamageStatistics(Contexts contexts, IGameRule gameRule, PlayerEntity srcPlayer, PlayerEntity targetPlayer, PlayerDamageInfo damage)
        {
            if (null == targetPlayer)
                return;

            bool isTargetDead = targetPlayer.gamePlay.IsDead();

            if (isTargetDead)
            {
                targetPlayer.isFlagCompensation = false;
            }

            //攻击者
            if (null != srcPlayer)
            {
                bool isTeammate = srcPlayer.playerInfo.TeamId == targetPlayer.playerInfo.TeamId;
                
                bool isKill = isTargetDead;
                bool isHitDown = targetPlayer.gamePlay.IsHitDown();
                bool isCrit = damage.part == (int) EBodyPart.Head;

                targetPlayer.statisticsData.AddOtherInfo(srcPlayer.entityKey.Value, srcPlayer.WeaponController().HeldConfigId, isKill, isHitDown, damage.damage, srcPlayer.playerInfo, System.DateTime.Now.Ticks / 10000);

                //添加别人对自己的伤害记录（受伤不算）
                if (targetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Alive) && !isTeammate)
                {
                    //添加自己对别人的伤害记录
                    srcPlayer.statisticsData.AddOpponentInfo(targetPlayer.entityKey.Value, srcPlayer.WeaponController().HeldConfigId, isKill, isHitDown, damage.damage, targetPlayer.playerInfo, targetPlayer.statisticsData.Statistics.DeadCount);
                    //总伤害量
                    srcPlayer.statisticsData.Statistics.TotalDamage += damage.damage;
                    //有效伤害
                    srcPlayer.statisticsData.Statistics.PlayerDamage += damage.damage;
                }

                if (isTargetDead)
                {
                    if (!isTeammate)
                    {
                        //击杀数
                        srcPlayer.statisticsData.Statistics.KillCount++;
                        srcPlayer.statisticsData.Statistics.LastKillTime = (int) DateTime.Now.Ticks / 10000;
                        //爆头击杀（不包括近战）
                        if (isCrit && !damage.IsKnife)
                        {
                            srcPlayer.statisticsData.Statistics.CritKillCount++;
                        }
                        //连杀数
                        srcPlayer.statisticsData.Statistics.EvenKillCount++;
                        if (srcPlayer.statisticsData.Statistics.EvenKillCount > srcPlayer.statisticsData.Statistics.MaxEvenKillCount)
                        {
                            srcPlayer.statisticsData.Statistics.MaxEvenKillCount = srcPlayer.statisticsData.Statistics.EvenKillCount;
                        }
                        //最大击杀距离
                        srcPlayer.statisticsData.Statistics.MaxKillDistance = Mathf.Max(srcPlayer.statisticsData.Statistics.MaxKillDistance,
                            UnityEngine.Vector3.Distance(srcPlayer.position.Value, targetPlayer.position.Value));
                        if (damage.WeaponType == EWeaponSubType.Pistol)
                        {
                            //手枪击杀
                            srcPlayer.statisticsData.Statistics.PistolKillCount++;
                        }
                        else if (damage.WeaponType == EWeaponSubType.Grenade)
                        {
                            //手雷击杀
                            srcPlayer.statisticsData.Statistics.GrenadeKillCount++;
                        }
                    }
                    else
                    {
                        //击杀队友
                        srcPlayer.statisticsData.Statistics.KillTeamCount++;
                    }
                }

                //击倒数
                if (isHitDown && !isTeammate)
                {
                    srcPlayer.statisticsData.Statistics.HitDownCount++;
                }

                //总爆头数（不包括近战）
                if (isCrit && !damage.IsKnife && !isTeammate)
                {
                    srcPlayer.statisticsData.Statistics.CritCount++;
                }
            }

            //受击者
            //承受伤害量
            targetPlayer.statisticsData.Statistics.TotalBeDamage += damage.damage;
            if (isTargetDead)
            {
                //死亡次数
                targetPlayer.statisticsData.Statistics.DeadCount++;
                targetPlayer.statisticsData.Statistics.LastDeadTime = (int) (DateTime.Now.Ticks / 10000L);
                if (null == srcPlayer)
                {
                    targetPlayer.statisticsData.SetDeadType(damage.type);
                }
                else
                {
                    targetPlayer.statisticsData.AddKillerInfo(srcPlayer.entityKey.Value, srcPlayer.WeaponController().HeldConfigId, damage.type, srcPlayer.playerInfo);
                }
                if (SharedConfig.IsServer && null != gameRule)
                {
                    //死亡顺序（非自杀与队友击杀）
                    if (null != srcPlayer && srcPlayer.playerInfo.TeamId != targetPlayer.playerInfo.TeamId)
                    {
                        gameRule.Contexts.session.serverSessionObjects.DeathOrder++;
                        targetPlayer.statisticsData.Statistics.DeathOrder = gameRule.Contexts.session.serverSessionObjects.DeathOrder;
                    }
                    else
                    {
                        targetPlayer.statisticsData.Statistics.DeathOrder = -1;
                    }

                    //存活时间
                    IEventArgs args = (IEventArgs)gameRule.Contexts.session.commonSession.FreeArgs;
                    int startTime = FreeUtil.ReplaceInt("{startWaitTime}", args);
                    int curTime = FreeUtil.ReplaceInt("{serverTime}", args);
                    targetPlayer.statisticsData.Statistics.AliveTime = (int)((curTime - startTime) * 0.001f);
                    if (null != srcPlayer)
                    {
                        //一血
                        if (targetPlayer.statisticsData.Statistics.DeathOrder == 1)
                        {
                            srcPlayer.statisticsData.Statistics.GetFirstBlood = true;
                        }
                    }
                    //助攻数
                    foreach (var other in targetPlayer.statisticsData.Battle.OtherDict.Values)
                    {
                        PlayerEntity otherEntity = gameRule.Contexts.player.GetEntityWithEntityKey(other.PlayerKey);
                        if (null != otherEntity)
                        {
                            if (otherEntity.playerInfo.TeamId == targetPlayer.playerInfo.TeamId)
                                continue;

                            bool assistFlag = false;
                            if (null == srcPlayer)
                            {
                                if (DateTime.Now.Ticks / 10000 - other.timestamp <= 10000)
                                    assistFlag = true;
                            }
                            else
                            {
                                if (other.PlayerKey == srcPlayer.entityKey.Value)
                                    continue;

                                if (srcPlayer.playerInfo.TeamId == otherEntity.playerInfo.TeamId)
                                {
                                    if (DateTime.Now.Ticks / 10000 - other.timestamp <= 10000)
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
                                message.Ss.Add(FreeUtil.ReplaceVar(otherEntity.playerInfo.PlayerName, args));
                                message.Ins.Add(0);
                                message.Ins.Add(0);
                                message.Ins.Add(feedbackType);
                                FreeMessageSender.SendMessage(otherEntity, message);
                            }
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
    }
}