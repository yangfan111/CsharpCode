using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class RagDollComponent : ISelfLatestComponent, IPlaybackComponent
    {
        [NetworkProperty] public string RigidBodyTransformName;
        [NetworkProperty] public Vector3 ForceAtPosition;
        [NetworkProperty] public Vector3 Impulse;
        
        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerRagDoll;
        }

        public void CopyFrom(object target)
        {
            var right = (RagDollComponent) target;
            RigidBodyTransformName = right.RigidBodyTransformName;
            ForceAtPosition = right.ForceAtPosition;
            Impulse = right.Impulse;
        }
        
        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
        
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }
}
