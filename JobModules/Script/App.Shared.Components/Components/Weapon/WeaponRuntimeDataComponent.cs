using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;
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
        [DontInitilize, NetworkProperty] public int ContinuesShootCount;
        /// <summary>
        /// 连射模式标志位
        /// </summary>
        [DontInitilize, NetworkProperty] public bool NeedAutoBurstShoot;
        /// <summary>
        /// 连射模式统计
        /// </summary>
        [DontInitilize, NetworkProperty] public int BurstShootCount;
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
        [DontInitilize, NetworkProperty] public bool IsPullingBolt; //是否在拉栓中
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
            IsPullingBolt         = remote.IsPullingBolt;
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
                && PunchYawSpeed == remote.PunchYawSpeed
                && TargetPunchPitchDelta == remote.TargetPunchPitchDelta
                && Accuracy == remote.Accuracy
                && LastRenderTimestamp == remote.LastRenderTimestamp
                && NextAttackTimestamp == remote.NextAttackTimestamp
                && LastAttackTimestamp == remote.LastAttackTimestamp
                //                && PullBoltEnd == remote.PullBoltEnd
                //                && IsPullingBolt == remote.IsPullingBolt
                //                && IsInterruptSightView == remote.IsInterruptSightView
                && ContinuesShootCount == remote.ContinuesShootCount
                && NeedAutoBurstShoot== remote.NeedAutoBurstShoot

                && NeedAutoReload== remote.NeedAutoReload

                && ContinuesShootReduceTimestamp == remote.ContinuesShootReduceTimestamp

                   //        && IsRecoverSightView == remote.IsRecoverSightView
                && BurstShootCount == remote.BurstShootCount
                && LastBulletDir == remote.LastBulletDir
                && LastSpreadX == remote.LastSpreadX
                && LastSpreadY == remote.LastSpreadY
                && ContinueAttackEndStamp == remote.ContinueAttackEndStamp
                && ContinueAttackStartStamp == remote.ContinueAttackStartStamp
                && NextAttackPeriodStamp == remote.NextAttackPeriodStamp
                && MeleeAttacking == remote.MeleeAttacking
                && IsPrevCmdFire == remote.IsPrevCmdFire
                && IsPullingBolt == remote.IsPullingBolt
                && PullBoltInterrupt == remote.PullBoltInterrupt;
        }
        public override string ToString()
        {
            return string.Format("PunchDecayLeftInterval: {0}, PunchYawLeftSide: {1}, PunchYawSpeed: {2}, TargetPunchPitchDelta: {3}, Accuracy: {4}, LastRenderTime: {5}, NextAttackTimestamp: {6}, LastAttackTimestamp: {7}, ContinuesShootCount: {8}, ContinuesShootReduceTimestamp: {9},BurstShootCount: {10}, LastBulletDir: {11}, LastSpreadX: {12}, LastSpreadY: {13}",
            PunchDecayLeftInterval, PunchYawLeftSide, PunchYawSpeed, TargetPunchPitchDelta, Accuracy, LastRenderTimestamp, NextAttackTimestamp, LastAttackTimestamp, ContinuesShootCount, ContinuesShootReduceTimestamp, BurstShootCount, LastBulletDir, LastSpreadX, LastSpreadY);
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