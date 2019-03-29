using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas.CodeGeneration.Attributes;

namespace Core.Components
{
    [Player, ]
    public class SoundComponent : IPlaybackComponent 
    {
        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerSound;
        }

        [DontInitilize, NetworkProperty] public long PlayOnce;
        [DontInitilize, NetworkProperty] public int Playing;
        [DontInitilize] public long LastPlayOnce;
        [DontInitilize] public int LastPlaying;

        
        public void CopyFrom(object rightComponent)
        {
            var remoteComp = rightComponent as SoundComponent;
            Playing = remoteComp.Playing;
            PlayOnce = remoteComp.PlayOnce;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
