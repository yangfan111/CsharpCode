using Core.Compensation;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Replicaton;
using Core.Utils;

namespace Core.Prediction
{
    public abstract class AbstractPredictionInfoProvider<TPredictionComponent> : IPredictionRewindInfoProvider
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<TPredictionComponent>.LoggerName);
        
        private IGameContexts _gameContexts;
        private ISnapshot _latestSnapshot;
        private ISnapshot _prevSnapshot;
        private ISnapshotSelectorContainer _snapshotSelector;
        public AbstractPredictionInfoProvider(
            ISnapshotSelectorContainer snapshotSelector,
            IGameContexts gameContexts)
        {
            _snapshotSelector = snapshotSelector;
            _gameContexts = gameContexts;


        }

        public void DestroyLocalEntity(IGameEntity entity)
        {
            _logger.DebugFormat("destroy local entity {0}", entity.EntityKey);
            entity.MarkDestroy();
            
        }

        public IGameEntity CreateAndGetLocalEntity(EntityKey entityKey)
        {
            _logger.DebugFormat("create local entity {0}", entityKey);
            var rc = _gameContexts.CreateAndGetGameEntity(entityKey);
            
            return rc;
        }

        public EntityMap LocalEntityMap
        {
            get { return _gameContexts.SelfEntityMap; }
        }

        public EntityMap RemoteEntityMap
        {
            get { return _latestSnapshot.SelfEntityMap; }
        }

        public bool IsRemoteEntityExists(EntityKey entityKey)
        {
            return _latestSnapshot.EntityMap.ContainsKey(entityKey);
        }

        


        public abstract int RemoteHistoryId { get; }

        public void Update()
        {
            _prevSnapshot = _latestSnapshot;
            _latestSnapshot = _snapshotSelector.SnapshotSelector.LatestSnapshot;
        }

        public void OnRewind()
        {
        }


        public IGameEntity GetLocalEntity(EntityKey entityKey)
        {
            return _gameContexts.GetGameEntity(entityKey);
        }

        public ISnapshot LatestSnapshot
        {
            get { return _latestSnapshot; }
        }
        public bool IsLatestSnapshotChanged()
        {
            if (_prevSnapshot != null && _latestSnapshot != null)
            {
                return _prevSnapshot.SnapshotSeq != _latestSnapshot.SnapshotSeq;
            }
            else if (_prevSnapshot != null && _latestSnapshot == null)
                return true;
            else if (_prevSnapshot == null && _latestSnapshot != null)
                return true;
            else
            {
                return false;
            }
        }

        
        public abstract bool IsReady();


        public abstract void AfterPredictionInit(bool isRewinded);
    }
}