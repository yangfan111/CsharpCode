using System;
using Core.Compare;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Weapon
{
    [Weapon]
    public class WeaponRuntimeDataComponent : IUserPredictionComponent
    {
        public static readonly WeaponRuntimeDataComponent Empty = new WeaponRuntimeDataComponent();

        /// <summary>
        ///     精准度
        /// </summary>
        [DontInitilize, NetworkProperty] public float Accuracy;

        /// <summary>
        ///     连射模式统计
        /// </summary>
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveShort)]
        public int BurstShootCount;

        [DontInitilize, NetworkProperty] public int CameraRotationInterval;

        /// <summary>
        ///     相机抖动速度
        /// </summary>
        [DontInitilize, NetworkProperty] public float CameraRotationSpeed;

        [DontInitilize, NetworkProperty] public int ContinueAttackEndStamp;
        [DontInitilize, NetworkProperty] public int ContinueAttackStartStamp;

        /// <summary>
        ///     连发数量统计
        /// </summary>
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveShort)]
        public int ContinuesShootCount;

        //子弹连射衰减时间戳
        [DontInitilize, NetworkProperty] public int ContinuesShootReduceTimestamp;

        /// <summary>
        ///     是否提前按住攻击键
        /// </summary>
        [DontInitilize, NetworkProperty] public bool IsPrevCmdFire;

        /// <summary>
        ///     上一次攻击时间
        /// </summary>
        [DontInitilize, NetworkProperty] public int LastAttackTimestamp;


        [DontInitilize, NetworkProperty] public Vector3 LastBulletDir;

        /// <summary>
        ///     上一次渲染时间戳
        /// </summary>
        [DontInitilize, NetworkProperty] public int LastRenderTimestamp;

        /// <summary>
        ///     准星扩散
        /// </summary>
        [DontInitilize, NetworkProperty] public float LastSpreadX;


        /// <summary>
        ///     准星扩散
        /// </summary>
        [DontInitilize, NetworkProperty] public float LastSpreadY;

        [DontInitilize, NetworkProperty] public bool MeleeAttacking;

        /// <summary>
        ///     连射模式标志位
        /// </summary>
        [DontInitilize, NetworkProperty] public bool NeedAutoBurstShoot;

        /// <summary>
        ///     是否触发自动换弹
        /// </summary>
        [DontInitilize, NetworkProperty] public bool NeedAutoReload;

        [DontInitilize, NetworkProperty] public int NextAttackPeriodStamp;

        /// <summary>
        ///     下一次攻击时间
        /// </summary>
        [DontInitilize, NetworkProperty] public int NextAttackTimestamp;

        public bool PullBoltFinish = true; //是否拉栓完成

        ///拉栓状态相关
        [DontInitilize, NetworkProperty] public bool PullBoltInterrupt; //是否在拉栓打断状态
        //枪械震动相关


        // 开火后坐力衰减时间,, 在这个时间内，不回落，初始值=FireShakeProvider.GetDecayInterval(controller);
        [DontInitilize, NetworkProperty] public int PunchDecayLeftInterval;
        [DontInitilize, NetworkProperty] public bool PunchYawLeftSide; // PunchYaw随机的方向

        [DontInitilize, NetworkProperty] public float PunchYawSpeed;

        //开火后坐力最终到达的强度增量
        [DontInitilize, NetworkProperty] public float TargetPunchPitchDelta;

        public bool IsPullboltInterrupt
        {
            get { return PullBoltInterrupt && !PullBoltFinish; }
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponRuntimeDataComponent;
            PunchDecayLeftInterval = remote.PunchDecayLeftInterval;
            PunchYawLeftSide       = remote.PunchYawLeftSide;
            PunchYawSpeed          = remote.PunchYawSpeed;
            TargetPunchPitchDelta  = remote.TargetPunchPitchDelta;

            Accuracy            = remote.Accuracy;
            LastRenderTimestamp = remote.LastRenderTimestamp;
            NextAttackTimestamp = remote.NextAttackTimestamp;
            LastAttackTimestamp = remote.LastAttackTimestamp;

            CameraRotationInterval = remote.CameraRotationInterval;
            //            PullBoltEnd = remote.PullBoltEnd;
            //            IsPullingBolt = remote.IsPullingBolt;
            //            IsRecoverSightView = remote.IsRecoverSightView;
            //            IsInterruptSightView = remote.IsInterruptSightView;
            ContinuesShootCount = remote.ContinuesShootCount;
            NeedAutoBurstShoot  = remote.NeedAutoBurstShoot;

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
            return PunchDecayLeftInterval == remote.PunchDecayLeftInterval &&
                            PunchYawLeftSide == remote.PunchYawLeftSide &&
                            CompareUtility.IsApproximatelyEqual(PunchYawSpeed, remote.PunchYawSpeed, 3) &&
                            CompareUtility.IsApproximatelyEqual(TargetPunchPitchDelta, remote.TargetPunchPitchDelta,
                                3) && Math.Abs(Accuracy - remote.Accuracy) < 0.01f &&
                            LastRenderTimestamp == remote.LastRenderTimestamp &&
                            NextAttackTimestamp == remote.NextAttackTimestamp &&
                            LastAttackTimestamp == remote.LastAttackTimestamp &&
                            ContinuesShootCount == remote.ContinuesShootCount &&
                            NeedAutoBurstShoot == remote.NeedAutoBurstShoot &&
                            NeedAutoReload == remote.NeedAutoReload &&
                            ContinuesShootReduceTimestamp == remote.ContinuesShootReduceTimestamp &&
                            BurstShootCount == remote.BurstShootCount &&
                            CameraRotationInterval == remote.CameraRotationInterval
                            // && CompareUtility.IsApproximatelyEqual(LastBulletDir ,remote.LastBulletDir) 
                            && Math.Abs(LastSpreadX - remote.LastSpreadX) < 0.0001f &&
                            Math.Abs(LastSpreadY - remote.LastSpreadY) < 0.0001f &&
                            ContinueAttackEndStamp == remote.ContinueAttackEndStamp &&
                            ContinueAttackStartStamp == remote.ContinueAttackStartStamp &&
                            NextAttackPeriodStamp == remote.NextAttackPeriodStamp &&
                            MeleeAttacking == remote.MeleeAttacking && IsPrevCmdFire == remote.IsPrevCmdFire &&
                            PullBoltInterrupt == remote.PullBoltInterrupt;
        }


        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
        LoggerAdapter _loggerAdapter = new LoggerAdapter("pullbolt");
        public void StartPullBolt()
        {
            _loggerAdapter.Info("Start Pullbolt");
            PullBoltFinish    = false;
            PullBoltInterrupt = false;
        }
        public void FinishPullBolt()
        {
            _loggerAdapter.Info("Finish Pullbolt");
            PullBoltFinish    = true;
            PullBoltInterrupt = false;
        }

        public void InterruptPullBolt()
        {
            if (!PullBoltFinish)
            {
                PullBoltInterrupt = true;
            }
        }

     

        public override string ToString()
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}",
                PunchDecayLeftInterval, PunchYawLeftSide, PunchYawSpeed, TargetPunchPitchDelta, Accuracy,
                LastRenderTimestamp, NextAttackTimestamp, LastAttackTimestamp, ContinuesShootCount, NeedAutoBurstShoot,
                NeedAutoReload, ContinuesShootReduceTimestamp, BurstShootCount, LastBulletDir, LastSpreadX, LastSpreadY,
                ContinueAttackEndStamp, ContinueAttackStartStamp, NextAttackPeriodStamp, MeleeAttacking, IsPrevCmdFire,
                PullBoltInterrupt, CameraRotationInterval);
        }

        public void Reset()
        {
            CopyFrom(Empty);
        }
    }
}