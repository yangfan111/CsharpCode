using System;
using System.Collections.Generic;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.Replicaton
{
    [Serializable]
    public class ListBasedSnapshot : BaseRefCounter, ISnapshot
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(ListBasedSnapshot)){}
            public override object MakeObject()
            {
                return new ListBasedSnapshot();
            }
        }
        public static ListBasedSnapshot Allocate()
        {
            return ObjectAllocatorHolder<ListBasedSnapshot>.Allocate();
        }

        private List<IGameEntity> _gameEntities = new List<IGameEntity>();

        private SnapshotHeader _header;

        protected ListBasedSnapshot()
        {
	        _header = new SnapshotHeader();
        }

        public SnapshotHeader Header
        {
            get { return _header; }
            set
            {
                _header.CopyFrom(value);
            }
        }

        public int ServerTime
        {
            get { return _header.ServerTime; }
            set { _header.ServerTime = value; }
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

        public int VehicleSimulationTime
        {
            get { return _header.VehicleSimulationTime; }
            set { _header.VehicleSimulationTime = value; }
        }

        public EntityKey Self
        {
            set
            {
                _header.Self = value;
            }
            get
            {
                return _header.Self;
            }
        }


        public void AddEntity(IGameEntity entity)
        {
            entity.AcquireReference();
            _gameEntities.Add(entity);
            
        }

        public IGameEntity GetEntity(EntityKey entityKey)
        {
            throw new NotImplementedException("not imple");
        }

      
        public IGameEntity GetOrCreate(EntityKey entityKey)
        {
            throw new NotImplementedException("not imple");
        }


        public EntityMap EntityMap
        {
            get { throw new NotImplementedException("not imple"); }
        }

        public EntityMap SelfEntityMap
        {
            get
            {
                throw new NotImplementedException("not imple");
            }
        }


        public EntityMap NonSelfEntityMap
        {
            get
            {
                throw new NotImplementedException("not imple");
            }
        }

        public EntityMap CompensationEntityMap
        {
            get
            {
                throw new NotImplementedException("not imple");
            }
        }

        public EntityMap LatestEntityMap
        {
            get
            {
                throw new NotImplementedException("not imple");
            }
        }

        public ICollection<IGameEntity> EntityList { get { return _gameEntities; }}


        public void ForeachGameEntity(Action<IGameEntity> action)
        {
            foreach (var entity in _gameEntities)
            {
                action(entity);
            }
        }

        public void RemoveEntity(EntityKey key)
        {
            throw new NotImplementedException("not imple");
        }

        private bool active;

        protected override void OnCleanUp()
        {
            active = true;
            foreach (var entity in _gameEntities)
            {
                entity.ReleaseReference();
            }

            _gameEntities.Clear();
            ObjectAllocatorHolder<ListBasedSnapshot>.Free(this);
        }



        protected override void OnReInit()
        {
            active = false;
            _gameEntities.Clear();
            SnapshotSeq = 0;
            ServerTime = 0;
            VehicleSimulationTime = 0;
        }

        public void CopyHead(ISnapshot srcSnapshot)
        {
            _header = srcSnapshot.Header;
        }
    }
}