using Core.EntityComponent;
using Core.Replicaton;

namespace Core.Playback
{
    public interface IPlaybackInfoProvider
    {
        IInterpolationInfo InterpolationInfo { get; }
        void Update(SnapshotPair snapshotPair, ISnapshot latestSnapshot);
        bool IsReady();
        void DestroyLocalEntity(IGameEntity entity);
        IGameEntity CreateLocalEntity(EntityKey entityKey);
        EntityMap LocalEntityMap { get; }

        EntityMap RemoteLeftEntityMap { get; }
        EntityMap RemoteRightEntityMap { get; }
        EntityMap GetRemoteAllEntityMap();
        bool IsRemoteEntityExists(EntityKey entityKey);
        EntityMap LocalAllEntityMap { get; }
        SnapshotPair SnapshotPair { get; }
    }
}
