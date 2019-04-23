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
        [DontInitilize, NetworkProperty] public int PunchDecayLeftInterval; 
        [DontInitilize, NetworkProperty] public bool PunchYawLeftSide; // PunchYaw随机的方向
        [DontInitilize, NetworkProperty] public float PunchYawSpeed;
        //开火后坐力最终到达的强度增量
        [DontInitilize, NetworkProperty] public float TargetPunchPitchDelta;
        [DontInitilize, NetworkProperty] public int LastRenderTime;
        [DontInitilize, NetworkProperty] public float CameraRotationSpeed;

        //精准
        [DontInitilize, NetworkProperty] public float Accuracy;

        //攻击时间
        [DontInitilize, NetworkProperty] public int LastAttackTimestamp;

        [DontInitilize, NetworkProperty] public int NextAttackTimestamp;
        //连发
        [DontInitilize, NetworkProperty] public int ContinuesShootCount;
        //子弹连射衰减时间戳
        [DontInitilize, NetworkProperty] public int ContinuesShootReduceTimestamp;
        //        [DontInitilize, NetworkProperty] public bool PullBoltEnd;//是否拉过栓了
        //        [DontInitilize, NetworkProperty] public bool IsPullingBolt;//是否在拉栓中
        //        [DontInitilize, NetworkProperty] public bool IsInterruptSightView;
        //        [DontInitilize, NetworkProperty] public bool IsRecoverSightView;


        //        [System.Obsolete]
        //        [DontInitilize, NetworkProperty] public bool    NeedReduceContinuesShootCD;

        [DontInitilize, NetworkProperty] public int BurstShootCount;

        [DontInitilize, NetworkProperty] public Vector3 LastBulletDir;

        //准星扩散
        [DontInitilize, NetworkProperty] public float LastSpreadX;
        [DontInitilize, NetworkProperty] public float LastSpreadY;
        [DontInitilize, NetworkProperty] public int ContinueAttackEndStamp;
        [DontInitilize, NetworkProperty] public int ContinueAttackStartStamp;
        [DontInitilize, NetworkProperty] public int NextAttackPeriodStamp;
        [DontInitilize, NetworkProperty] public bool MeleeAttacking;
        [DontInitilize, NetworkProperty] public bool IsPrevCmdFire;

        //拉栓相关
        [DontInitilize, NetworkProperty] public bool IsPullingBolt; //是否在拉栓中
        [DontInitilize, NetworkProperty] public bool PullBoltInterrupt;



        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponRuntimeDataComponent;
            PunchDecayLeftInterval = remote.PunchDecayLeftInterval;
            PunchYawLeftSide = remote.PunchYawLeftSide;
            PunchYawSpeed = remote.PunchYawSpeed;
            TargetPunchPitchDelta = remote.TargetPunchPitchDelta;

            Accuracy = remote.Accuracy;
            LastRenderTime = remote.LastRenderTime;
            NextAttackTimestamp = remote.NextAttackTimestamp;
            LastAttackTimestamp = remote.LastAttackTimestamp;
            //            PullBoltEnd = remote.PullBoltEnd;
            //            IsPullingBolt = remote.IsPullingBolt;
            //            IsRecoverSightView = remote.IsRecoverSightView;
            //            IsInterruptSightView = remote.IsInterruptSightView;
            ContinuesShootCount = remote.ContinuesShootCount;
            ContinuesShootReduceTimestamp = remote.ContinuesShootReduceTimestamp;
            BurstShootCount = remote.BurstShootCount;
            LastBulletDir = remote.LastBulletDir;
            LastSpreadX = remote.LastSpreadX;
            LastSpreadY = remote.LastSpreadY;
            ContinueAttackEndStamp = remote.ContinueAttackEndStamp;
            ContinueAttackStartStamp = remote.ContinueAttackStartStamp;

            NextAttackPeriodStamp = remote.NextAttackPeriodStamp;
            MeleeAttacking = remote.MeleeAttacking;
            IsPrevCmdFire = remote.IsPrevCmdFire;
            IsPullingBolt = remote.IsPullingBolt;
            PullBoltInterrupt = remote.PullBoltInterrupt;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponRuntimeData;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponRuntimeDataComponent;
            return PunchDecayLeftInterval == remote.PunchDecayLeftInterval
                && PunchYawLeftSide == remote.PunchYawLeftSide
                && PunchYawSpeed == remote.PunchYawSpeed
                && TargetPunchPitchDelta == remote.TargetPunchPitchDelta
                && Accuracy == remote.Accuracy
                && LastRenderTime == remote.LastRenderTime
                && NextAttackTimestamp == remote.NextAttackTimestamp
                && LastAttackTimestamp == remote.LastAttackTimestamp
                //                && PullBoltEnd == remote.PullBoltEnd
                //                && IsPullingBolt == remote.IsPullingBolt
                //                && IsInterruptSightView == remote.IsInterruptSightView
                && ContinuesShootCount == remote.ContinuesShootCount
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