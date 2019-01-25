using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Compare;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.VehiclePrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{
    public abstract class CarWheelComponent : IInterpolatableComponent, IRewindableComponent, IVehicleLatestComponent, IVehicleResetableComponent
    {
        protected static LoggerAdapter _logger = new LoggerAdapter(typeof(CarWheelComponent));

        public bool ServerAuthorative;

        [DontInitilize]
        public bool IsSyncLatest { get; set; }

        [NetworkProperty] 
		[DontInitilize]
        public float ColliderSteerAngle;

        [NetworkProperty] 
		[DontInitilize]
        public float SteerAngle;

        public void CopyFrom(object rightComponent)
        {
            var r = (CarWheelComponent) rightComponent;

            ColliderSteerAngle = r.ColliderSteerAngle;
            SteerAngle = r.SteerAngle;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (CarWheelComponent) left;
            var r = (CarWheelComponent) right;

            if (IsSyncLatest)
            {
                return;
            }

            CopyFrom(r);
            var rotio = interpolationInfo.Ratio;
            ColliderSteerAngle =
                InterpolateUtility.Interpolate(l.ColliderSteerAngle, r.ColliderSteerAngle, rotio);
            SteerAngle = InterpolateUtility.Interpolate(l.SteerAngle, r.SteerAngle, rotio);
        }

        public override string ToString()
        {
            return string.Format("Collider Steer Angle {0} SteerAngle {1}",
                ColliderSteerAngle, SteerAngle);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void RewindTo(object rightComponent)  {
            CopyFrom(rightComponent);
        }

        public abstract int GetComponentId();

        public void Reset()
        {
            IsSyncLatest = false;
            ColliderSteerAngle = 0;
            SteerAngle = 0;
        }
    }

    public abstract class CarRewindWheelComponent : CarWheelComponent, IVehiclePredictionComponent, IPlaybackComponent
    {
        
      
        public bool IsApproximatelyEqual(object right)
        {
            if (ServerAuthorative)
            {
                var r = right as CarRewindWheelComponent;
                const float delta = 0.000001f;
                return CompareUtility.IsApproximatelyEqual(SteerAngle, r.SteerAngle, delta) &&
                       CompareUtility.IsApproximatelyEqual(ColliderSteerAngle, r.ColliderSteerAngle, delta);
            }

            return true;
        }

       
    }

    


    [Vehicle]
    
    public class CarFirstRewnWheelComponent : CarRewindWheelComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.CarFirstRewnWheel;
        }

        
    }

    [Vehicle]
    
    public class CarSecondRewnWheelComponent : CarRewindWheelComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.CarSecondRewnWheel;
        }
    }

    [Vehicle]
    
    public class CarThirdRewnWheelComponent : CarRewindWheelComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.CarThirdRewnWheel;
        }
    }

    [Vehicle]
    
    public class CarFourthRewnWheelComponent : CarRewindWheelComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.CarFourthRewnWheel;
        }
    }
}
