using Core.EntityComponent;
using Core.Replicaton;

namespace Core.SyncLatest
{
    /// <summary>
    /// ************************[Entity提取]************************
    /// FlagSyncSelf:ISelfLastest  FlagSyncNoSelf:ISelfNoLastest
    /// 把EntityMap中带有FlagSyncSelf，FlagSyncNoSelf与Self逻辑匹配的都提取出来
    /// ************************[INetSyncProvider]************************
    /// 提供GameContexts创建GameEntity方法
    /// ************************[Component提取]************************
    /// ILatestComponent|| ISelfNoLastest,ISelfLastest与Self逻辑匹配的都提取出来
    /// ************************[EntityMissing]************************
    /// Entity全量同步,收集所有IGameComponent，Compoent提取规则同步
    /// ************************[Entity差异比较]************************
    /// 收集所有ILatestComponent,按Compoent提取规则同步
    /// </summary>
    public class SyncLastestManager : INetSyncProvider
    {
        private SyncLatestMapDiffHandler diffHandler;
        private IGameContexts gameContexts;
        private GameEntitySelfLatestCompareAgent latestCompareAgent;
        private int latestSnapshotSeq = -1;
        private ISnapshotSelector snapshotSelector;

        public SyncLastestManager(IGameContexts gameContexts, ISnapshotSelector snapshotSelector)
        {
            this.gameContexts     = gameContexts;
            this.snapshotSelector = snapshotSelector;
            diffHandler           = new SyncLatestMapDiffHandler(this);
            latestCompareAgent    = new GameEntitySelfLatestCompareAgent();
        }
        
        private EntityMap RemoteEntityMap
        {
            get { return snapshotSelector.LatestSnapshot.LatestEntityMap; }
        }

        /// <summary>
        ///has FlagSyncNoSelfComponent/FlagSyncSelfComponent
        /// </summary>
        private EntityMap LocalEntityMap
        {
            get { return gameContexts.LatestEntityMap; }
        }

        public IGameEntity CreateAndGetLocalEntity(EntityKey entityKey)
        {
            return gameContexts.CreateAndGetGameEntity(entityKey);
        }

        public void DestroyLocalEntity(IGameEntity entity)
        {
            entity.MarkDestroy();
        }

        public bool IsSelf(EntityKey key)
        {
            return gameContexts.Self.Equals(key);
        }

        public void SyncLatest()
        {
            var lastestSnapshot = snapshotSelector.LatestSnapshot;
            if (null == lastestSnapshot)
            {
                return;
            }
            if (latestSnapshotSeq != lastestSnapshot.SnapshotSeq)
            {
                gameContexts.Self = lastestSnapshot.Self;
                latestSnapshotSeq = lastestSnapshot.SnapshotSeq;
                EntityMap remoteEntityMap = SyncLatestEntityMap.Allocate(false);
                EntityMap localEntityMap  = SyncLatestEntityMap.Allocate(false);

                remoteEntityMap.AddAll(RemoteEntityMap);
                localEntityMap.AddAll(LocalEntityMap);
                latestCompareAgent.Init(diffHandler, lastestSnapshot.ServerTime);
                EntityMapCompareExecutor.Diff(localEntityMap, remoteEntityMap, diffHandler, "syncLatest",
                    latestCompareAgent);

                RefCounterRecycler.Instance.ReleaseReference(remoteEntityMap);
                RefCounterRecycler.Instance.ReleaseReference(localEntityMap);
            }
        }
    }

    public interface INetSyncProvider
    {
        IGameEntity CreateAndGetLocalEntity(EntityKey entityKey);
        void        DestroyLocalEntity(IGameEntity entity);
        bool        IsSelf(EntityKey key);
    }
}