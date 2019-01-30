using System;
using System.Collections.Generic;
using Core;
using Core.Compare;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Core.WeaponLogic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class WeaponComponentAgent : IComponent
    {
        public IPlayerWeaponComponentGetter Content;

    }


    [Player]
    public class WeaponFactoryComponent : IComponent
    {
        public IWeaponFactory Factory;
    }

    [Player]
    public class WeaponLogicComponent : IComponent
    {
        public IPlayerWeaponState State;
        [DontInitilize] public IWeaponLogic Weapon;
        [DontInitilize] public IWeaponSoundLogic WeaponSound;
        [DontInitilize] public IWeaponEffectLogic WeaponEffect;
    }

    [Player]
    public class WeaponAutoStateComponent : IComponent
    {
        [DontInitilize] public bool AutoThrowing;

        public void Reset()
        {
            AutoThrowing = false; 
        }
    }

    [Player]
    
    public class WeaponLogicInfoComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int WeaponId;
        [DontInitilize] public int LastWeaponId;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerWeaponLogicInfo;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightInfo = right as WeaponLogicInfoComponent;
            return rightInfo.WeaponId == WeaponId;
        }

        public void CopyFrom(object rightComponent)
        {
            var rightInfo = rightComponent as WeaponLogicInfoComponent;
            WeaponId = rightInfo.WeaponId;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class PlayerInterruptStateComponent : IComponent
    {
        [Flags]
        public enum InterruptReason
        {
            BagUI = 1,
        }
        [DontInitilize] public int ForceInterruptGunSight; 
    }

    [Player]
    [Serializable]
    
    public class PlayerWeaponStateComponent : IUserPredictionComponent
    {
        ////////////////////////////////////////////////////////////
        /// IKickbackLogic, IKickbackDecayLogic 后坐力
        ////////////////////////////////////////////////////////////
        [NetworkProperty] public int PunchDecayCdTime; // 开火后坐力效果时间, 在这个时间内，不回落

        [NetworkProperty] public bool PunchYawLeftSide; // PunchYaw随机的方向


        ////////////////////////////////////////////////////////////
        /// IAccuracyLogic
        ////////////////////////////////////////////////////////////
        [NetworkProperty] public float Accuracy;


        ////////////////////////////////////////////////////////////
        /// IFireReady
        ////////////////////////////////////////////////////////////
        [NetworkProperty] public int NextAttackTimer;

        [NetworkProperty] public int LastFireTime;
        [DontInitilize, NetworkProperty] public bool PullBolting;
        [DontInitilize, NetworkProperty] public bool PullBoltFinish;
        [DontInitilize, NetworkProperty] public bool GunSightBeforePullBolting;
        [DontInitilize, NetworkProperty] public bool ForceChangeGunSight;


        ////////////////////////////////////////////////////////////
        /// ContinuesShoot
        ////////////////////////////////////////////////////////////
        [NetworkProperty] public int ContinuesShootCount;

        [NetworkProperty] public bool ContinuesShootDecreaseNeeded;
        [NetworkProperty] public int ContinuesShootDecreaseTimer;
        [NetworkProperty] public int BurstShootCount;

        [DontInitilize] public Vector3 LastBulletDir;
        [DontInitilize] public float LastSpreadX;
        [DontInitilize] public float LastSpreadY;

        [DontInitilize, NetworkProperty] public int ContinuousAttackTime;
        [DontInitilize, NetworkProperty] public int NextAttackingTimeLimit;
        [DontInitilize, NetworkProperty] public bool MeleeAttacking;
        [DontInitilize, NetworkProperty] public bool RangeAttacking;
        [DontInitilize] public bool Reloading;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerWeapon;
        }

        public void CopyFrom(object rightComponent)
        {
            PlayerWeaponStateComponent obj = rightComponent as PlayerWeaponStateComponent;
            PunchDecayCdTime = obj.PunchDecayCdTime;
            PunchYawLeftSide = obj.PunchYawLeftSide;
            Accuracy = obj.Accuracy;
            NextAttackTimer = obj.NextAttackTimer;
            LastFireTime = obj.LastFireTime;
            ContinuesShootCount = obj.ContinuesShootCount;
            ContinuesShootDecreaseNeeded = obj.ContinuesShootDecreaseNeeded;
            ContinuesShootDecreaseTimer = obj.ContinuesShootDecreaseTimer;
            ContinuousAttackTime = obj.ContinuousAttackTime;
            NextAttackingTimeLimit = obj.NextAttackingTimeLimit;
            MeleeAttacking = obj.MeleeAttacking;
            PullBolting = obj.PullBolting;
            PullBoltFinish = obj.PullBoltFinish;
            ForceChangeGunSight = obj.ForceChangeGunSight;
            GunSightBeforePullBolting = obj.GunSightBeforePullBolting;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            PlayerWeaponStateComponent obj = right as PlayerWeaponStateComponent;
            bool rc = true;
            rc = rc && CompareUtility.IsApproximatelyEqual(this.PunchDecayCdTime, obj.PunchDecayCdTime);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.PunchYawLeftSide, obj.PunchYawLeftSide);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.Accuracy, obj.Accuracy);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.NextAttackTimer, obj.NextAttackTimer);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.LastFireTime, obj.LastFireTime);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.ContinuesShootCount, obj.ContinuesShootCount);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.ContinuesShootDecreaseNeeded,
                     obj.ContinuesShootDecreaseNeeded);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.ContinuesShootDecreaseTimer,
                     obj.ContinuesShootDecreaseTimer);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.ContinuousAttackTime, obj.ContinuousAttackTime);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.NextAttackingTimeLimit, obj.NextAttackingTimeLimit);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.MeleeAttacking, obj.MeleeAttacking);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.PullBolting, obj.PullBolting);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.PullBoltFinish, obj.PullBoltFinish);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.GunSightBeforePullBolting,
                     obj.GunSightBeforePullBolting);
            rc = rc && CompareUtility.IsApproximatelyEqual(this.ForceChangeGunSight, obj.ForceChangeGunSight);
            return rc;
        }

        public override string ToString()
        {
            return string.Format(
                "PunchDecayCdTime: {0}, PunchYawLeftSide: {1}, Accuracy: {2}, NextAttackTimer: {3}, LastFireTime: {4}, PullBolting: {5}, PullBoltFinish: {6}, GunSightBeforePullBolting: {7}, ForceChangeGunSight: {8}, ContinuesShootCount: {9}, ContinuesShootDecreaseNeeded: {10}, ContinuesShootDecreaseTimer: {11}, ContinuousAttackTime: {12}, NextAttackingTimeLimit: {13}, MeleeAttacking: {14}",
                PunchDecayCdTime, PunchYawLeftSide, Accuracy, NextAttackTimer, LastFireTime, PullBolting,
                PullBoltFinish, GunSightBeforePullBolting, ForceChangeGunSight, ContinuesShootCount,
                ContinuesShootDecreaseNeeded, ContinuesShootDecreaseTimer, ContinuousAttackTime, NextAttackingTimeLimit,
                MeleeAttacking);
        }
    }
}