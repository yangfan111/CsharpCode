using Core.EntityComponent;
using System.Collections.Generic;
using UnityEngine;
using WeaponConfigNs;

namespace Core.BulletSimulation
{

    public interface IMovable
    {
        Vector3 Origin { get; set; }
        bool IsValid { get; set; }
    }

    public interface IBulletEntity : IMovable
    {
        EntityKey OwnerEntityKey { get; }
        List<int> ExcludePlayerList { get; }
        Vector3 Velocity { get; set; }
        float Gravity { get; }
        float VelocityDecay { get; }
        int NextFrameTime { get; set; }
        int ServerTime { get; set; }
        float Distance { get; set; } 
        float DistanceDecayFactor { get; } // 距离衰减系数
        float BaseDamage { get; set; } // 武器基础伤害 
        float PenetrableThickness { get; set; } // 穿透系数
        float GetDamageFactor(EBodyPart part); // hitbox系数 
        int PenetrableLayerCount { get; set; }
        void AddPenetrateInfo(EEnvironmentType type);
        EBulletCaliber Caliber { get; }
        int WeaponId { get; }
        bool IsOverWall { get; }
        Vector3 GunEmitPosition { get; }
        bool IsNew { get; set; }
        Vector3 HitPoint { get; set; }
        EHitType HitType { get; set; }
    }
}