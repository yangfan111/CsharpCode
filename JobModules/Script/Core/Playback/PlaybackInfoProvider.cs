using Core.Compensation;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Replicaton;
using Core.Utils;

namespace Core.Playback
{
    public class PlaybackInfoProvider : IPlaybackInfoProvider
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<PlaybackInfoProvider>.LoggerName);
        private IGameContexts _gameContexts;
        private SnapshotPair _snapshotPair;
        public PlaybackInfoProvider(
            IGameContexts gameContexts)
        {
            _gameContexts = gameContexts;            
        }

        public void Update(SnapshotPair snapshotPair, ISnapshot lastestSnapshot)
        {
            _snapshotPair = snapshotPair;
        }

        



        public IInterpolationInfo InterpolationInfo
        {
            get { return new InterpolationInfo(_snapshotPair); }
        }

        public SnapshotPair SnapshotPair
        {
            get { return _snapshotPair; }
        }

        public bool IsReady()
        {
            return _snapshotPair != null;
        }

        public void DestroyLocalEntity(IGameEntity entity)
        {
            
            _logger.DebugFormat("destroy local entity {0}", entity.EntityKey);
            entity.MarkDestroy();
        }

        public IGameEntity CreateLocalEntity(EntityKey entityKey)
        {
            _logger.DebugFormat("create local entity {0}", entityKey);
            return _gameContexts.CreateAndGetGameEntity(entityKey);
        }

        public EntityMap LocalEntityMap
        {
            get { return _gameContexts.NonSelfEntityMap; }
        }

        public EntityMap LocalAllEntityMap
        {
            get { return _gameContexts.EntityMap; }
        }

        public EntityMap RemoteLeftEntityMap
        {
            get { return _snapshotPair.LeftSnapshot.NonSelfEntityMap; }
        }

        public EntityMap RemoteRightEntityMap
        {
            get { return _snapshotPair.RightSnapshot.NonSelfEntityMap; }
        }

        public bool IsRemoteEntityExists(EntityKey entityKey)
        {
            return _snapshotPair.RightSnapshot.EntityMap.ContainsKey(entityKey);
        }

        public EntityMap GetRemoteAllEntityMap()
        {

            return _snapshotPair.RightSnapshot.EntityMap;

        }


    }
}