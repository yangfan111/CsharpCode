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
using Core;
using Core.UpdateLatest;
using UnityEngine.Serialization;

namespace App.Shared.Components.Player
{
  

    [Player]
    [Serializable]
    
    public class OrientationComponent : IComponent, IUserPredictionComponent, IPlaybackComponent, ICompensationComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.MoveOrientation; } }

        [DontInitilize] [NetworkProperty] public bool AlwaysEqual;
        // 当Yaw不是直接反应鼠标移动，如在载具上，Quaternion to Euler Angle Restricted to [-180, 180]
        // 理想状态是主动旋转按实际路径插值（即支持两个Snapshot超过PI的差值的正确插值）
        // 而人物在载具上既有主动旋转（鼠标移动），也有被动旋转（载具移动），区别主/被动进行插值较复杂
        private float _yaw;
        [NetworkProperty] public float Yaw
        {
            get { return _yaw; }
            set { _yaw = YawPitchUtility.Normalize(value); }
        }

        [NetworkProperty] public float Pitch;
        [NetworkProperty] public float Roll;
        [NetworkProperty] public float PunchYaw;
        [NetworkProperty] public float PunchPitch;
        /// <summary>
        /// WeaponPunchYaw 为 NegPunchYaw的缩放，作用于角色
        /// </summary>
        [FormerlySerializedAs("WeaponPunchYaw")] [NetworkProperty, DontInitilize] public float AccPunchYawValue;
        [FormerlySerializedAs("WeaponPunchPitch")] [NetworkProperty, DontInitilize] public float AccPunchPitchValue;
        [NetworkProperty, DontInitilize] public float ModelPitch;
        [NetworkProperty, DontInitilize] public float ModelYaw;

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
            return CompareUtility.IsApproximatelyEqual(Yaw, r.Yaw)
                   && CompareUtility.IsApproximatelyEqual(Pitch, r.Pitch)
                   && CompareUtility.IsApproximatelyEqual(Roll, r.Roll)
                   && CompareUtility.IsApproximatelyEqual(PunchYaw, r.PunchYaw)
                   && CompareUtility.IsApproximatelyEqual(PunchPitch, r.PunchPitch)
                   && CompareUtility.IsApproximatelyEqual(ModelPitch, r.ModelPitch)
                   && CompareUtility.IsApproximatelyEqual(ModelYaw, r.ModelYaw);
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as OrientationComponent;
            var r = right as OrientationComponent;
            var rotio = interpolationInfo.Ratio;

            Yaw = ShortInterpolateAngle(l.Yaw, r.Yaw, rotio);
            Pitch = InterpolateUtility.Interpolate(l.Pitch, r.Pitch, rotio);
            Roll = InterpolateUtility.Interpolate(l.Roll, r.Roll, rotio);
            PunchYaw = InterpolateUtility.Interpolate(l.PunchYaw, r.PunchYaw, rotio);
            PunchPitch = InterpolateUtility.Interpolate(l.PunchPitch, r.PunchPitch, rotio);
            ModelPitch = ShortInterpolateAngle(l.ModelPitch, r.ModelPitch, rotio);
            ModelYaw = ShortInterpolateAngle(l.ModelYaw, r.ModelYaw, rotio);
        }

        /// <summary>
        /// 按照最短路径插值yaw
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="rotio"></param>
        /// <returns></returns>
        private float ShortInterpolateAngle(float l, float r, float rotio)
        {
            var ret = l;
            if (Math.Abs(l - r) <= 180)
                ret = InterpolateUtility.Interpolate(l, r, rotio);
            else
            {
                ret = l < 0 ?
                    InterpolateUtility.Interpolate(l, r - 360, rotio) :
                    InterpolateUtility.Interpolate(l - 360, r, rotio);
            }

            return ret;
        }

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
    }

    [Player]
    
    public class PlayerRotateLimitComponent : IUpdateComponent
    {

        [NetworkProperty] public bool LimitAngle;
        [NetworkProperty, DontInitilize] public float LeftBound;
        [NetworkProperty, DontInitilize] public float RightBound;
        
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
