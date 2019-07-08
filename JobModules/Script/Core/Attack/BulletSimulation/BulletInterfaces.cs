using System.Collections.Generic;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.Network;
using UnityEngine;
using WeaponConfigNs;

namespace Core.Attack
{
    public enum EHitType
    {
        Environment,
        Player,
        Vehicle,
    }


    public interface IBulletEntitiesCollector
    {
        List<IBulletEntityAgent> GetAllPlayerBulletAgents();
        EntityKey BulletOwner { set; }
    }
    public interface IBulletHitHandler
    {
        IBulletHitHandler SetHitLayerMask(int hitLayerMask);
        void              OnHit(int cmdSeq, IBulletEntityAgent bulletEntityAgent, RaycastHit hit, CompensationWorld compensationWorld);
    }
    public interface IMovable
    {
        Vector3 Position { get; set; }
        bool    IsValid  { get; set; }
    }

    public interface IBulletEntityAgent : IMovable
    {
        EntityKey         OwnerEntityKey      { get; }
        List<int>         ExcludePlayerList   { get; }
        Vector3           Velocity            { get; set; }
        float             Gravity             { get; }
        float             VelocityDecay       { get; }
        int               NextFrameTime       { get; set; }
        int               ServerTime          { get; set; }
        float             Distance            { get; set; } 
        float             DistanceDecayFactor { get; set; } // 距离衰减系数
        float             BaseDamage          { get; set; } // 武器基础伤害 
        float             PenetrableThickness { get; set; } // 穿透系数
        float             GetDamageFactor(EBodyPart part);  // hitbox系数 
        int               PenetrableLayerCount { get; set; }
        void              AddPenetrateInfo(EEnvironmentType type);
        EBulletCaliber    Caliber         { get; }
        int               WeaponId        { get; }
        bool              IsOverWall      { get; }
        Vector3           GunEmitPosition { get; }
        bool              IsNew           { get; set; }
        PrecisionsVector3 HitPoint        { get; set; }
        EHitType          HitType         { get; set; }
        void SetAnimationAndColliderText(string aniText, string colliderText);
        void SetIsNewInspect(bool isNew);
        void SetIsBlockInspect(bool isblock);
        BulletStatisticsInfo StatisticsInfo { get; }

    }
}