using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.Replicaton
{
    public class EntityMapCache
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(EntityMapCache)){}
            public override object MakeObject()
            {
                return new EntityMapCache();
            }
        }
        public static EntityMapCache Allocate(EntityMap entityId2Entity)
        {
            EntityMapCache cache = ObjectAllocatorHolder<EntityMapCache>.Allocate();
            cache.SetEntityMap(entityId2Entity);
            return cache;
        }

        public static void Free(EntityMapCache cache)
        {
            ObjectAllocatorHolder<EntityMapCache>.Free(cache);
        }

        protected EntityMapCache()
        {
        }

        ReplicationFilter _filter = new ReplicationFilter();
        private EntityKey _self;
        private EntityMap _entityMap;
        private EntityMap _selfEntityMapCache;
        private EntityMap _nonSelfEntityMapCache;
        private EntityMap _compensationEntityMapCache;
        private EntityMap _latestestEntityMapCache;
#pragma warning disable RefCounter001
        public void SetEntityMap(EntityMap entityId2Entity)
#pragma warning restore RefCounter001
        {
            _entityMap = entityId2Entity;
        }

        public EntityKey Self
        {
            set
            {
                if (!(_self == value))
                {
                    _self = value;
                    InvalidCache();
                }
            }
            get { return _self; }
        }

        public void OnEntityAdded(IGameEntity entity)
        {
            AddToSelfNonSelf(entity);

            AddToCompensation(entity);

            AddToLatest(entity);
        }

        public void OnEntityRemoved(EntityKey entityKey)
        {
            if (_nonSelfEntityMapCache != null)
                _nonSelfEntityMapCache.Remove(entityKey);
            if (_selfEntityMapCache != null)
                _selfEntityMapCache.Remove(entityKey);
            if (_latestestEntityMapCache != null)
                _latestestEntityMapCache.Remove(entityKey);
            if (_compensationEntityMapCache != null)
                _compensationEntityMapCache.Remove(entityKey);
        }

        public void OnEntityComponentChanged(IGameEntity entity, int index)
        {
           
            OnEntityRemoved(entity.EntityKey);
            OnEntityAdded(entity);
            
        }

        private void AddToSelfNonSelf(IGameEntity entity)
        {
            if (_filter.IsSyncNonSelf(entity, Self))
            {
                if (_nonSelfEntityMapCache != null)
                    _nonSelfEntityMapCache.Add(entity.EntityKey, entity);
            }
            else if (_filter.IsSyncSelf(entity, Self))
            {
                if (_selfEntityMapCache != null)
                    _selfEntityMapCache.Add(entity.EntityKey, entity);
            }
        }

        private void AddToLatest(IGameEntity entity)
        {
            if (_filter.IsSyncSelfOrThird(entity, Self))
            {
                if (_latestestEntityMapCache != null)
                    _latestestEntityMapCache.Add(entity.EntityKey, entity);
            }
        }

        private void AddToCompensation(IGameEntity entity)
        {
            if (_filter.IsCompensation(entity))
            {
                if (_compensationEntityMapCache != null)
                    _compensationEntityMapCache.Add(entity.EntityKey, entity);
            }
        }

        public EntityMap SelfEntityMap
        {
            get
            {
                if (_selfEntityMapCache == null)
                {
                    InitMiscEntityMap();
                }
                return _selfEntityMapCache;
            }
        }

        
        private void InitMiscEntityMap()
        {
            _selfEntityMapCache = CacheEntityMap.Allocate(false);
            _nonSelfEntityMapCache = CacheEntityMap.Allocate(false);
            foreach (var entity in _entityMap.Values)
            {
                AddToSelfNonSelf(entity);
            }
        }

        public EntityMap NonSelfEntityMap
        {
            get
            {
                if (_nonSelfEntityMapCache == null)
                {
                    InitMiscEntityMap();
                }
                return _nonSelfEntityMapCache;
            }
        }

        public EntityMap CompensationEntityMap
        {
            get
            {
                if (_compensationEntityMapCache == null)
                {
                    _compensationEntityMapCache = CacheEntityMap.Allocate(false);
                    foreach (var entity in _entityMap.Values)
                    {
                        AddToCompensation(entity);
                    }
                }
                return _compensationEntityMapCache;
            }
        }

        public EntityMap LatestEntityMap
        {
            get
            {
                if (_latestestEntityMapCache == null)
                {
                    _latestestEntityMapCache = CacheEntityMap.Allocate(false);
                    foreach (var entity in _entityMap.Values)
                    {
                        AddToLatest(entity);
                    }
                }
                return _latestestEntityMapCache;
            }
        }

        public void InvalidCache()
        {
            if (_compensationEntityMapCache != null)
            {
                _compensationEntityMapCache.ReleaseReference();
                _compensationEntityMapCache = null;
            }
            if (_nonSelfEntityMapCache != null)
            {
                _nonSelfEntityMapCache.ReleaseReference();
                _nonSelfEntityMapCache = null;
            }

            if (_selfEntityMapCache != null)
            {
                _selfEntityMapCache.ReleaseReference();
                _selfEntityMapCache = null;
            }
            if (_latestestEntityMapCache != null)
            {
                _latestestEntityMapCache.ReleaseReference();
                _latestestEntityMapCache = null;
            }
        }
    }
}