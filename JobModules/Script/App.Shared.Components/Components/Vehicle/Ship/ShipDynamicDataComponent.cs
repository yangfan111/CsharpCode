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
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    
    public class ShipDynamicDataComponent : VehicleDynamicDataComponent, IVehiclePredictionComponent, IPlaybackComponent
    {
        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override int GetComponentId()
        {
            return (int) EComponentIds.ShipDynamicData;
        }
    }

    public abstract class ShipRudderDynamicData : IRewindableComponent, IInterpolatableComponent, IComparableComponent, IVehicleLatestComponent, IVehicleResetableComponent
    {
        public bool ServerAuthoritive;

        [DontInitilize]
        public bool IsSyncLatest { get; set; }

        [NetworkProperty]
        [DontInitilize]
        public float Angle;
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as ShipRudderDynamicData;
            var r = right as ShipRudderDynamicData;

            if (IsSyncLatest)
            {
                return;
            }

            Angle = InterpolateUtility.Interpolate(l.Angle, r.Angle, interpolationInfo);
           
        }

        public bool IsApproximatelyEqual(object right)
        {
            if (ServerAuthoritive)
            {
                var r = right as ShipRudderDynamicData;
                const float delta = 0.000001f;
                return CompareUtility.IsApproximatelyEqual(Angle, r.Angle, delta);
            }

            return true;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as ShipRudderDynamicData;

            Angle = r.Angle;
        }
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void Reset()
        {
            IsSyncLatest = false;
            Angle = 0;
        }

        public abstract int GetComponentId();
    }

    [Vehicle]
    
    public class ShipFirstRudderDynamicData : ShipRudderDynamicData, IVehiclePredictionComponent, IPlaybackComponent
    {
        public override int GetComponentId()
        {
            return (int) EComponentIds.ShipFirstRudderDynamicData;
        }
    }

    [Vehicle]
    
    public class ShipSecondRudderDynamicData : ShipRudderDynamicData, IVehiclePredictionComponent, IPlaybackComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.ShipScondRudderDynamicData;
        }
    }
}
