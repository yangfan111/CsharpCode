using Core;
using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas;
using Core.Free;

namespace App.Shared.Components.Player
{
    [Player, ]
    public class PlayerMaskComponent : ISelfLatestComponent, IPlaybackComponent , IRule
    {
        [NetworkProperty] public int SelfMask;
        [NetworkProperty] public int TargetMask;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerMask;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerMaskComponent;
            SelfMask = remote.SelfMask;
            TargetMask = remote.TargetMask;
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerMaskComponent;
        }
    }

    [Player]
    public class PlayerHitMaskControllerComponent : IComponent
    {
        public IHitMaskController HitMaskController;
    }
}
