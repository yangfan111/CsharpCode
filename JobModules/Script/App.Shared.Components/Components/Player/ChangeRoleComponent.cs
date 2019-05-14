using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;

namespace App.Shared.Components.Player
{
    [Player]
    public class ChangeRoleComponent : ISelfLatestComponent, IPlaybackComponent
    {
        [NetworkProperty] public bool ChangeRoleAnimationFinished;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerChangeRole;
        }

        public bool IsInterpolateEveryFrame()
        {
            return false;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var leftComponent = left as PredictedAppearanceComponent;
            CopyFrom(leftComponent);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void CopyFrom(object rightComponent)
        {
            var right = (ChangeRoleComponent)rightComponent;
            if (right != null)
            {
                ChangeRoleAnimationFinished = right.ChangeRoleAnimationFinished;
            }
        }

        public override string ToString()
        {
            return string.Format("ChangeRoleAnimationFinished: {0}", ChangeRoleAnimationFinished);
        }
    }
}
