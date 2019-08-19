using System;
using System.Collections.Generic;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.Replicaton
{
    public class CloneSnapshot : Snapshot
    {
        public static CloneSnapshot Allocate()
        {
            return ObjectAllocatorHolder<CloneSnapshot>.Allocate();
        }

        protected override void OnCleanUp()
        {
            CleanUp();

            ObjectAllocatorHolder<CloneSnapshot>.Free(this);
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CloneSnapshot))
            {
            }

            public override object MakeObject()
            {
                return new CloneSnapshot();
            }
        }
    }
    /// <summary>
    /// EntityMapCacheGroup entityCacheGroup;
    /// EntityMap: _entityId2Entity
    /// SnapshotHeader Header
    /// </summary>
    [Serializable]
    public class Snapshot : BaseRefCounter, ISnapshot
    {
        private EntityMap entityId2Entity;

        [NonSerialized] private volatile EntityMapCacheGroup entityCacheGroup;

        private SnapshotHeader header;

        protected Snapshot()
        {
            header = new SnapshotHeader();
        }

        public SnapshotHeader Header
        {
            get { return header; }
            set
            {
                header.CopyFrom(value);
                entityCacheGroup.MyKey = header.Self;
            }
        }

        public int ServerTime
        {
            get { return header.ServerTime; }
            set { header.ServerTime = value; }
        }

        public int VehicleSimulationTime
        {
            get { return header.VehicleSimulationTime; }
            set { header.VehicleSimulationTime = value; }
        }


        public int SnapshotSeq
        {
            get { return header.SnapshotSeq; }
            set { header.SnapshotSeq = value; }
        }

        public int LastUserCmdSeq
        {
            get { return header.LastUserCmdSeq; }
            set { header.LastUserCmdSeq = value; }
        }

        public EntityKey Self
        {
            set
            {
                header.Self           = value;
                entityCacheGroup.MyKey = header.Self;
            }
            get { return header.Self; }
        }


        public void AddEntity(IGameEntity entity)
        {
            entityId2Entity.Add(entity.EntityKey, entity);
            entityCacheGroup.OnEntityAdded(entity);
        }

        public IGameEntity GetEntity(EntityKey entityKey)
        {
            return Get(entityKey);
        }

        public IGameEntity GetOrCreate(EntityKey entityKey)
        {
#pragma warning disable RefCounter002
            IGameEntity e;
#pragma warning restore RefCounter002
            if (!entityId2Entity.TryGetValue(entityKey, out e))
            {
                e = GameEntity.Allocate(entityKey);
                entityId2Entity.Add(entityKey, e);
                e.ReleaseReference();

                entityCacheGroup.OnEntityAdded(e);
            }

            return e;
        }


        public EntityMap EntityMap
        {
            get { return entityId2Entity; }
        }

        public EntityMap SelfEntityMap
        {
            get { return entityCacheGroup.SelfEntityMap; }
        }


        public EntityMap NonSelfEntityMap
        {
            get { return entityCacheGroup.NonSelfEntityMap; }
        }

        public EntityMap CompensationEntityMap
        {
            get { return entityCacheGroup.CompensationEntityMap; }
        }

        public EntityMap LatestEntityMap
        {
            get { return entityCacheGroup.LatestEntityMap; }
        }

        public ICollection<IGameEntity> EntityList
        {
            get { return entityId2Entity.Values; }
        }


        public void ForeachGameEntity(Action<IGameEntity> action)
        {
            foreach (var entity in entityId2Entity.Values)
            {
                action(entity);
            }
        }

        public void RemoveEntity(EntityKey key)
        {
            entityId2Entity.Remove(key);
            entityCacheGroup.OnEntityRemoved(key);
        }

        public static Snapshot Allocate()
        {
            return ObjectAllocatorHolder<Snapshot>.Allocate();
        }

        private IGameEntity Get(EntityKey entityKey)
        {
            IGameEntity e;
            entityId2Entity.TryGetValue(entityKey, out e);
            return e;
        }

        protected override void OnCleanUp()
        {
            CleanUp();

            ObjectAllocatorHolder<Snapshot>.Free(this);
        }

        protected void CleanUp()
        {
            entityId2Entity.ReleaseReference();
            entityId2Entity = null;


            entityCacheGroup.InvalidCache();
            EntityMapCacheGroup.Free(entityCacheGroup);
            entityCacheGroup = null;
        }


        protected override void OnReInit()
        {
            entityId2Entity = EntityMap.Allocate();
            entityCacheGroup = EntityMapCacheGroup.Allocate(entityId2Entity);
            SnapshotSeq      = 0;
            ServerTime       = 0;
        }

        public void CopyHead(ISnapshot srcSnapshot)
        {
            header = srcSnapshot.Header;
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(Snapshot))
            {
            }

            public override object MakeObject()
            {
                return new Snapshot();
            }
        }
    }
}