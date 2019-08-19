using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.Replicaton;
using Core.SyncLatest;
using Core.Utils;

namespace Core.Prediction
{
    public abstract class AbstractPredictionProvider
    {
        private static LoggerAdapter logger = new LoggerAdapter("IPredictionRewindProvider");

        private IGameContexts gameContexts;
        private ISnapshot latestSnapshot;
        private ISnapshot prevSnapshot;
        private ISnapshotSelector snapshotSelector;

        public AbstractPredictionProvider(ISnapshotSelector snapshotSelector, IGameContexts gameContexts)
        {
            this.snapshotSelector = snapshotSelector;
            this.gameContexts     = gameContexts;
        }
        /// <summary>
        /// FlagSyncSelfComponent
        /// </summary>
        public EntityMap LocalEntityMap
        {
            get { return gameContexts.SelfEntityMap; }
        }

        public EntityMap RemoteEntityMap
        {
            get { return latestSnapshot.SelfEntityMap; }
        }


        public abstract int LastSelfUserCmdSeqId { get; }

        public ISnapshot LatestSnapshot
        {
            get { return latestSnapshot; }
        }
        /// <summary>
        /// remoteOwnerEntity.UserOwner 
        /// </summary>
        public virtual IUserCmdOwner UserCmdOwner { get; }

        public void DestroyLocalEntity(IGameEntity entity)
        {
            logger.DebugFormat("destroy local entity {0}", entity.EntityKey);
            entity.MarkDestroy();
        }

        public bool IsSelf(EntityKey key)
        {
            return gameContexts.Self == key;
        }

        public IGameEntity CreateAndGetLocalEntity(EntityKey entityKey)
        {
            logger.DebugFormat("create local entity {0}", entityKey);
            var rc = gameContexts.CreateAndGetGameEntity(entityKey);

            return rc;
        }

        public bool IsRemoteEntityExists(EntityKey entityKey)
        {
            return latestSnapshot.EntityMap.ContainsKey(entityKey);
        }

        public void Update()
        {
            prevSnapshot   = latestSnapshot;
            latestSnapshot = snapshotSelector.LatestSnapshot;
        }

        public void OnRewind()
        {
        }


        public void SetSelf(EntityKey self)
        {
            gameContexts.Self = self;
        }

        public IGameEntity GetLocalEntity(EntityKey entityKey)
        {
            return gameContexts.GetGameEntity(entityKey);
        }

        public bool IsReady()
        {
            return LatestSnapshot != null;
        }

        public bool IsLatestSnapshotChanged()
        {
            if (prevSnapshot != null && latestSnapshot != null)
                return prevSnapshot.SnapshotSeq != latestSnapshot.SnapshotSeq;
            if (prevSnapshot == null && latestSnapshot == null)
                return false;
            return true;
        }

        public abstract void AfterPredictionInit(bool isRewinded);
    }
}