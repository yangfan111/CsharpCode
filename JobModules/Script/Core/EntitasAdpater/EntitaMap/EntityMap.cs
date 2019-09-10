using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.ObjectPool;
using Core.Utils.System46;
using Entitas.Utils;

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

    public class PlayBackEntityMap : EntityMap
    {
        public static PlayBackEntityMap Allocate(bool ownEntity = true)
        {
            PlayBackEntityMap rc = ObjectAllocatorHolder<PlayBackEntityMap>.Allocate();
            rc.ownEntity  = ownEntity;
            rc.arrayDirty = true;
            return rc;
        }

        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<PlayBackEntityMap>.Free(this);
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
    }

    public class PredictionEntityMap : EntityMap
    {
        public static PredictionEntityMap Allocate(bool ownEntity = true)
        {
            PredictionEntityMap rc = ObjectAllocatorHolder<PredictionEntityMap>.Allocate();
            rc.ownEntity  = ownEntity;
            rc.arrayDirty = true;
            return rc;
        }

        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<PredictionEntityMap>.Free(this);
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
    }

    public class SyncLatestEntityMap : EntityMap
    {
        public static SyncLatestEntityMap Allocate(bool ownEntity = true)
        {
            SyncLatestEntityMap rc = ObjectAllocatorHolder<SyncLatestEntityMap>.Allocate();
            rc.ownEntity  = ownEntity;
            rc.arrayDirty = true;
            return rc;
        }

        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<SyncLatestEntityMap>.Free(this);
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
    }

    public class CacheEntityMap : EntityMap
    {
        public static CacheEntityMap Allocate(bool ownEntity = true)
        {
            CacheEntityMap rc = ObjectAllocatorHolder<CacheEntityMap>.Allocate();
            rc.ownEntity  = ownEntity;
            rc.arrayDirty = true;
            return rc;
        }

        protected override void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<CacheEntityMap>.Free(this);
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
    }

    #endregion

    [Serializable]
    public class EntityMap : IRefCounter, IEnumerable<KeyValuePair<EntityKey, IGameEntity>>
    {
        private IGameEntity[] array;
        protected bool arrayDirty;
        private MyDictionary<EntityKey, IGameEntity> entities;
        ///是否Acuiqe/ReleaseReference
        protected bool ownEntity;
        private RefCounter refCounter;
        protected StringBuilder stringBuilder = new StringBuilder();


        protected EntityMap()
        {
            entities   = new MyDictionary<EntityKey, IGameEntity>(1024, new EntityKeyComparer());
            refCounter = new RefCounter(OnCleanUp, this);
        }

        public MyDictionary<EntityKey, IGameEntity>.ValueCollection Values
        {
            get { return entities.Values; }
        }

        public int Count
        {
            get { return entities.Count; }
        }

        IEnumerator<KeyValuePair<EntityKey, IGameEntity>> IEnumerable<KeyValuePair<EntityKey, IGameEntity>>.
                        GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void AcquireReference()
        {
            refCounter.AcquireReference();
        }

        public void ReleaseReference()
        {
            refCounter.ReleaseReference();
        }

        public int RefCount
        {
            get { return refCounter.RefCount; }
        }

        public void ReInit()
        {
            refCounter.ReInit();
        }

        public override string ToString()
        {
            stringBuilder.Length = 0;
            
            foreach (var value in entities.Keys)
            {
                stringBuilder.Append("&");
                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }

        public static EntityMap Allocate(bool ownEntity = true)
        {
            EntityMap rc = ObjectAllocatorHolder<EntityMap>.Allocate();
            rc.ownEntity  = ownEntity;
            rc.arrayDirty = true;
            return rc;
        }

        protected void CleanUpAllEntity()
        {
            if (ownEntity)
            {
                foreach (var entity in entities.Values)
                {
                    entity.ReleaseReference();
                }
            }

            arrayDirty = true;
            entities.Clear();
        }

        protected virtual void OnCleanUp()
        {
            CleanUpAllEntity();
            ObjectAllocatorHolder<EntityMap>.Free(this);
        }

        public bool ContainsKey(EntityKey entityKey)
        {
            return entities.ContainsKey(entityKey);
        }

        public MyDictionary<EntityKey, IGameEntity>.Enumerator GetEnumerator()
        {
            return entities.GetEnumerator();
        }


        public bool TryGetValue(EntityKey entityKey, out IGameEntity leftEntity)
        {
            return entities.TryGetValue(entityKey, out leftEntity);
        }

        public void Add(EntityKey entityEntityKey, IGameEntity entity)
        {
            Remove(entityEntityKey);
            if (ownEntity)
                entity.AcquireReference();
            entities.Add(entityEntityKey, entity);
            arrayDirty = true;
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
            if (entities.TryGetValue(entityKey, out eEntity))
            {
                if (ownEntity)
                    eEntity.ReleaseReference();
                entities.Remove(entityKey);
                arrayDirty = true;
            }
        }

        public IGameEntity[] ToArray()
        {
            if (arrayDirty)
            {
                array      = entities.Values.ToArray();
                arrayDirty = false;
            }

            return array;
        }

    }
}