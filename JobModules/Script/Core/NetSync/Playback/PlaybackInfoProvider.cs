using System;
using Core.EntityComponent;
using Core.Replicaton;
using Core.SyncLatest;
using Core.Utils;

namespace Core.Playback
{

    public class PlaybackInfoProvider : INetSyncProvider
    {
        private static LoggerAdapter logger = new LoggerAdapter(LoggerNameHolder<PlaybackInfoProvider>.LoggerName);
        private IGameContexts gameContexts;
        private SnapshotPair snapshotPair;

        public PlaybackInfoProvider(IGameContexts gameContexts)
        {
            this.gameContexts = gameContexts;
        }

        public IInterpolationInfo InterpolationInfo
        {
            get { return new InterpolationInfo(snapshotPair); }
        }

        public SnapshotPair SnapshotPair
        {
            get { return snapshotPair; }
        }

        public EntityMap LocalEntityMap
        {
            get { return gameContexts.NonSelfEntityMap; }
        }

        public EntityMap LocalAllEntityMap
        {
            get { return gameContexts.MyEntityMap; }
        }

        public EntityMap RemoteLeftEntityMap
        {
            get { return snapshotPair.LeftSnapshot.NonSelfEntityMap; }
        }

        public EntityMap RemoteRightEntityMap
        {
            get { return snapshotPair.RightSnapshot.NonSelfEntityMap; }
        }


        public void DestroyLocalEntity(IGameEntity entity)
        {
            logger.DebugFormat("destroy local entity {0}", entity.EntityKey);
            entity.MarkDestroy();
        }

        public bool IsSelf(EntityKey key)
        {
            throw new NotImplementedException();
        }

        public IGameEntity CreateAndGetLocalEntity(EntityKey entityKey)
        {
            logger.DebugFormat("create local entity {0}", entityKey);
            return gameContexts.CreateAndGetGameEntity(entityKey);
        }

        public void Update(SnapshotPair snapshotPair)
        {
            this.snapshotPair = snapshotPair;
        }

        public bool IsReady()
        {
            return snapshotPair != null;
        }

        public bool IsRemoteEntityExists(EntityKey entityKey)
        {
            return snapshotPair.RightSnapshot.EntityMap.ContainsKey(entityKey);
        }

        public EntityMap GetRemoteAllEntityMap()
        {
            return snapshotPair.RightSnapshot.EntityMap;
        }
    }
}