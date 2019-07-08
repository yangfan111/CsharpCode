using Core.Attack;
using Core.EntityComponent;
using Core.Utils;
using System.Collections.Generic;
using Core;
using Core.Components;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.GameModules.Attack
{
    public class BulletEntityAgent : IBulletEntityAgent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletEntityAgent));
        private        BulletEntity  bulletEntity;
        private        PlayerContext playerContext;

        public BulletEntityAgent(BulletEntity bulletEntity)
        {
            this.bulletEntity = bulletEntity;
        }

        public void SetPlayerContext(PlayerContext playerContext)
        {
            this.playerContext = playerContext;
        }
        

        public EntityKey OwnerEntityKey
        {
            get { return bulletEntity.ownerId.Value; }
        }

        public Vector3 Position
        {
            get { return bulletEntity.position.Value; }
            set { bulletEntity.position.Value = value; }
        }


        

        public void SetAnimationAndColliderText(string aniText,string colliderText)
        {
            bulletEntity.bulletData.StatisticsInfo.aniStr = aniText;
            bulletEntity.bulletData.StatisticsInfo.colliderStr = colliderText;
            bulletEntity.bulletData.StatisticsInfo.bulletRunEndStr = ToDynamicString();
        }

        public BulletStatisticsInfo StatisticsInfo
        {
            get { return bulletEntity.bulletData.StatisticsInfo; }
        }

        public void SetIsNewInspect(bool isNew)
        {
            bulletEntity.bulletData.StatisticsInfo.isNewIns = isNew;
        }

        public void SetIsBlockInspect(bool isblock)
        {
            bulletEntity.bulletData.StatisticsInfo.isBlockIns = isblock;

        }
        public Vector3 Velocity
        {
            get { return bulletEntity.bulletData.Velocity; }
            set { bulletEntity.bulletData.Velocity = value; }
        }

        public float Gravity
        {
            get { return bulletEntity.bulletData.Gravity; }
            set { bulletEntity.bulletData.Gravity = value; }
        }


        public float VelocityDecay
        {
            get { return bulletEntity.bulletData.VelocityDecay; }
            set { bulletEntity.bulletData.VelocityDecay = value; }
        }

        public int NextFrameTime
        {
            get { return bulletEntity.bulletData.RemainFrameTime; }
            set { bulletEntity.bulletData.RemainFrameTime = value; }
        }

        public int ServerTime
        {
            get { return bulletEntity.bulletData.ServerTime; }
            set { bulletEntity.bulletData.ServerTime = value; }
        }

        public bool IsValid
        {
            get { return !bulletEntity.isFlagDestroy; }
            set { bulletEntity.isFlagDestroy = !value; }
        }

        public List<int> ExcludePlayerList
        {
            get
            {
                var ownerEntity = playerContext.GetEntityWithEntityKey(bulletEntity.ownerId.Value);
                return ownerEntity.playerHitMaskController.HitMaskController.BulletExcludeTargetList;
            }
        }

        public float Distance
        {
            get { return bulletEntity.bulletData.Distance; }
            set
            {
                bulletEntity.bulletData.Distance = value;

                if (bulletEntity.bulletData.Distance > bulletEntity.bulletData.MaxDistance)
                {
                    bulletEntity.isFlagDestroy = true;
                }
            }
        }

        public float DistanceDecayFactor
        {
            get { return bulletEntity.bulletData.DistanceDecay; }
            set { bulletEntity.bulletData.DistanceDecay = value; }
        }

        public float BaseDamage
        {
            get { return bulletEntity.bulletData.BaseDamage; }
            set { bulletEntity.bulletData.BaseDamage = value; }
        }

        public float PenetrableThickness
        {
            get { return bulletEntity.bulletData.PenetrableThickness; }
            set { bulletEntity.bulletData.PenetrableThickness = value; }
        }

        public float GetDamageFactor(EBodyPart part)
        {
            var configs = bulletEntity.bulletData.DefaultBulletConfig.BodyDamages;
            for (int i = 0; i < configs.Length; i++)
            {
                if (configs[i].BodyPart == part)
                    return configs[i].Factor;
            }

            return 1;
        }

        public void AddPenetrateInfo(EEnvironmentType type)
        {
            if (bulletEntity.bulletRuntime.Layers == null)
                bulletEntity.bulletRuntime.Layers = new List<EEnvironmentType>();

            bulletEntity.bulletRuntime.Layers.Add(type);
        }

        public int PenetrableLayerCount
        {
            get { return bulletEntity.bulletData.PenetrableLayerCount; }
            set { bulletEntity.bulletData.PenetrableLayerCount = value; }
        }

        public EBulletCaliber Caliber
        {
            get { return bulletEntity.bulletData.Caliber; }
        }

        public int WeaponId
        {
            get { return bulletEntity.bulletData.WeaponId; }
        }

        public Vector3 GunEmitPosition
        {
            get { return bulletEntity.bulletData.EmitPoint; }
        }

        public bool IsNew
        {
            get { return bulletEntity.bulletRuntime.IsNew; }
            set { bulletEntity.bulletRuntime.IsNew = value; }
        }

        public bool IsOverWall
        {
            get
            {
                if (bulletEntity.bulletRuntime.Layers == null)
                {
                    return false;
                }
                var index = bulletEntity.bulletRuntime.Layers.FindIndex(etype => etype == EEnvironmentType.Concrete);
                return index > -1;
            }
        }

        public PrecisionsVector3 HitPoint
        {
            get { return bulletEntity.bulletData.HitPoint; }
            set { bulletEntity.bulletData.HitPoint = value; }
        }
      
        public EHitType HitType
        {
            get { return bulletEntity.bulletData.HitType; }
            set { bulletEntity.bulletData.HitType = value; }
        }

        private string ToDynamicString()
        {
            return string.Format("Position:{0},Velocity:{1},Distance:{2},NextFrameTime:{3}", Position, Velocity, Distance, NextFrameTime);
        }

       
    }
}