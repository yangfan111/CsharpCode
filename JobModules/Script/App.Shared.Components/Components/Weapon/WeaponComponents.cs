using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;



using Core;
using Entitas;
using Core.EntityComponent;
using Utils.Singleton;
using Assets.Utils.Configuration;
using App.Shared.Components.SceneObject;

namespace App.Shared.Components.Weapon
{

    [Weapon, UniquePrefix("is")]
    public class ActiveComponent : IUserPredictionComponent
    {
        public void CopyFrom(object rightComponent)
        {
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponActiveFlag;
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
    public class WeaponScanComponent : IComponent
    {
        [DontInitilize]
        public WeaponScanStruct Value;
        public void SyncSelf(EntityKey key,WeaponBasicDataComponent basic)
        {
            Value.Assign(key, basic.ConfigId);
            Value.AvatarId       = basic.WeaponAvatarId;
            Value.UpperRail      = basic.UpperRail;
            Value.LowerRail      = basic.LowerRail;
            Value.Magazine       = basic.Magazine;
            Value.Muzzle         = basic.Muzzle;
            Value.Bullet         = basic.Bullet;
            Value.Stock          = basic.Stock;
            Value.ReservedBullet = basic.ReservedBullet;
        }



    }
    [Weapon]
    public class WeaponBasicDataComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int ConfigId;
        [DontInitilize, NetworkProperty] public int WeaponAvatarId;
        [DontInitilize, NetworkProperty] public int UpperRail;
        [DontInitilize, NetworkProperty] public int LowerRail;
        [DontInitilize, NetworkProperty] public int Stock;
        [DontInitilize, NetworkProperty] public int Muzzle;
        [DontInitilize, NetworkProperty] public int Magazine;
        [DontInitilize, NetworkProperty] public int Bullet;
        [DontInitilize, NetworkProperty] public bool PullBolt;
        [DontInitilize, NetworkProperty] public int FireModel;
        /// <summary>
        /// 除了在备弹管理类中，不要直接使用
        /// </summary>
        [DontInitilize, NetworkProperty] public int ReservedBullet;
        public int RealFireModel
        {
            get
            {
                if (FireModel == 0 && ConfigId > 0)
                {
                    var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);
                    FireModel = (int)weaponAllConfig.GetDefaultFireModel();
                }
                return FireModel;
            }
           
       
    }
        public static explicit operator WeaponObjectComponent(WeaponBasicDataComponent remote)
        {
            var newComp = new WeaponObjectComponent();
            newComp.ConfigId = remote.ConfigId;
            newComp.WeaponAvatarId = remote.WeaponAvatarId;
            newComp.UpperRail = remote.UpperRail;
            newComp.LowerRail = remote.LowerRail;
            newComp.Stock = remote.Stock;
            newComp.Muzzle = remote.Muzzle;
            newComp.Magazine = remote.Magazine;
            newComp.Bullet = remote.Bullet;
            //newComp.ReservedBullet = remote.ReservedBullet;
            //newComp.PullBolt = remote.PullBolt;
            //newComp.FireModel = remote.FireModel;
            return newComp;
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponBasicDataComponent;
            ConfigId = remote.ConfigId;
            WeaponAvatarId = remote.WeaponAvatarId;
            UpperRail = remote.UpperRail;
            LowerRail = remote.LowerRail;
            Stock = remote.Stock;
            Muzzle = remote.Muzzle;
            Magazine = remote.Magazine;
            Bullet = remote.Bullet;
            ReservedBullet = remote.ReservedBullet;
            PullBolt = remote.PullBolt;
            FireModel = remote.FireModel;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponBasicInfo;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponBasicDataComponent;
            return ConfigId == remote.ConfigId &&
            WeaponAvatarId == remote.WeaponAvatarId &&
            UpperRail == remote.UpperRail &&
            LowerRail == remote.LowerRail &&
            Stock == remote.Stock &&
            Muzzle == remote.Muzzle &&
            Magazine == remote.Magazine &&
            Bullet == remote.Bullet &&
            ReservedBullet == remote.ReservedBullet &&
            PullBolt == remote.PullBolt &&
            FireModel == remote.FireModel;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
        public void Reset()
        {
            CopyFrom(Empty);
        }
        public static readonly WeaponBasicDataComponent Empty = new WeaponBasicDataComponent();
    }


    [Weapon]
    public class WeaponRuntimeDataComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int PunchDecayCdTime; // 开火后坐力效果时间, 在这个时间内，不回落
        [DontInitilize, NetworkProperty] public bool PunchYawLeftSide; // PunchYaw随机的方向
        [DontInitilize, NetworkProperty] public float PunchYawSpeed;
        [DontInitilize, NetworkProperty] public float PunchPitchSpeed;
        [DontInitilize, NetworkProperty] public int LastRenderTime;
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
        [DontInitilize, NetworkProperty] public int ContinueAttackEndStamp;
        [DontInitilize, NetworkProperty] public int ContinueAttackStartStamp;
        [DontInitilize, NetworkProperty] public int NextAttackPeriodStamp;
        [DontInitilize, NetworkProperty] public bool MeleeAttacking;
        [DontInitilize, NetworkProperty] public bool IsPrevCmdFire;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponRuntimeDataComponent;
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
            ContinueAttackEndStamp = remote.ContinueAttackEndStamp;
            NextAttackPeriodStamp = remote.NextAttackPeriodStamp;
            MeleeAttacking = remote.MeleeAttacking;
            IsPrevCmdFire = remote.IsPrevCmdFire;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponData;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponRuntimeDataComponent;
            return PunchDecayCdTime == remote.PunchDecayCdTime
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
                && ContinueAttackEndStamp == remote.ContinueAttackEndStamp
                && NextAttackPeriodStamp == remote.NextAttackPeriodStamp
                && MeleeAttacking == remote.MeleeAttacking
                && IsPrevCmdFire == remote.IsPrevCmdFire;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public static readonly WeaponRuntimeDataComponent Empty = new WeaponRuntimeDataComponent();

        public void Reset()
        {
            CopyFrom(Empty);
        }
    }
}
