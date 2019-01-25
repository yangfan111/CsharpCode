using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.ObjectPool;
using Core.Utils;
using Core.Utils.System46;

namespace Core.EntityComponent
{
    
    public interface IEntityMapFilter
    {
        bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType);
        bool IsIncludeEntity(IGameEntity entity);
    }

    public class DummyEntityMapFilter : IEntityMapFilter
    {
        public static DummyEntityMapFilter Instance = new DummyEntityMapFilter();
        public bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType)
        {
            return true;
        }

        public bool IsIncludeEntity(IGameEntity entity)
        {
            return true;
        }
    }

    #region ForOOMCheck
   
    public class PlayBackEntityMap:EntityMap{
        public static PlayBackEntityMap Allocate(bool ownEntity=true)
        {
            
            PlayBackEntityMap rc = ObjectAllocatorHolder<PlayBackEntityMap>.Allocate();
            rc._ownEntity = ownEntity;
            rc._arrayDirty = true;
            return rc;
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(PlayBackEntityMap))
            {
            }

            public override object MakeObject()
            {
                return new PlayBackEntityMap();
            }
        }
        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<PlayBackEntityMap>.Free(this);
        }
    }
    public class PredictionEntityMap:EntityMap{
        public static PredictionEntityMap Allocate(bool ownEntity=true)
        {
            
            PredictionEntityMap rc = ObjectAllocatorHolder<PredictionEntityMap>.Allocate();
            rc._ownEntity = ownEntity;
            rc._arrayDirty = true;
            return rc;
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(PredictionEntityMap))
            {
            }

            public override object MakeObject()
            {
                return new PredictionEntityMap();
            }
            
        }
        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<PredictionEntityMap>.Free(this);
        }
    } 
    public class SyncLatestEntityMap:EntityMap{
        public static SyncLatestEntityMap Allocate(bool ownEntity=true)
        {
            
            SyncLatestEntityMap rc = ObjectAllocatorHolder<SyncLatestEntityMap>.Allocate();
            rc._ownEntity = ownEntity;
            rc._arrayDirty = true;
            return rc;
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(SyncLatestEntityMap))
            {
            }

            public override object MakeObject()
            {
                return new SyncLatestEntityMap();
            }
        }
        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<SyncLatestEntityMap>.Free(this);
        }
    }
    public class CacheEntityMap:EntityMap{
        public static CacheEntityMap Allocate(bool ownEntity=true)
        {
            
            CacheEntityMap rc = ObjectAllocatorHolder<CacheEntityMap>.Allocate();
            rc._ownEntity = ownEntity;
            rc._arrayDirty = true;
            return rc;
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CacheEntityMap))
            {
            }

            public override object MakeObject()
            {
                return new CacheEntityMap();
            }
        }
        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<CacheEntityMap>.Free(this);
        }
    }
    #endregion
    [Serializable]
    public class EntityMap : IRefCounter, IEnumerable<KeyValuePair<EntityKey, IGameEntity>>
    {
        private RefCounter _refCounter;
        private MyDictionary<EntityKey, IGameEntity> _entities;
        public MyDictionary<EntityKey, IGameEntity>.ValueCollection Values { get { return _entities.Values; } }
        public int Count { get { return _entities.Count; } }
        protected bool _ownEntity;
        private IGameEntity[] _array;
        protected bool _arrayDirty;
        public static EntityMap Allocate(bool ownEntity=true)
        {
            
            EntityMap rc = ObjectAllocatorHolder<EntityMap>.Allocate();
            rc._ownEntity = ownEntity;
            rc._arrayDirty = true;
            return rc;
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(EntityMap))
            {
            }

            public override object MakeObject()
            {
                return new EntityMap();
            }
        }
        

        protected EntityMap()
        {
            _entities = new MyDictionary<EntityKey, IGameEntity>(1024, new EntityKeyComparer());
            _refCounter = new RefCounter(OnCleanUp, this);
        }
    

        public void AcquireReference()
        {
            _refCounter.AcquireReference();
        }

        public void ReleaseReference()
        {
            _refCounter.ReleaseReference();
        }

        public int RefCount
        {
            get { return _refCounter.RefCount; }
        }

        protected void CleanUpAllEntity()
        {
            if (_ownEntity)
            {
                foreach (var entity in _entities.Values)
                {
                    entity.ReleaseReference();
                }
            }

            _arrayDirty = true;
            _entities.Clear();
        }
        protected virtual void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<EntityMap>.Free(this);
        }

        public void ReInit()
        {
            _refCounter.ReInit();
        }

        public bool ContainsKey(EntityKey entityKey)
        {
            return _entities.ContainsKey(entityKey);
        }

        public MyDictionary<EntityKey, IGameEntity>.Enumerator GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator<KeyValuePair<EntityKey, IGameEntity>> IEnumerable<KeyValuePair<EntityKey, IGameEntity>>.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }


        public bool TryGetValue(EntityKey entityKey, out IGameEntity leftEntity)
        {
            return _entities.TryGetValue(entityKey, out leftEntity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(EntityKey entityEntityKey, IGameEntity entity)
        {
            Remove(entityEntityKey);
            if (_ownEntity)
                entity.AcquireReference();
            _entities.Add(entityEntityKey, entity);
            _arrayDirty = true;
        }

        public void AddAll(EntityMap right)
        {
           
            foreach (var entity in right.Values)
            {
                Add(entity.EntityKey, entity);
            }
        }

        public void Remove(EntityKey entityKey)
        {
#pragma warning disable RefCounter001
            IGameEntity eEntity;
#pragma warning restore RefCounter001
            if (_entities.TryGetValue(entityKey, out eEntity))
            {
                if (_ownEntity)
                    eEntity.ReleaseReference();
                _entities.Remove(entityKey);
                _arrayDirty = true;
            }
        }

        public IGameEntity[] ToArray()
        {
            if (_arrayDirty)
            {
                _array = _entities.Values.ToArray();
                _arrayDirty = false;
            }

            return _array;
        }
    }
}