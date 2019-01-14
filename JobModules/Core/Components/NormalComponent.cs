using System;
using Core.Compare;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SnapshotReplication.Serialization.Patch;
using UnityEngine;

namespace Core.Components
{
    
    [Serializable]
    public class NormalComponent : IUserPredictionComponent, IPlaybackComponent
    {
        public int GetComponentId() { { return (int)ECoreComponentIds.Normal; } }

        [NetworkProperty] public Vector3 Value;



        public override string ToString()
        {
            return string.Format("Rotation: {0}", Value);
        }

        public void RewindTo(object rightComponent)
        {
            
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as NormalComponent;
            Value = r.Value;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as NormalComponent;
            return CompareUtility.IsApproximatelyEqual(Value, r.Value);
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (left as NormalComponent);
            var r = right as NormalComponent;
            Value = InterpolateUtility.Interpolate(l.Value,
                r.Value, interpolationInfo);
        }
    }
}