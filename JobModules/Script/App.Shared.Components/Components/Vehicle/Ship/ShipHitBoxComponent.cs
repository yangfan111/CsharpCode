using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Compensation;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction.VehiclePrediction.Cmd;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    public class ShipHitBoxComponent : ICompensationComponent
    {
        [DontInitilize] public Vector3 BodyPosition;
        [DontInitilize] public Quaternion BodyRotation;
        public Vector3[] RudderPositionList = new Vector3[(int)VehiclePartIndex.MaxRudderCount];
        public Quaternion[] RudderRotationList = new Quaternion[(int)VehiclePartIndex.MaxRudderCount];

        public int GetComponentId()
        {
            return (int) EComponentIds.ShipHixBox;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (ShipHitBoxComponent)left;
            var r = (ShipHitBoxComponent)right;

            BodyPosition = InterpolateUtility.Interpolate(l.BodyPosition, r.BodyPosition, interpolationInfo);
            BodyRotation = InterpolateUtility.Interpolate(l.BodyRotation, r.BodyRotation, interpolationInfo);

            int count = RudderPositionList.Length;
            for (int i = 0; i < count; ++i)
            {
                RudderPositionList[i] = InterpolateUtility.Interpolate(l.RudderPositionList[i], r.RudderPositionList[i],
                    interpolationInfo);
                RudderRotationList[i] = InterpolateUtility.Interpolate(l.RudderRotationList[i], r.RudderRotationList[i],
                    interpolationInfo);
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (ShipHitBoxComponent) rightComponent;

            BodyPosition = r.BodyPosition;
            BodyRotation = r.BodyRotation;

            int count = RudderPositionList.Length;
            for (int i = 0; i < count; ++i)
            {
                RudderPositionList[i] = r.RudderPositionList[i];
                RudderRotationList[i] = r.RudderRotationList[i];
            }
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
