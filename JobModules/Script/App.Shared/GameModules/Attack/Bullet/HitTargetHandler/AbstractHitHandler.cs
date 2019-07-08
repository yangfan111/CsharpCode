using System;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.Attack;
using Core.Compensation;
using Core.HitBox;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;

namespace App.Shared.GameModules.Attack
{
    public abstract class AbstractHitHandler
    {
        protected Contexts contexts;
        protected int cmdSeq;
        protected CustomProfileInfo profilerInfo;

        protected abstract bool Filter(HitBoxOwnerComponent boxOwnerComponent, int cmdSeq);

        protected abstract void Handle(CompensationWorld compensationWorld, IBulletEntityAgent bulletEntityAgent,
                                       RaycastHit hit, PlayerEntity srcPlayer);

        public bool ProcessHitTarget(HitBoxOwnerComponent boxOwnerComponent,int cmdSeq,CompensationWorld compensationWorld, 
                                     IBulletEntityAgent bulletEntityAgent,RaycastHit hit,PlayerEntity srcPlayer)
        {
            if (Filter(boxOwnerComponent,cmdSeq))
            {
                try
                {
                    profilerInfo.BeginProfileOnlyEnableProfile();
                    Handle(compensationWorld,bulletEntityAgent,hit,srcPlayer);
                    
                }
                finally
                {
                    profilerInfo.EndProfileOnlyEnableProfile();
                }

                
                return true;
            }
            return false;
        }

        public AbstractHitHandler(Contexts contexts)
        {
            this.contexts = contexts;
        }
        protected float GetBulletDamage(IBulletEntityAgent bulletEntityAgent, float hitboxFactor, float distance)
        {
            float baseHarm      = bulletEntityAgent.BaseDamage;
            float distanceDecay = bulletEntityAgent.DistanceDecayFactor;
            // 武器基础伤害 * (距离系数 ^ (实际命中距离 / 1270)) * hitbox系数 * 防弹装备系数 * 穿透系数
            float totalDamage = baseHarm * Mathf.Pow(distanceDecay, distance / 12.7f) * hitboxFactor;

            BulletHitHandler._logger.InfoFormat(
                "bullet damage baseHarm {0}, distance decay {1}, distance {2}, hitbox factor {3}, result damage {4}",
                baseHarm, distanceDecay, distance, hitboxFactor, totalDamage);

            return totalDamage;
        }
    }

 
}