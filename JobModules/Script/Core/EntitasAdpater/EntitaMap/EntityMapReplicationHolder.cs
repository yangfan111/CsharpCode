using Core.Replicaton;

namespace Core.EntityComponent
{
    /// <summary>
    /// 以entity.IsCompensation ，enity.IsSyncSelf,entity.IsSyncNoSelf为过滤整个entity添加/删除
    /// </summary>
    public abstract class EntityMapReplicationHolder
    {
        public EntityMapReplicationHolder(IEntityMapOrigin origin)
        {
            this.origin = origin;
        }

        private  IEntityMapOrigin origin;
        protected static readonly ReplicationFilter filter = new ReplicationFilter();

        protected EntityMap OriginEntityMap
        {
            get { return origin.OriginEntityMap; }
        }
        protected CacheEntityMap ownedMap;
        protected EntityKey Self
        {
            get { return origin.MyKey; }
        }
        public bool Initialized { get; private set; }

        public CacheEntityMap CachedMap
        {
            get
            {
                Init();
                return ownedMap;
            }
        }

        public void Init()
        {
            if (ownedMap == null)
            {
                ownedMap = CacheEntityMap.Allocate(false);
                Init(ownedMap);
            }
        }

        private void Init(CacheEntityMap map)
        {
            foreach (var entity in OriginEntityMap.Values)
            {
                CacheEntity(entity);
            }

            Initialized = true;
        }

        public void Clear()
        {
            if (ownedMap != null)
            {
                ownedMap.ReleaseReference();
                ownedMap = null;
            }

            Initialized = false;
        }

        public void Remove(EntityKey entityKey)
        {
            if (ownedMap != null)
            {
                ownedMap.Remove(entityKey);
            }
        }

        public bool CacheEntity(IGameEntity entity)
        {
            if (ownedMap != null && Filter(entity))
            {
                ownedMap.Add(entity.EntityKey, entity);
                return true;
            }

            return false;
        }

        protected abstract bool Filter(IGameEntity entity);

    }
        //IsCompensation
    public class CompensationEntityMapReplicationHolder : EntityMapReplicationHolder
    {
        public CompensationEntityMapReplicationHolder(IEntityMapOrigin @group) : base(@group)
        {
        }

        protected override bool Filter(IGameEntity entity)
        {
            return filter.IsCompensation(entity);

        }
    }
    // FlagSyncNoSelfComponent,FlagSyncSelfComponent
    public class LatestestEntityMapReplicationHolder : EntityMapReplicationHolder
    {
       
        protected override bool Filter(IGameEntity entity)
        {
            return filter.IsSyncSelfOrThird(entity, Self);
        }

        public LatestestEntityMapReplicationHolder(EntityMapCacheGroup @group) : base(@group)
        {
        }
    }
    //IsSyncNonSelf
    public class NonSelfEntityMapReplicationHolder : EntityMapReplicationHolder
    {
        public NonSelfEntityMapReplicationHolder(EntityMapCacheGroup @group) : base(@group)
        {
        }
        protected override bool Filter(IGameEntity entity)
        {
            return filter.IsSyncNonSelf(entity, Self);
        }
       
    }
//IsSyncSelf
    public class SelfEntityMapReplicationHolder : EntityMapReplicationHolder
    {
        public SelfEntityMapReplicationHolder(EntityMapCacheGroup @group) : base(@group)
        {
        }
        protected override bool Filter(IGameEntity entity)
        {
            return filter.IsSyncSelf(entity, Self);
        }
     
    }
}