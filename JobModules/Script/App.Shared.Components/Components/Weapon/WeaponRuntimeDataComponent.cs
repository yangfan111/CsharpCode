using Core.Compare;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;
using System;
using UnityEngine;

namespace App.Shared.Components.Weapon
{
    [Weapon]
    public class WeaponRuntimeDataComponent : IUserPredictionComponent
    {
        //枪械震动相关

      

// 开火后坐力衰减时间,, 在这个时间内，不回落，初始值=FireShakeProvider.GetDecayInterval(controller);
        [DontInitilize, NetworkProperty] public int  PunchDecayLeftInterval;
        [DontInitilize, NetworkProperty] public bool PunchYawLeftSide; // PunchYaw随机的方向
        [DontInitilize, NetworkProperty] public float PunchYawSpeed;
        //开火后坐力最终到达的强度增量
        [DontInitilize, NetworkProperty] public float TargetPunchPitchDelta;
        /// <summary>
        /// 上一次渲染时间戳
        /// </summary>
        [DontInitilize, NetworkProperty] public int   LastRenderTimestamp;
        /// <summary>
        /// 相机抖动速度
        /// </summary>
        [DontInitilize, NetworkProperty] public float CameraRotationSpeed;

        [DontInitilize, NetworkProperty] public int CameraRotationInterval;
        /// <summary>
        /// 精准度
        /// </summary>
        [DontInitilize, NetworkProperty] public float Accuracy;
        /// <summary>
        /// 上一次攻击时间
        /// </summary>
        [DontInitilize, NetworkProperty] public int LastAttackTimestamp;
        /// <summary>
        /// 下一次攻击时间
        /// </summary>
        [DontInitilize, NetworkProperty] public int NextAttackTimestamp;

