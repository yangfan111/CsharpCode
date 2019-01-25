using System.Runtime.Remoting.Messaging;
using Core.Compensation;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Prediction;
using Core.Replicaton;
using Core.Utils;

namespace Core.SyncLatest
{
    public interface ISyncLatestHandler
    {
        EntityMap RemoteEntityMap { get; }
        EntityMap LocalEntityMap { get; }
        IGameEntity CreateAndGetLocalEntity(EntityKey entityKey);
        void DestroyLocalEntity(IGameEntity entity);
        ISnapshot LatestSnapshot { get; }
        void SetSelf(EntityKey self);
        bool IsSelf(EntityKey key);
    }

    public class SyncLatestHandler : ISyncLatestHandler
    {
        private ISnapshotSelectorContainer _snapshotPool;
        private IGameContexts _gameContexts;
        private ISnapshotEntityMapFilter _snapshotEntityMapFilter;

        public SyncLatestHandler(
            ISnapshotSelectorContainer snapshotPool, 
            IGameContexts gameContexts, 
            ISnapshotEntityMapFilter snapshotEntityMapFilter)
        {
            _snapshotPool = snapshotPool;
            _gameContexts = gameContexts;
            _snapshotEntityMapFilter = snapshotEntityMapFilter;
        }

        public ISnapshot LatestSnapshot
        {
            get { return _snapshotPool.SnapshotSelector.LatestSnapshot; }
        }

        public void SetSelf(EntityKey self)
        {
            _gameContexts.Self = self;
        }

        public bool IsSelf(EntityKey key)
        {
            return _gameContexts.Self.Equals(key);
        }

        public EntityMap RemoteEntityMap
        {
            get { return _snapshotPool.SnapshotSelector.LatestSnapshot.EntityMap; }
        }

        public EntityMap LocalEntityMap
        {
            get { return _gameContexts.LatestEntityMap; }
        }

        public IGameEntity CreateAndGetLocalEntity(EntityKey entityKey)
        {
            return _gameContexts.CreateAndGetGameEntity(entityKey);
        }

        public void DestroyLocalEntity(IGameEntity entity)
        {
            entity.MarkDestroy();
        }
    }

    public interface ISyncLatestManager
    {
        void  SyncLatest();
    }
    public class SyncLatestManager : ISyncLatestManager
    {
        private ISyncLatestHandler _handler;
        private int _latestSnapshotSeq= -1;
        private GameEntitySelfLatestComparator _latestComparator = new GameEntitySelfLatestComparator();
        public SyncLatestManager(ISyncLatestHandler handler)
        {
            _handler = handler;
        }

        public void SyncLatest()
        {
            if (null == _handler.LatestSnapshot)
            {
                return;
            }
            if (_latestSnapshotSeq != _handler.LatestSnapshot.SnapshotSeq)
            {
                _handler.SetSelf(_handler.LatestSnapshot.Self);
                _latestSnapshotSeq = _handler.LatestSnapshot.SnapshotSeq;
                EntityMap remoteEntityMap = SyncLatestEntityMap.Allocate(false);
                EntityMap localEntityMap = SyncLatestEntityMap.Allocate(false);

                remoteEntityMap.AddAll(_handler.RemoteEntityMap);
                localEntityMap.AddAll(_handler.LocalEntityMap); 

                SyncLatestRewindHandler rewindHandler = new SyncLatestRewindHandler(_handler);
                EntityMapComparator.Diff(localEntityMap, remoteEntityMap, rewindHandler, "syncLatest", _latestComparator.Init(rewindHandler,_handler.LatestSnapshot.ServerTime));

                remoteEntityMap.ReleaseReference();
                localEntityMap.ReleaseReference();
            }
        }
    }

    public class SyncLatestRewindHandler : IEntityMapCompareHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SyncLatestRewindHandler));
        private ISyncLatestHandler _handler;
        public SyncLatestRewindHandler(ISyncLatestHandler handler)
        {
            _handler = handler;
        }

        public void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            var entityKey = rightEntity.EntityKey;
            IGameEntity localEntity = _handler.CreateAndGetLocalEntity(entityKey);
            _logger.DebugFormat("create entity {0}", entityKey);
            foreach (var rightComponent in rightEntity.ComponentList)
            {
                if (rightComponent is ILatestComponent && localEntity.GetComponent(rightComponent.GetComponentId()) == null)
                {
                    _logger.DebugFormat("add component {0}:{1}", entityKey, rightComponent.GetType());
                    var leftComponent = (ILatestComponent) localEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);
                  
                }
            }
        }

        public void OnRightEntityMissing(IGameEntity leftEntity)
        {
            _logger.DebugFormat("destroy entity {0}", leftEntity.EntityKey);
            _handler.DestroyLocalEntity(leftEntity);
        }

        public void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("add component {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            var leftComponent = (ILatestComponent)leftEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);
            
        }

        public void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.DebugFormat("remove component {0}:{1}", leftEntity.EntityKey, leftComponent.GetType());
            leftEntity.RemoveComponent(leftComponent.GetComponentId());
        }

        public bool IsBreak()
        {
            return false;
        }

        public bool IsExcludeComponent(IGameComponent component)
        {
            if (_isSelfEntity && component is INonSelfLatestComponent && !(component is ISelfLatestComponent))
                return true;
            if ((component is IVehicleLatestComponent) && !((IVehicleLatestComponent) component).IsSyncLatest)
                return true;
            if (!(component is ILatestComponent))
                return true;
            return false;
        }

        private bool _isSelfEntity; 
        public void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            _isSelfEntity= _handler.IsSelf(leftEntity.EntityKey);
        }

        public void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
        }

        public void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity,
            IGameComponent rightComponent)
        {
            (leftComponent as ILatestComponent).SyncLatestFrom(rightComponent);
        }
    }
}