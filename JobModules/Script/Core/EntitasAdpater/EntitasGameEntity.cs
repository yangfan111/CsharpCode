using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Playback;
using Core.Prediction;
using Core.Replicaton;
using Core.Utils;
using Core.Utils.System46;
using Entitas;

namespace Core.EntityComponent
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public class ComponentIndex<TEntityType, TComponentType>
    {
        public const int UnInitialized = -100;
        public static int Index = UnInitialized;
    }

    [Serializable]
    public class EntitasGameEntity<TEntity> : BaseRefCounter, IGameEntity where TEntity : Entity
    {
        private readonly EntityComponentChanged entityOnOnComponentAddedCache;
        private readonly EntityComponentChanged entityOnOnComponentRemovedCache;
        private readonly EntityComponentReplaced entityOnOnComponentReplacedCache;
        private readonly EntityEvent entityOnOnDestroyEntityCache;
        private volatile int assetComponentListLock;

        private List<IAssetComponent> assetComponentsList = new List<IAssetComponent>();
        private volatile bool assetComponentsListDirty;
        private volatile MyDictionary<int, IGameComponent> compensationDict = new MyDictionary<int, IGameComponent>();
        private volatile bool compensationListDirty = true;

        private int compensationListLock;

        private List<IGameComponent> componentList = new List<IGameComponent>();
        private IGameComponent[] componentListCopy;

        private volatile bool componentListDirty = true;

        private int componentListLock;


        private TEntity entity;

        private IComponentTypeLookup lookup;
        private volatile IGameEntity nonSelfEntityCopy;
        private int nonSelfEntityCopyLock;

        private volatile int nonSelfSnapShotSeq = -1;
        private volatile MyDictionary<int, IGameComponent> playbacktDict = new MyDictionary<int, IGameComponent>();
        private volatile bool playbacktListDirty = true;

        private int playbacktListLock;
        private volatile IGameEntity selfEntityCopy;
        private int selfEntityCopyLock;

        private volatile int selfSnapShotSeq = -1;

        private volatile MyDictionary<int, IGameComponent> syncLatestDict = new MyDictionary<int, IGameComponent>();
        private volatile bool syncLatestListDirty = true;

        private int syncLatestListLock;

        List<IGameComponent> updateLatestComponents = new List<IGameComponent>();


        public EntitasGameEntity()
        {
            entityOnOnComponentAddedCache    = EntityOnOnComponentAdded;
            entityOnOnComponentRemovedCache  = EntityOnOnComponentRemoved;
            entityOnOnComponentReplacedCache = EntityOnOnComponentReplaced;
            entityOnOnDestroyEntityCache     = EntityOnOnDestroyEntity;
        }


        public int EntityId
        {
            get { return EntityKey.EntityId; }
        }

        public int EntityType
        {
            get { return EntityKey.EntityType; }
        }


        public EntityKey EntityKey
        {
            get
            {
                int index = lookup.EntityKeyComponentIndex;
                if (index >= 0 && entity.HasComponent(index))
                    return ((EntityKeyComponent) entity.GetComponent(index)).Value;
                throw new Exception(String.Format("entity type {0} don't support component type EntityKey",
                    typeof(TEntity)));
            }
        }

        public object RealEntity
        {
            get { return entity; }
        }

        public PositionComponent Position
        {
            get
            {
                int index = lookup.PositionComponentIndex;
                if (index >= 0 && entity.HasComponent(index))
                    return ((PositionComponent) entity.GetComponent(index));
                throw new Exception(String.Format("entity type {0} don't support component type PositionComponent",
                    typeof(TEntity)));
            }
        }

        public PositionFilterComponent PositionFilter
        {
            get
            {
                int index = lookup.FlagPositionFilterComponentIndex;
                if (index >= 0 && entity.HasComponent(index))
                    return ((PositionFilterComponent) entity.GetComponent(index));
                throw new Exception(String.Format(
                    "entity type {0} don't support component type PositionFilterComponent", typeof(TEntity)));
            }
        }

        public bool HasOwnerIdComponent
        {
            get
            {
                int index = lookup.OwnerIdComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public OwnerIdComponent OwnerIdComponent
        {
            get
            {
                int index = lookup.OwnerIdComponentIndex;
                if (index >= 0 && entity.HasComponent(index))
                    return ((OwnerIdComponent) entity.GetComponent(index));
                throw new Exception(String.Format("entity type {0} don't support component type OwnerIdComponent",
                    typeof(TEntity)));
            }
        }

        public bool HasFlagImmutabilityComponent
        {
            get
            {
                int index = lookup.FlagImmutabilityComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public FlagImmutabilityComponent FlagImmutabilityComponent
        {
            get
            {
                int index = lookup.FlagImmutabilityComponentIndex;
                if (index >= 0 && entity.HasComponent(index))
                    return ((FlagImmutabilityComponent) entity.GetComponent(index));
                throw new Exception(String.Format(
                    "entity type {0} don't support component type FlagImmutabilityComponent", typeof(TEntity)));
            }
        }

        public LifeTimeComponent LifeTimeComponent
        {
            get
            {
                int index = lookup.LifeTimeComponentIndex;
                if (index >= 0 && entity.HasComponent(index))
                    return ((LifeTimeComponent) entity.GetComponent(index));
                throw new Exception(String.Format("entity type {0} don't support component type LifeTimeComponent",
                    typeof(TEntity)));
            }
        }


        public bool HasPositionFilter
        {
            get
            {
                int index = lookup.FlagPositionFilterComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public bool IsCompensation
        {
            get
            {
                int index = lookup.FlagCompensationComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public bool IsDestroy
        {
            get
            {
                int index = lookup.FlagDestroyComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public bool IsSelf
        {
            get
            {
                int index = lookup.FlagSelfComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public bool IsSyncNonSelf
        {
            get
            {
                int index = lookup.FlagSyncNonSelfComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public bool IsSyncSelf
        {
            get
            {
                int index = lookup.FlagSyncSelfComponentIndex;
                return index >= 0 && entity.HasComponent(index);
            }
        }

        public T AddComponent<T>() where T : IGameComponent, new()
        {
            int index = lookup.GetComponentIndex<T>();
            if (index >= 0)
            {
                return DoAddComponent<T>(entity, index);
            }

            throw new Exception(String.Format("entity type {0} don't support component type {1}", typeof(TEntity),
                typeof(T)));
        }


        public IGameComponent AddComponent(int componentId)
        {
            int  index         = lookup.GetComponentIndex(componentId);
            Type componentType = lookup.GetComponentType(componentId);
            if (index >= 0)
            {
                IGameComponent component = (IGameComponent) entity.CreateComponent(index, componentType);
                entity.AddComponent(index, component);
                return component;
            }

            throw new Exception(String.Format("entity type {0} don't support component type {1}", typeof(TEntity),
                componentType));
        }

        public IGameComponent AddComponent(int componentId, IGameComponent copyValue)
        {
            int  index         = lookup.GetComponentIndex(componentId);
            Type componentType = lookup.GetComponentType(componentId);
            if (index >= 0)
            {
                IGameComponent component = (IGameComponent) entity.CreateComponent(index, componentType);
                ((ICloneableComponent) component)?.CopyFrom(copyValue);
                entity.AddComponent(index, component);
                return component;
            }

            throw new Exception(String.Format("entity type {0} don't support component type {1}", typeof(TEntity),
                componentType));
        }

        public bool HasComponent<T>() where T : IGameComponent
        {
            return GetComponent<T>() != null;
        }


        public IGameComponent GetComponent(int componentId)
        {
            var index = lookup.GetComponentIndex(componentId);
            return DoGetComponent(index);
        }

        public void RemoveComponent<T>() where T : IGameComponent
        {
            int index = lookup.GetComponentIndex<T>();
            if (index >= 0)
                entity.RemoveComponent(index);
        }

        public T GetComponent<T>() where T : IGameComponent
        {
            int index = lookup.GetComponentIndex<T>();
            return (T) DoGetComponent(index);
        }

        public void RemoveComponent(int componentType)
        {
            int index = lookup.GetComponentIndex(componentType);
            if (index >= 0)
                entity.RemoveComponent(index);
        }

        public ICollection<IGameComponent> ComponentList
        {
            get { return SortedComponentList; }
        }

        public List<IGameComponent> SortedComponentList
        {
            get
            {
                if (componentListDirty)
                {
                    try
                    {
                        SpinWait spin = new SpinWait();
                        while (Interlocked.Increment(ref componentListLock) != 1)
                        {
                            Interlocked.Decrement(ref componentListLock);
                            spin.SpinOnce();
                        }

                        if (componentListDirty)
                        {
                            componentListDirty = false;

                            if (componentList == null)
                                componentList = new List<IGameComponent>(16);
                            else
                                componentList.Clear();
                            int[] idxById = lookup.IndexByComponentId;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                    if (component != null && component is IGameComponent)
                                        componentList.Add((IGameComponent) component);
                                }
                            }

                            componentList.Sort(GameComponentIComparer.Instance);
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref componentListLock);
                    }
                }

                return componentList;
            }
        }

        public MyDictionary<int, IGameComponent> SyncLatestComponentDict
        {
            get
            {
                if (syncLatestListDirty)
                {
                    try
                    {
                        SpinWait spin = new SpinWait();
                        while (Interlocked.Increment(ref syncLatestListLock) != 1)
                        {
                            Interlocked.Decrement(ref syncLatestListLock);
                            spin.SpinOnce();
                        }

                        if (syncLatestListDirty)
                        {
                            if (syncLatestDict == null)
                                syncLatestDict = new MyDictionary<int, IGameComponent>();
                            syncLatestDict.Clear();
                            var idxById = lookup.SyncLatestIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                    if (component != null && component is ILatestComponent)
                                        syncLatestDict.Add(((IGameComponent) component).GetComponentId(),
                                            (IGameComponent) component);
                                }
                            }

                            syncLatestListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref syncLatestListLock);
                    }
                }

                return syncLatestDict;
            }
        }

        public MyDictionary<int, IGameComponent> PlayBackComponentDictionary
        {
            get
            {
                if (playbacktListDirty)
                {
                    try
                    {
                        SpinWait spin = new SpinWait();
                        while (Interlocked.Increment(ref playbacktListLock) != 1)
                        {
                            Interlocked.Decrement(ref playbacktListLock);
                            spin.SpinOnce();
                        }

                        if (playbacktListDirty)
                        {
                            if (playbacktDict == null)
                                playbacktDict = new MyDictionary<int, IGameComponent>(16);
                            playbacktDict.Clear();
                            var idxById = lookup.PlaybackIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                    if (component != null && component is IPlaybackComponent)
                                        playbacktDict.Add(((IGameComponent) component).GetComponentId(),
                                            (IGameComponent) component);
                                }
                            }

                            playbacktListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref playbacktListLock);
                    }
                }

                return playbacktDict;
            }
        }

        public MyDictionary<int, IGameComponent> SortedCompensationComponentDict
        {
            get
            {
                if (compensationListDirty)
                {
                    try
                    {
                        SpinWait spin = new SpinWait();
                        while (Interlocked.Increment(ref compensationListLock) != 1)
                        {
                            Interlocked.Decrement(ref compensationListLock);
                            spin.SpinOnce();
                        }

                        if (compensationListDirty)
                        {
                            if (compensationDict == null)
                                compensationDict = new MyDictionary<int, IGameComponent>(16);
                            compensationDict.Clear();
                            var idxById = lookup.CompensationIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                    if (component != null && component is ICompensationComponent)
                                        compensationDict.Add(((IGameComponent) component).GetComponentId(),
                                            (IGameComponent) component);
                                }
                            }

                            compensationListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref compensationListLock);
                    }
                }

                return compensationDict;
            }
        }


        public void MarkDestroy()
        {
            AddComponent<FlagDestroyComponent>();
        }

        public void Destroy()
        {
            entity.Destroy();
        }

        #region //self Entity
        public IGameEntity GetSelfEntityCopy(int snapShotSeq)
        {
            if (selfSnapShotSeq != snapShotSeq)
            {
                try
                {
                    SpinWait spin = new SpinWait();
                    while (Interlocked.Increment(ref selfEntityCopyLock) != 1)
                    {
                        Interlocked.Decrement(ref selfEntityCopyLock);
                        spin.SpinOnce();
                    }

                    if (selfSnapShotSeq != snapShotSeq)
                    {
                        if (selfEntityCopy != null)
                        {
                            RefCounterRecycler.Instance.ReleaseReference(selfEntityCopy);
                            selfEntityCopy = null;
                        }

                        selfEntityCopy = GameEntity.Allocate(EntityKey);
                        var idxById = lookup.SelfIndexs;
                        for (int i = 0; i < idxById.Length; i++)
                        {
                            int index = idxById[i];
                            if (index >= 0)
                            {
                                var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                if (component != null && component is IGameComponent)
                                {
                                    var c = component as IGameComponent;
                                    selfEntityCopy.AddComponent(c.GetComponentId(), c);
                                }
                            }
                        }
                    }

                    selfSnapShotSeq = snapShotSeq;
                }
                finally
                {
                    Interlocked.Decrement(ref selfEntityCopyLock);
                }

                return selfEntityCopy;
            }

            return selfEntityCopy;
        }
        

        public IGameEntity GetNonSelfEntityCopy(int snapShotSeq)
        {
            if (nonSelfSnapShotSeq != snapShotSeq)
            {
                try
                {
                    SpinWait spin = new SpinWait();
                    while (Interlocked.Increment(ref nonSelfEntityCopyLock) != 1)
                    {
                        Interlocked.Decrement(ref nonSelfEntityCopyLock);
                        spin.SpinOnce();
                    }

                    if (nonSelfSnapShotSeq != snapShotSeq)
                    {
                        if (nonSelfEntityCopy != null)
                        {
                            RefCounterRecycler.Instance.ReleaseReference(nonSelfEntityCopy);

                            nonSelfEntityCopy = null;
                        }

                        nonSelfEntityCopy = GameEntity.Allocate(EntityKey);
                        var idxById = lookup.NoSelfIndexs;
                        for (int i = 0; i < idxById.Length; i++)
                        {
                            int index = idxById[i];
                            if (index >= 0)
                            {
                                var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                if (component != null && component is IGameComponent)
                                {
                                    var c = component as IGameComponent;
                                    nonSelfEntityCopy.AddComponent(c.GetComponentId(), c);
                                }
                            }
                        }
                    }

                    nonSelfSnapShotSeq = snapShotSeq;
                }
                finally
                {
                    Interlocked.Decrement(ref nonSelfEntityCopyLock);
                }


                return nonSelfEntityCopy;
            }

            {
                return nonSelfEntityCopy;
            }
        }

        #endregion
       

        public List<IGameComponent> GetUpdateLatestComponents()
        {
            updateLatestComponents.Clear();
            var idxById = lookup.UpdateLatestIndexs;
            for (int i = 0; i < idxById.Length; i++)
            {
                int index = idxById[i];
                if (index >= 0)
                {
                    var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                    if (component != null && component is IGameComponent)
                    {
                        var c = component as IGameComponent;
                        updateLatestComponents.Add(c);
                    }
                }
            }

            return updateLatestComponents;
        }

        public List<IAssetComponent> AssetComponents
        {
            get
            {
                if (assetComponentsListDirty)
                {
                    try
                    {
                        SpinWait spin = new SpinWait();
                        while (Interlocked.Increment(ref assetComponentListLock) != 1)
                        {
                            Interlocked.Decrement(ref assetComponentListLock);
                            spin.SpinOnce();
                        }

                        if (assetComponentsListDirty)
                        {
                            assetComponentsListDirty = false;

                            if (assetComponentsList == null)
                                assetComponentsList = new List<IAssetComponent>(16);
                            assetComponentsList.Clear();
                            var idxById = lookup.AssetComponentIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = entity.HasComponent(index) ? entity.GetComponent(index) : null;
                                    if (component != null && component is IAssetComponent)
                                        assetComponentsList.Add((IAssetComponent) component);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref assetComponentListLock);
                    }
                }

                return assetComponentsList;
            }
        }

        public static EntitasGameEntity<TEntity> Allocate(TEntity entity, IComponentTypeLookup lookup)
        {
            var rc = ObjectAllocatorHolder<EntitasGameEntity<TEntity>>.Allocate();
            rc.Init(entity, lookup);
            return rc;
        }


        public void Init(TEntity entity, IComponentTypeLookup lookup)
        {
            this.entity = entity;
            this.lookup = lookup;
            entity.AddOnComponentAdded(entityOnOnComponentAddedCache);
            entity.AddOnComponentRemoved(entityOnOnComponentRemovedCache);
            entity.AddOnComponentReplaced(entityOnOnComponentReplacedCache);
            entity.AddOnDestroyEntity(entityOnOnDestroyEntityCache);
        }

        public void CleanUpEntiryCopy()
        {
            lock (this)
            {
                selfSnapShotSeq    = -1;
                nonSelfSnapShotSeq = -1;
                if (selfEntityCopy != null)
                {
                    selfEntityCopy.ReleaseReference();
                    selfEntityCopy = null;
                }

                if (nonSelfEntityCopy != null)
                {
                    nonSelfEntityCopy.ReleaseReference();
                    nonSelfEntityCopy = null;
                }
            }
        }

        protected override void OnCleanUp()
        {
            CleanUpEntiryCopy();
        }

        private void SetDirty()
        {
            componentListDirty       = true;
            playbacktListDirty       = true;
            syncLatestListDirty      = true;
            assetComponentsListDirty = true;
            compensationListDirty    = true;
        }

        private void EntityOnOnDestroyEntity(IEntity entity)
        {
            CleanUpEntiryCopy();
            SetDirty();
            entity.RemoveOnComponentAdded(entityOnOnComponentAddedCache);
            entity.RemoveOnComponentRemoved(entityOnOnComponentRemovedCache);
            entity.RemoveOnComponentReplaced(entityOnOnComponentReplacedCache);
            entity.RemoveOnDestroyEntity(entityOnOnDestroyEntityCache);
        }

        private void EntityOnOnComponentReplaced(IEntity entity, int index1, IComponent previousComponent,
                                                 IComponent newComponent)
        {
            SetDirty();
        }

        private void EntityOnOnComponentRemoved(IEntity entity, int index1, IComponent component1)
        {
            SetDirty();
        }

        private void EntityOnOnComponentAdded(IEntity entity, int index1, IComponent component1)
        {
            SetDirty();
        }

        public static T DoAddComponent<T>(TEntity entity, int index) where T : IGameComponent, new()
        {
            var component = entity.CreateComponent<T>(index);
            entity.AddComponent(index, component);
            return component;
        }

        private IGameComponent DoGetComponent(int index)
        {
            return DoGetComponent(entity, index);
        }

        public static IGameComponent DoGetComponent(TEntity e, int index)
        {
            if (index >= 0 && e.HasComponent(index))
                return e.GetComponent(index) as IGameComponent;
            return null;
        }
    }
}