        /// <summary>
        /// 连发数量统计
        /// </summary>
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveShort)] public int ContinuesShootCount;
        /// <summary>
        /// 连射模式标志位
        /// </summary>
        [DontInitilize, NetworkProperty] public bool NeedAutoBurstShoot;
        /// <summary>
        /// 连射模式统计
        /// </summary>
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveShort)] public int BurstShootCount;
        /// <summary>
        /// 是否触发自动换弹
        /// </summary>
        [DontInitilize, NetworkProperty] public bool NeedAutoReload;
        //子弹连射衰减时间戳
        [DontInitilize, NetworkProperty] public int ContinuesShootReduceTimestamp;


        [DontInitilize, NetworkProperty] public Vector3 LastBulletDir;

        /// <summary>
        /// 准星扩散
        /// </summary>
        [DontInitilize, NetworkProperty] public float LastSpreadX;
        /// <summary>
        /// 准星扩散
        /// </summary>
        [DontInitilize, NetworkProperty] public float LastSpreadY;
        [DontInitilize, NetworkProperty] public int   ContinueAttackEndStamp;
        [DontInitilize, NetworkProperty] public int   ContinueAttackStartStamp;
        [DontInitilize, NetworkProperty] public int   NextAttackPeriodStamp;
        [DontInitilize, NetworkProperty] public bool  MeleeAttacking;
        /// <summary>
        /// 是否提前按住攻击键
        /// </summary>
        [DontInitilize, NetworkProperty] public bool  IsPrevCmdFire;

        ///拉栓状态相关
        [DontInitilize, NetworkProperty] public bool PullBoltInterrupt;//是否在拉栓打断状态
        public                                  bool PullBoltFinish = true;//是否拉栓完成

        
        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponRuntimeDataComponent;
            PunchDecayLeftInterval = remote.PunchDecayLeftInterval;
            PunchYawLeftSide       = remote.PunchYawLeftSide;
            PunchYawSpeed          = remote.PunchYawSpeed;
            TargetPunchPitchDelta  = remote.TargetPunchPitchDelta;

            Accuracy            = remote.Accuracy;
            LastRenderTimestamp      = remote.LastRenderTimestamp;
            NextAttackTimestamp = remote.NextAttackTimestamp;
            LastAttackTimestamp = remote.LastAttackTimestamp;

            CameraRotationInterval = remote.CameraRotationInterval;
            //            PullBoltEnd = remote.PullBoltEnd;
            //            IsPullingBolt = remote.IsPullingBolt;
            //            IsRecoverSightView = remote.IsRecoverSightView;
            //            IsInterruptSightView = remote.IsInterruptSightView;
            ContinuesShootCount           = remote.ContinuesShootCount;
            NeedAutoBurstShoot = remote.NeedAutoBurstShoot;

            NeedAutoReload = remote.NeedAutoReload;

            ContinuesShootReduceTimestamp = remote.ContinuesShootReduceTimestamp;
            BurstShootCount               = remote.BurstShootCount;
            LastBulletDir                 = remote.LastBulletDir;
            LastSpreadX                   = remote.LastSpreadX;
            LastSpreadY                   = remote.LastSpreadY;
            ContinueAttackEndStamp        = remote.ContinueAttackEndStamp;
            ContinueAttackStartStamp      = remote.ContinueAttackStartStamp;

            NextAttackPeriodStamp = remote.NextAttackPeriodStamp;
            MeleeAttacking        = remote.MeleeAttacking;
            IsPrevCmdFire         = remote.IsPrevCmdFire;
            PullBoltInterrupt     = remote.PullBoltInterrupt;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.WeaponRuntimeData;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponRuntimeDataComponent;
            return PunchDecayLeftInterval == remote.PunchDecayLeftInterval
                && PunchYawLeftSide == remote.PunchYawLeftSide
                && CompareUtility.IsApproximatelyEqual(PunchYawSpeed ,remote.PunchYawSpeed,3) 
                && CompareUtility.IsApproximatelyEqual(TargetPunchPitchDelta ,remote.TargetPunchPitchDelta,3) 
                && Math.Abs(Accuracy - remote.Accuracy) < 0.01f
                && LastRenderTimestamp == remote.LastRenderTimestamp
                && NextAttackTimestamp == remote.NextAttackTimestamp
                && LastAttackTimestamp == remote.LastAttackTimestamp
                && ContinuesShootCount == remote.ContinuesShootCount
                && NeedAutoBurstShoot== remote.NeedAutoBurstShoot
                && NeedAutoReload== remote.NeedAutoReload
                && ContinuesShootReduceTimestamp == remote.ContinuesShootReduceTimestamp
                && BurstShootCount == remote.BurstShootCount
                && CameraRotationInterval == remote.CameraRotationInterval
                // && CompareUtility.IsApproximatelyEqual(LastBulletDir ,remote.LastBulletDir) 
                && Math.Abs(LastSpreadX - remote.LastSpreadX) < 0.0001f
                && Math.Abs(LastSpreadY - remote.LastSpreadY) < 0.0001f
                && ContinueAttackEndStamp == remote.ContinueAttackEndStamp
                && ContinueAttackStartStamp == remote.ContinueAttackStartStamp
                && NextAttackPeriodStamp == remote.NextAttackPeriodStamp
                && MeleeAttacking == remote.MeleeAttacking
                && IsPrevCmdFire == remote.IsPrevCmdFire
                && PullBoltInterrupt == remote.PullBoltInterrupt;
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}",
                PunchDecayLeftInterval, PunchYawLeftSide, PunchYawSpeed, TargetPunchPitchDelta, Accuracy,
                LastRenderTimestamp, NextAttackTimestamp, LastAttackTimestamp, ContinuesShootCount, NeedAutoBurstShoot,
                NeedAutoReload, ContinuesShootReduceTimestamp, BurstShootCount, LastBulletDir, LastSpreadX, LastSpreadY,
                ContinueAttackEndStamp, ContinueAttackStartStamp, NextAttackPeriodStamp, MeleeAttacking, IsPrevCmdFire,
                 PullBoltInterrupt, CameraRotationInterval);
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