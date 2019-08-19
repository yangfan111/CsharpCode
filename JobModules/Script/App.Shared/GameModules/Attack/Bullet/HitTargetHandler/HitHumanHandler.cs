using App.Shared.Components.Player;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Core;
using Core.Attack;
using Core.Compensation;
using Core.Enums;
using Core.HitBox;
using Core.Utils;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Attack
{


    public class HitHumanHandler : AbstractHitHandler
    {
        private StringBuilder stringBuilder = new StringBuilder();

        public string GetCollidersDebugDatas(PlayerEntity entity)
        {
            if (entity.hasPosition && entity.hasHitBox)
            {
                HitBoxTransformProvider provider = SingletonManager.Get<HitBoxTransformProviderCache>().GetProvider(entity.thirdPersonModel.Value);
                if (provider != null)
                {
                    stringBuilder.Length = 0;
                    var colliders = provider.GetHitBoxColliders();
                    foreach (KeyValuePair<string, Collider> keyPair in colliders)
                    {
                        stringBuilder.AppendFormat("<{0}=>trans:{1}>\n", keyPair.Key,
                            keyPair.Value.transform.position);
                    }
                    return stringBuilder.ToString();
                }
            }
            return string.Empty;
        }
        private IPlayerDamager damager; // null for client

        private PlayerEntity hitTargetPlayer;
       
        public HitHumanHandler(IPlayerDamager damager, Contexts contexts) : base(contexts)
        {
            this.damager = damager;
            profilerInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BulletHitHandler_OnHitPlayer");
        }

        protected override bool Filter(HitBoxOwnerComponent boxOwnerComponent, int cmdSeq)
        {
            if (boxOwnerComponent && boxOwnerComponent.OwnerEntityKey.EntityType == (short) EEntityType.Player)
            {
                hitTargetPlayer = contexts.player.GetEntityWithEntityKey(boxOwnerComponent.OwnerEntityKey) ?? null;
                if (hitTargetPlayer == null)
                {
                    BulletHitHandler._logger.ErrorFormat("[Hit{1}]cant get player entity with key {0}",
                        boxOwnerComponent.OwnerEntityKey, cmdSeq);
                    return false;
                }

                this.cmdSeq = cmdSeq;
                return true;
            }

            return false;
        }

        protected override void Handle(CompensationWorld compensationWorld, IBulletEntityAgent bulletEntityAgent,
                                       RaycastHit hit, PlayerEntity srcPlayer)
        {
            if (srcPlayer.gamePlay.IsDead())
            {
                BulletHitHandler._logger.ErrorFormat(
                    "[Hit{1}]cant hit player entity with key {0} becase of srcplayer is dead",
                    hitTargetPlayer.entityKey.Value, cmdSeq);
                return;
            }

            Vector3 pos;
            if (compensationWorld.TryGetEntityPosition(hitTargetPlayer.entityKey.Value, out pos))
            {
                DoHitPlayer(srcPlayer, bulletEntityAgent, hit);
            }
            else
            {
                BulletHitHandler._logger.ErrorFormat("[Hit{1}]cant get player compensation position with key {0}",
                    hitTargetPlayer.entityKey.Value, cmdSeq);
                DoHitPlayer(srcPlayer, bulletEntityAgent, hit);
            }

            bulletEntityAgent.HitType = EHitType.Player;
        }

        private void DoHitPlayer(PlayerEntity srcPlayer,IBulletEntityAgent bulletEntityAgent, RaycastHit hit)
        {
            Collider  collider = hit.collider;
            bulletEntityAgent.SetAnimationAndColliderText(hitTargetPlayer.networkAnimator.ToStringExt(),GetCollidersDebugDatas(hitTargetPlayer));
            EBodyPart part     = BulletPlayerUtil.GetBodyPartByHitBoxName(collider);

            BulletHitHandler._logger.InfoFormat("[Hit{0}]HitPlayer in {1}", cmdSeq, part);
            float hitboxFactor = bulletEntityAgent.GetDamageFactor(part);
            float totalDamage = GetBulletDamage(bulletEntityAgent, hitboxFactor,
                Vector3.Distance(hit.point, bulletEntityAgent.GunEmitPosition));
         
            bulletEntityAgent.IsValid = false;

            //由于动画放在客户端做了,服务器调用的命令会被忽视,需要发送事件到客户端
            //            if (hitTargetPlayer.hasStateInterface && hitTargetPlayer.stateInterface.State.CanBeenHit())
            //            {
            //                hitTargetPlayer.stateInterface.State.BeenHit();
            //            }

            ClientEffectFactory.AddBeenHitEvent(srcPlayer, hitTargetPlayer, 
                AttackUtil.GeneraterUniqueHitId(srcPlayer, cmdSeq),
                contexts.session.currentTimeObject.CurrentTime);
            //添加假红统计
            if(hitTargetPlayer.gamePlay.IsAlive())
                srcPlayer.StatisticsController().AddShootPlayer(cmdSeq, bulletEntityAgent, hit.point,
                    hitTargetPlayer.entityKey.Value, hitTargetPlayer.position.Value, part, totalDamage);
            ClientEffectFactory.AddHitPlayerEffectEvent(srcPlayer,hitTargetPlayer.entityKey.Value, hit.point,(int) EAudioUniqueId.BulletHit, part);

            BulletHitHandler._logger.InfoFormat("[Hit{5}]bullet from {0} hit player {1}, part {2}, hitbox factor {3}, result damage {4}",
                bulletEntityAgent.OwnerEntityKey, hitTargetPlayer.entityKey.Value, collider, hitboxFactor, totalDamage,
                cmdSeq);

            if (!hitTargetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
            {
                //有效命中
                if (hitTargetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
                {
                    srcPlayer.statisticsData.Statistics.ShootingPlayerCount++;
                }

                srcPlayer.statisticsData.Statistics.ShootingSuccCount++;
            }

           
            BulletPlayerUtil.ProcessPlayerHealthDamage(contexts, damager, srcPlayer, hitTargetPlayer,
                new PlayerDamageInfo(totalDamage, (int) EUIDeadType.Weapon, (int) part, bulletEntityAgent.WeaponId,
                    bulletEntityAgent.IsOverWall, false, false, bulletEntityAgent.HitPoint, bulletEntityAgent.Velocity));
            DebugUtil.AppendShootText(cmdSeq,"[HitPlayer]hitPoint:{0},collider:{1},totalDamage:{2},part:{3}",hit.point,hit.collider,totalDamage,part);
        }
  
    }
}