using System;
using Core.CameraControl;

using Core.Compare;
using Core.Compensation;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Core;
using Core.Components;
using Core.UpdateLatest;
using Core.Utils;
using UnityEngine.Serialization;
using Core.Free;

namespace App.Shared.Components.Player
{
  

    [Player]
    [Serializable]
    
    public class OrientationComponent : IComponent, IUserPredictionComponent, IPlaybackComponent, ICompensationComponent, IRule
    {
        public int GetComponentId() { { return (int)EComponentIds.MoveOrientation; } }

        [DontInitilize] [NetworkProperty] public bool AlwaysEqual;
        // 当Yaw不是直接反应鼠标移动，如在载具上，Quaternion to Euler Angle Restricted to [-180, 180]
        // 理想状态是主动旋转按实际路径插值（即支持两个Snapshot超过PI的差值的正确插值）
        // 而人物在载具上既有主动旋转（鼠标移动），也有被动旋转（载具移动），区别主/被动进行插值较复杂
        private float _yaw;
        [NetworkProperty(SyncFieldScale.Yaw)] public float Yaw
        {
            get { return _yaw.FloatPrecision(3); }
            set { _yaw = YawPitchUtility.Normalize(value); }
        }
        public float YawRound
        {
            get { return _yaw.FloatRoundPrecision(2); }

        }
        public float PitchRound
        {
            get { return _pitch.FloatRoundPrecision(2); }

        }

   
        private float _pitch;
        [NetworkProperty(SyncFieldScale.Pitch)] public float Pitch
        {
            get { return _pitch.FloatPrecision(3); }
            set { _pitch = value; }
        }
        [NetworkProperty(SyncFieldScale.Roll)] public float Roll;
        [NetworkProperty(SyncFieldScale.Yaw)] public float PunchYaw;
        [NetworkProperty(SyncFieldScale.Pitch)] public float PunchPitch;
        /// <summary>
        /// WeaponPunchYaw 为 NegPunchYaw的缩放，作用于角色
        /// </summary>
        [FormerlySerializedAs("WeaponPunchYaw")] [NetworkProperty(SyncFieldScale.Yaw), DontInitilize] public float AccPunchYawValue;
        [FormerlySerializedAs("WeaponPunchPitch")] [NetworkProperty(SyncFieldScale.Pitch), DontInitilize] public float AccPunchPitchValue;
        [NetworkProperty(SyncFieldScale.Pitch), DontInitilize] public float ModelPitch;
        [NetworkProperty(SyncFieldScale.Yaw), DontInitilize] public float ModelYaw;

        [DontInitilize] public float FireRoll;

        [DontInitilize]
        public float AccPunchYaw
        {
            get { return -PunchYaw; }
            set { PunchYaw = -value; }
        }
        [DontInitilize]
        public float AccPunchPitch
        {
            get { return -PunchPitch; }
            set { PunchPitch = -value; }
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as OrientationComponent;
            Yaw = r.Yaw;
            Pitch = r.Pitch;
            Roll = r.Roll;
            PunchYaw = r.PunchYaw;
            PunchPitch = r.PunchPitch;
            AlwaysEqual = r.AlwaysEqual;
            ModelPitch = r.ModelPitch;
            ModelYaw = r.ModelYaw;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as OrientationComponent;
            if (r.AlwaysEqual || AlwaysEqual)
                return true;
            return CompareUtility.IsApproximatelyEqual(Yaw, r.Yaw, 0.02f)
                   && CompareUtility.IsApproximatelyEqual(Pitch, r.Pitch, 0.02f)
                   && CompareUtility.IsApproximatelyEqual(Roll, r.Roll, 0.02f)
                   && CompareUtility.IsApproximatelyEqual(PunchYaw, r.PunchYaw, 0.02f)
                   && CompareUtility.IsApproximatelyEqual(PunchPitch, r.PunchPitch, 0.02f)
                   && CompareUtility.IsApproximatelyEqual(ModelPitch, r.ModelPitch, 0.02f)
                   && CompareUtility.IsApproximatelyEqual(ModelYaw, r.ModelYaw, 0.02f);
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as OrientationComponent;
            var r = right as OrientationComponent;
            var rotio = interpolationInfo.Ratio;

            Yaw = InterpolateUtility.ShortInterpolate(l.Yaw, r.Yaw, interpolationInfo);
            Pitch = InterpolateUtility.Interpolate(l.Pitch, r.Pitch, rotio);
            Roll = InterpolateUtility.Interpolate(l.Roll, r.Roll, rotio);
            PunchYaw = InterpolateUtility.Interpolate(l.PunchYaw, r.PunchYaw, rotio);
            PunchPitch = InterpolateUtility.Interpolate(l.PunchPitch, r.PunchPitch, rotio);
            ModelPitch = InterpolateUtility.ShortInterpolate(l.ModelPitch, r.ModelPitch, interpolationInfo);
            ModelYaw = InterpolateUtility.ShortInterpolate(l.ModelYaw, r.ModelYaw, interpolationInfo);
        }

        /// <summary>
        /// 按照最短路径插值yaw
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="rotio"></param>
        /// <returns></returns>

        public Quaternion ModelView
        {
            get {return Quaternion.Euler(ModelPitch, ModelYaw, 0);}
        }

        public Quaternion RotationView
        {
            get { return Quaternion.Euler(Pitch, Yaw, 0); }
        }

        public Vector3 EulerAngle
        {
            get { return new Vector3(Pitch, Yaw, 0); }
        }

        [DontInitilize]
        public Quaternion RotationYaw
        {
            //get { return Quaternion.Euler(0, Yaw + PunchYaw, 0); }
            get { return Quaternion.Euler(0, Yaw - AccPunchYawValue, 0); }
        }

        public override string ToString()
        {
            return string.Format("AlwaysEqual: {0}, Yaw: {1}, Pitch: {2}, Roll: {3}, PunchYaw: {4}, PunchPitch: {5}, WeaponPunchYaw: {6}, WeaponPunchPitch: {7}, ModelPitch: {8}, ModelYaw: {9}", AlwaysEqual, _yaw, Pitch, Roll, PunchYaw, PunchPitch, AccPunchYawValue, AccPunchPitchValue, ModelPitch, ModelYaw);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.OrientationComponent;
        }
    }

    [Player]
    
    public class PlayerRotateLimitComponent : IUpdateComponent
    {

        [NetworkProperty] public bool LimitAngle;
        [NetworkProperty(720,-720,0.001f), DontInitilize] public float LeftBound;
        [NetworkProperty(720,-720,0.001f), DontInitilize] public float RightBound;
        
        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as PlayerRotateLimitComponent;
            if (right != null)
            {
                LimitAngle = right.LimitAngle;
                LeftBound = right.LeftBound;
                RightBound = right.RightBound;
            }
        }

        public void SetNoLimit()
        {
            LimitAngle = false;
            LeftBound = 0f;
            RightBound = 0f;
        }
        
        public void SetLimit(float leftBound, float rightBound)
        {
            LimitAngle = true;
            LeftBound = leftBound;
            RightBound = rightBound;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerRotateLimit;
        }
    }
}
