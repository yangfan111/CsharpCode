using System;
using System.Collections.Generic;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.Replicaton
{
    public class CloneSnapshot : Snapshot
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CloneSnapshot)){}
            public override object MakeObject()
            {
                return new CloneSnapshot();
            }
        }
        public static CloneSnapshot Allocate()
        {
            return ObjectAllocatorHolder<CloneSnapshot>.Allocate();
        }
        protected override void OnCleanUp()
        {
            CleanUp();

            ObjectAllocatorHolder<CloneSnapshot>.Free(this);
        }
    }
    
    [Serializable]
    public class Snapshot : BaseRefCounter, ISnapshot
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(Snapshot)){}
            public override object MakeObject()
            {
                return new Snapshot();
            }
        }
        public static Snapshot Allocate()
        {
            return ObjectAllocatorHolder<Snapshot>.Allocate();
        }

        private EntityMap _entityId2Entity;

        [NonSerialized]
        private volatile EntityMapCache _cache;

        private SnapshotHeader _header;

        protected Snapshot()
        {
            _header = new SnapshotHeader();
        }

        public SnapshotHeader Header
        {
            get { return _header; }
            set
            {
                _header.CopyFrom(value);
                _cache.Self = _header.Self;
            }
        }

        public int ServerTime
        {
            get { return _header.ServerTime; }
            set { _header.ServerTime = value; }
        }

        public int VehicleSimulationTime
        {
            get { return _header.VehicleSimulationTime; }
            set { _header.VehicleSimulationTime = value; }
        }


        public int SnapshotSeq
        {
            get { return _header.SnapshotSeq; }
            set { _header.SnapshotSeq = value; }
        }

        public int LastUserCmdSeq
        {
            get { return _header.LastUserCmdSeq; }
            set { _header.LastUserCmdSeq = value; }
        }

        public EntityKey Self
        {
            set
            {
                _header.Self = value;
                _cache.Self = _header.Self;
            }
            get
            {
                return _header.Self;
            }
        }


        public void AddEntity(IGameEntity entity)
        {
            _entityId2Entity.Add(entity.EntityKey, entity);
            _cache.OnEntityAdded(entity);
        }

        public IGameEntity GetEntity(EntityKey entityKey)
        {
            return Get(entityKey);
        }

        private IGameEntity Get(EntityKey entityKey)
        {
            IGameEntity e;
            _entityId2Entity.TryGetValue(entityKey, out e);
            return e;
        }
        public IGameEntity GetOrCreate(EntityKey entityKey)
        {
#pragma warning disable RefCounter002
            IGameEntity e;
#pragma warning restore RefCounter002
            if (!_entityId2Entity.TryGetValue(entityKey, out e))
            {
                e = GameEntity.Allocate(entityKey);
                _entityId2Entity.Add(entityKey, e);
                e.ReleaseReference();

                _cache.OnEntityAdded(e);
            }
            return e;
        }


        public EntityMap EntityMap
        {
            get { return _entityId2Entity; }
        }

        public EntityMap SelfEntityMap
        {
            get
            {
                return _cache.SelfEntityMap;
            }
        }

       
        public EntityMap NonSelfEntityMap
        {
            get
            {
                return _cache.NonSelfEntityMap;
            }
        }

        public EntityMap CompensationEntityMap
        {
            get
            {
                
                return _cache.CompensationEntityMap;
            }
        }

        public EntityMap LatestEntityMap
        {
            get
            {
                
                return _cache.LatestEntityMap;
            }
        }

        public ICollection<IGameEntity> EntityList { get { return _entityId2Entity.Values; } }


        public void ForeachGameEntity(Action<IGameEntity> action)
        {
            foreach (var entity in _entityId2Entity.Values)
            {
                action(entity);
            }
        }

        public void RemoveEntity(EntityKey key)
        {
            _entityId2Entity.Remove(key);
            _cache.OnEntityRemoved(key);
        }

		protected override void OnCleanUp()
		{
		    CleanUp();

		    ObjectAllocatorHolder<Snapshot>.Free(this);
		}

        protected void CleanUp()
        {
            _entityId2Entity.ReleaseReference();
            _entityId2Entity = null;


            _cache.InvalidCache();
            EntityMapCache.Free(_cache);
            _cache = null;
        }


        protected override void OnReInit()
        {
			_entityId2Entity = EntityMap.Allocate();
            _cache = EntityMapCache.Allocate(_entityId2Entity);
            SnapshotSeq = 0;
            ServerTime = 0;
            
        }

        public void CopyHead(ISnapshot srcSnapshot)
        {
            _header = srcSnapshot.Header;
        }
    }
}