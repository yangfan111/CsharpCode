using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Weapon
{
    [Weapon]
    [Unique]
    public class FlagEmptyHandComponent : IComponent, IUserPredictionComponent
    {
        public void CopyFrom(object rightComponent)
        {
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.BagEmptyHand;
        }

        public bool IsApproximatelyEqual(object right)
        {
            return true;
        }

        public void RewindTo(object rightComponent)
        {
        }
    }

    [Weapon]
    public class WeaponDataComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int WeaponId;
        [DontInitilize, NetworkProperty] public int WeaponAvatarId;
        [DontInitilize, NetworkProperty] public int UpperRail;
        [DontInitilize, NetworkProperty] public int LowerRail;
        [DontInitilize, NetworkProperty] public int Stock;
        [DontInitilize, NetworkProperty] public int Muzzle;
        [DontInitilize, NetworkProperty] public int Magazine;
        [DontInitilize, NetworkProperty] public int Bullet;
        /// <summary>
        /// 除了在备弹管理类中，不要直接使用 TODO 添加限制
        /// </summary>
        [DontInitilize, NetworkProperty] public int ReservedBullet;

        [DontInitilize, NetworkProperty] public bool PullBolt;
        [DontInitilize, NetworkProperty] public int PunchDecayCdTime; // 开火后坐力效果时间, 在这个时间内，不回落
        [DontInitilize, NetworkProperty] public bool PunchYawLeftSide; // PunchYaw随机的方向
        [DontInitilize, NetworkProperty] public float Accuracy;
        [DontInitilize, NetworkProperty] public int NextAttackTimer;
        [DontInitilize, NetworkProperty] public int LastFireTime;
        [DontInitilize, NetworkProperty] public bool PullBolting;
        [DontInitilize, NetworkProperty] public bool PullBoltFinish;
        [DontInitilize, NetworkProperty] public bool GunSightBeforePullBolting;
        [DontInitilize, NetworkProperty] public bool ForceChangeGunSight;
        [DontInitilize, NetworkProperty] public int ContinuesShootCount;

        [DontInitilize, NetworkProperty] public bool ContinuesShootDecreaseNeeded;
        [DontInitilize, NetworkProperty] public int ContinuesShootDecreaseTimer;
        [DontInitilize, NetworkProperty] public int BurstShootCount;

        [DontInitilize, NetworkProperty] public Vector3 LastBulletDir;
        [DontInitilize, NetworkProperty] public float LastSpreadX;
        [DontInitilize, NetworkProperty] public float LastSpreadY;

        [DontInitilize, NetworkProperty] public int ContinuousAttackTime;
        [DontInitilize, NetworkProperty] public int NextAttackingTimeLimit;
        [DontInitilize, NetworkProperty] public bool MeleeAttacking;

        [DontInitilize, NetworkProperty] public bool IsPrevCmdFire;
        [DontInitilize, NetworkProperty] public int FireMode;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponDataComponent;
            WeaponId = remote.WeaponId;
            WeaponAvatarId = remote.WeaponAvatarId;
            UpperRail = remote.UpperRail;
            LowerRail = remote.LowerRail;
            Stock = remote.Stock;
            Muzzle = remote.Muzzle;
            Magazine = remote.Magazine;
            Bullet = remote.Bullet;
            ReservedBullet = remote.ReservedBullet;
            PullBolt = remote.PullBolt;
            PunchDecayCdTime = remote.PunchDecayCdTime;
            PunchYawLeftSide = remote.PunchYawLeftSide;
            Accuracy = remote.Accuracy;
            NextAttackTimer = remote.NextAttackTimer;
            LastFireTime = remote.LastFireTime;
            PullBolting = remote.PullBolting;
            PullBoltFinish = remote.PullBoltFinish;
            GunSightBeforePullBolting = remote.GunSightBeforePullBolting;
            ForceChangeGunSight = remote.ForceChangeGunSight;
            ContinuesShootCount = remote.ContinuesShootCount;
            ContinuesShootDecreaseNeeded = remote.ContinuesShootDecreaseNeeded;
            ContinuesShootDecreaseTimer = remote.ContinuesShootDecreaseTimer;
            BurstShootCount = remote.BurstShootCount;
            LastBulletDir = remote.LastBulletDir;
            LastSpreadX = remote.LastSpreadX;
            LastSpreadY = remote.LastSpreadY;
            ContinuousAttackTime = remote.ContinuousAttackTime;
            NextAttackingTimeLimit = remote.NextAttackingTimeLimit;
            MeleeAttacking = remote.MeleeAttacking;
            IsPrevCmdFire = remote.IsPrevCmdFire;
            FireMode = remote.FireMode;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponData;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponDataComponent;
            return WeaponId == remote.WeaponId
                && WeaponAvatarId == remote.WeaponAvatarId
                && UpperRail == remote.UpperRail
                && LowerRail == remote.LowerRail
                && Stock == remote.Stock
                && Muzzle == remote.Muzzle
                && Magazine == remote.Magazine
                && PullBolt == remote.PullBolt
                && PunchDecayCdTime == remote.PunchDecayCdTime
                && PunchYawLeftSide == remote.PunchYawLeftSide
                && Accuracy == remote.Accuracy
                && NextAttackTimer == remote.NextAttackTimer
                && LastFireTime == remote.LastFireTime
                && PullBolting == remote.PullBolting
                && PullBoltFinish == remote.PullBoltFinish
                && GunSightBeforePullBolting == remote.GunSightBeforePullBolting
                && ForceChangeGunSight == remote.ForceChangeGunSight
                && ContinuesShootCount == remote.ContinuesShootCount
                && ContinuesShootDecreaseNeeded == remote.ContinuesShootDecreaseNeeded
                && ContinuesShootDecreaseTimer == remote.ContinuesShootDecreaseTimer
                && BurstShootCount == remote.BurstShootCount
                && LastBulletDir == remote.LastBulletDir
                && LastSpreadX == remote.LastSpreadX
                && LastSpreadY == remote.LastSpreadY
                && ContinuousAttackTime == remote.ContinuousAttackTime
                && NextAttackingTimeLimit == remote.NextAttackingTimeLimit
                && MeleeAttacking == remote.MeleeAttacking
                && IsPrevCmdFire == remote.IsPrevCmdFire
                && FireMode == remote.FireMode;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
