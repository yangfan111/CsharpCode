using Core.ObjectPool;

namespace Core.EntityComponent
{
    public interface IEntityMapOrigin
    {
        EntityKey MyKey { get; }
         EntityMap OriginEntityMap { get; }
    }
    public class EntityMapCacheGroup:IEntityMapOrigin
    {
        private EntityKey myKey;
        
        private EntityMapReplicationHolder compensationEntityMapReplicationHolder;
        private EntityMapReplicationHolder lastestEntityMapReplicationHolder;
        private EntityMapReplicationHolder nonSelfEntityMapReplicationHolder;
        private EntityMapReplicationHolder selfEntityMapReplicationHolder;

        protected EntityMapCacheGroup()
        {
            compensationEntityMapReplicationHolder = new CompensationEntityMapReplicationHolder(this);
            lastestEntityMapReplicationHolder      = new LatestestEntityMapReplicationHolder(this);
            nonSelfEntityMapReplicationHolder      = new NonSelfEntityMapReplicationHolder(this);
            selfEntityMapReplicationHolder         = new SelfEntityMapReplicationHolder(this);
        }

        public EntityMap OriginEntityMap { get; private set; }
        /// <summary>
        /// 由GameContexts.Self传入
        /// </summary>
        public EntityKey MyKey
        {
            set
            {
                if (myKey != value)
                {
                    myKey = value;
                    InvalidCache();
                }
            }
            get { return myKey; }
        }


        public EntityMap SelfEntityMap
        {
            get
            {
                if (!selfEntityMapReplicationHolder.Initialized)
                    InitSelfNonSelfEntityMap();
                return selfEntityMapReplicationHolder.CachedMap;
            }
        }

        public EntityMap NonSelfEntityMap
        {
            get
            {
                if (!nonSelfEntityMapReplicationHolder.Initialized)
                    InitSelfNonSelfEntityMap();
                return nonSelfEntityMapReplicationHolder.CachedMap;
            }
        }

        public EntityMap CompensationEntityMap
        {
            get { return compensationEntityMapReplicationHolder.CachedMap; }
        }


        public EntityMap LatestEntityMap
        {
            get { return lastestEntityMapReplicationHolder.CachedMap; }
        }

        public static EntityMapCacheGroup Allocate(EntityMap entityId2Entity)
        {
            EntityMapCacheGroup cacheGroup = ObjectAllocatorHolder<EntityMapCacheGroup>.Allocate();
            cacheGroup.Initialize(entityId2Entity);
            return cacheGroup;
        }

        public static void Free(EntityMapCacheGroup cacheGroup)
        {
            ObjectAllocatorHolder<EntityMapCacheGroup>.Free(cacheGroup);
        }

        //     private EntityMap latestestEntityMapCache;
#pragma warning disable RefCounter001
        public void Initialize(EntityMap entityId2Entity)
#pragma warning restore RefCounter001
        {
            OriginEntityMap = entityId2Entity;
        }

        public void OnEntityAdded(IGameEntity entity)
        {
            selfEntityMapReplicationHolder.CacheEntity(entity);
            nonSelfEntityMapReplicationHolder.CacheEntity(entity);
            compensationEntityMapReplicationHolder.CacheEntity(entity);
            lastestEntityMapReplicationHolder.CacheEntity(entity);
        }

        public void OnEntityRemoved(EntityKey entityKey)
        {
            nonSelfEntityMapReplicationHolder.Remove(entityKey);
            selfEntityMapReplicationHolder.Remove(entityKey);
            lastestEntityMapReplicationHolder.Remove(entityKey);
            compensationEntityMapReplicationHolder.Remove(entityKey);
        }

        public void OnEntityComponentChanged(IGameEntity entity, int index)
        {
            OnEntityRemoved(entity.EntityKey);
            OnEntityAdded(entity);
        }

        private void InitSelfNonSelfEntityMap()
        {
            nonSelfEntityMapReplicationHolder.Init();
            selfEntityMapReplicationHolder.Init();
            // foreach (var entity in originEntityMap.Values)
            // {
            //     AddToSelfNonSelf(entity);
            // }
        }

        public void InvalidCache()
        {
            compensationEntityMapReplicationHolder.Clear();
            selfEntityMapReplicationHolder.Clear();
            nonSelfEntityMapReplicationHolder.Clear();
            lastestEntityMapReplicationHolder.Clear();
        }

       
    }
}