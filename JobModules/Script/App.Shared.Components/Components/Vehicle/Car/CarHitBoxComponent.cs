using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Compensation;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    public class CarHitBoxComponent : ICompensationComponent
    {
        [DontInitilize] public Vector3 BodyPosition;
        [DontInitilize] public Quaternion BodyRotation;
        public Vector3[] FlexiblePositionList = new Vector3[(int)VehiclePartIndex.MaxFlexibleCount];
        public Quaternion[] FlexibleRotationList = new Quaternion[(int)VehiclePartIndex.MaxFlexibleCount];
        public Vector3[] WheelPositionList = new Vector3[(int)VehiclePartIndex.MaxWheelCount];
        public Quaternion[] WheelRotationList = new Quaternion[(int)VehiclePartIndex.MaxWheelCount];

        public int GetComponentId()
        {
            return (int) EComponentIds.CarHitBox;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (CarHitBoxComponent) left;
            var r = (CarHitBoxComponent) right;
            var ratio = interpolationInfo.Ratio;
            BodyPosition = InterpolateUtility.Interpolate(l.BodyPosition, r.BodyPosition, ratio);
            BodyRotation = InterpolateUtility.Interpolate(l.BodyRotation, r.BodyRotation, ratio);

            int count = FlexiblePositionList.Length;
            for (int i = 0; i < count; ++i)
            {
                FlexiblePositionList[i] =
                    InterpolateUtility.Interpolate(l.FlexiblePositionList[i], r.FlexiblePositionList[i], ratio);
                FlexibleRotationList[i] =
                    InterpolateUtility.Interpolate(l.FlexibleRotationList[i], r.FlexibleRotationList[i], ratio);
            }

            count = WheelPositionList.Length;
            for (int i = 0; i < count; ++i)
            {
                WheelPositionList[i] =
                    InterpolateUtility.Interpolate(l.WheelPositionList[i], r.WheelPositionList[i], ratio);
                WheelRotationList[i] =
                    InterpolateUtility.Interpolate(l.WheelRotationList[i], r.WheelRotationList[i], ratio);
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (CarHitBoxComponent) rightComponent;

            BodyPosition = r.BodyPosition;
            BodyRotation = r.BodyRotation;

            int count = FlexiblePositionList.Length;
            for (int i = 0; i < count; ++i)
            {
                FlexiblePositionList[i] = r.FlexiblePositionList[i];
                FlexibleRotationList[i] = r.FlexibleRotationList[i];
            }

            count = WheelPositionList.Length;
            for (int i = 0; i < count; ++i)
            {
                WheelPositionList[i] = r.WheelPositionList[i];
                WheelRotationList[i] = r.WheelRotationList[i];
            }
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
