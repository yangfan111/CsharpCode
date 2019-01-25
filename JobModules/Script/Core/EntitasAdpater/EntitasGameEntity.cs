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
using Core.Prediction.UserPrediction;
using Core.Replicaton;
using Core.SyncLatest;
using Core.Utils.System46;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Entitas.Utils;

namespace Core.EntitasAdpater
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
        public static EntitasGameEntity<TEntity> Allocate(TEntity entity, IComponentTypeLookup lookup)
        {
            var rc = ObjectAllocatorHolder<EntitasGameEntity<TEntity>>.Allocate();
            rc.Init(entity, lookup);
            return rc;
        }


        public EntitasGameEntity()
        {
            _entityOnOnComponentAddedCache = EntityOnOnComponentAdded;
            _entityOnOnComponentRemovedCache = EntityOnOnComponentRemoved;
            _entityOnOnComponentReplacedCache = EntityOnOnComponentReplaced;
            _entityOnOnDestroyEntityCache = EntityOnOnDestroyEntity;
        }

        private readonly EntityComponentChanged _entityOnOnComponentAddedCache;
        private readonly EntityComponentChanged _entityOnOnComponentRemovedCache;
        private readonly EntityComponentReplaced _entityOnOnComponentReplacedCache;
        private readonly EntityEvent _entityOnOnDestroyEntityCache;


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
                int index = _lookup.EntityKeyComponentIndex;
                if (index >= 0 && _entity.HasComponent(index))
                    return ((EntityKeyComponent) _entity.GetComponent(index)).Value;
                throw new Exception(String.Format("entity type {0} don't support component type EntityKey",
                    typeof(TEntity)));
            }
        }


        public object RealEntity
        {
            get { return _entity; }
        }

        public PositionComponent Position
        {
            get
            {
                int index = _lookup.PositionComponentIndex;
                if (index >= 0 && _entity.HasComponent(index))
                    return ((PositionComponent) _entity.GetComponent(index));
                throw new Exception(String.Format("entity type {0} don't support component type PositionComponent",
                    typeof(TEntity)));
            }
        }

        public PositionFilterComponent PositionFilter
        {
            get
            {
                int index = _lookup.FlagPositionFilterComponentIndex;
                if (index >= 0 && _entity.HasComponent(index))
                    return ((PositionFilterComponent) _entity.GetComponent(index));
                throw new Exception(String.Format(
                    "entity type {0} don't support component type PositionFilterComponent",
                    typeof(TEntity)));
            }
        }

        public bool HasOwnerIdComponent
        {
            get
            {
                int index = _lookup.OwnerIdComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public OwnerIdComponent OwnerIdComponent
        {
            get
            {
                int index = _lookup.OwnerIdComponentIndex;
                if (index >= 0 && _entity.HasComponent(index))
                    return ((OwnerIdComponent) _entity.GetComponent(index));
                throw new Exception(String.Format(
                    "entity type {0} don't support component type OwnerIdComponent",
                    typeof(TEntity)));
            }
        }

        public bool HasFlagImmutabilityComponent
        {
            get
            {
                int index = _lookup.FlagImmutabilityComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public FlagImmutabilityComponent FlagImmutabilityComponent
        {
            get
            {
                int index = _lookup.FlagImmutabilityComponentIndex;
                if (index >= 0 && _entity.HasComponent(index))
                    return ((FlagImmutabilityComponent) _entity.GetComponent(index));
                throw new Exception(String.Format(
                    "entity type {0} don't support component type FlagImmutabilityComponent",
                    typeof(TEntity)));
            }
        }

        public LifeTimeComponent LifeTimeComponent {
            get
            {
                int index = _lookup.LifeTimeComponentIndex;
                if (index >= 0 && _entity.HasComponent(index))
                    return ((LifeTimeComponent) _entity.GetComponent(index));
                throw new Exception(String.Format(
                    "entity type {0} don't support component type LifeTimeComponent",
                    typeof(TEntity)));
            }
        }


        public bool HasPositionFilter
        {
            get
            {
                int index = _lookup.FlagPositionFilterComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public bool IsCompensation
        {
            get
            {
                int index = _lookup.FlagCompensationComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public bool IsDestroy
        {
            get
            {
                int index = _lookup.FlagDestroyComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public bool IsSelf
        {
            get
            {
                int index = _lookup.FlagSelfComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public bool IsSyncNonSelf
        {
            get
            {
                int index = _lookup.FlagSyncNonSelfComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

        public bool IsSyncSelf
        {
            get
            {
                int index = _lookup.FlagSyncSelfComponentIndex;
                return index >= 0 && _entity.HasComponent(index);
            }
        }

      


        private TEntity _entity;

        private IComponentTypeLookup _lookup;

        private volatile bool _componentListDirty = true;


        public void Init(TEntity entity, IComponentTypeLookup lookup)
        {
            _entity = entity;
            _lookup = lookup;
            _entity.AddOnComponentAdded(_entityOnOnComponentAddedCache);
            _entity.AddOnComponentRemoved(_entityOnOnComponentRemovedCache);
            _entity.AddOnComponentReplaced(_entityOnOnComponentReplacedCache);
            _entity.AddOnDestroyEntity(_entityOnOnDestroyEntityCache);
        }

        public void CleanUpEntiryCopy()
        {
            lock (this)
            {
                _selfSnapShotSeq = -1;
                _nonSelfSnapShotSeq = -1;
                if (_selfEntityCopy != null)
                {
                    _selfEntityCopy.ReleaseReference();
                    _selfEntityCopy = null;
                }

                if (_nonSelfEntityCopy != null)
                {
                    _nonSelfEntityCopy.ReleaseReference();
                    _nonSelfEntityCopy = null;
                }
            }
        }

        protected override void OnCleanUp()
        {
            CleanUpEntiryCopy();
        }

        private void SetDirty()
        {
            _componentListDirty = true;
            _playbacktListDirty = true;
            _syncLatestListDirty = true;
            _assetComponentsListDirty = true;
            _compensationListDirty = true;
        }

        private void EntityOnOnDestroyEntity(IEntity entity)
        {
            CleanUpEntiryCopy();
            SetDirty();
            _entity.RemoveOnComponentAdded(_entityOnOnComponentAddedCache);
            _entity.RemoveOnComponentRemoved(_entityOnOnComponentRemovedCache);
            _entity.RemoveOnComponentReplaced(_entityOnOnComponentReplacedCache);
            _entity.RemoveOnDestroyEntity(_entityOnOnDestroyEntityCache);
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

        public T AddComponent<T>() where T : IGameComponent, new()
        {
            int index = _lookup.GetComponentIndex<T>();
            if (index >= 0)
            {
                return DoAddComponent<T>(_entity, index);
            }

            throw new Exception(String.Format("entity type {0} don't support component type {1}", typeof(TEntity),
                typeof(T)));
        }

        public static T DoAddComponent<T>(TEntity entity, int index) where T : IGameComponent, new()
        {
            var component = entity.CreateComponent<T>(index);
            entity.AddComponent(index, component);
            return component;
        }


        public IGameComponent AddComponent(int componentId)
        {
            int index = _lookup.GetComponentIndex(componentId);
            Type componentType = _lookup.GetComponentType(componentId);
            if (index >= 0)
            {
                IGameComponent component = (IGameComponent) _entity.CreateComponent(index, componentType);
                _entity.AddComponent(index, component);
                return component;
            }

            throw new Exception(String.Format("entity type {0} don't support component type {1}", typeof(TEntity),
                componentType));
        }

        public IGameComponent AddComponent(int componentId, IGameComponent copyValue)
        {
            int index = _lookup.GetComponentIndex(componentId);
            Type componentType = _lookup.GetComponentType(componentId);
            if (index >= 0)
            {
                IGameComponent component = (IGameComponent) _entity.CreateComponent(index, componentType);
                ((ICloneableComponent) component).CopyFrom(copyValue);
                _entity.AddComponent(index, component);
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
            var index = _lookup.GetComponentIndex(componentId);
            return DoGetComponent(index);
        }

        private IGameComponent DoGetComponent(int index)
        {
            return DoGetComponent(_entity, index);
        }

        public static IGameComponent DoGetComponent(TEntity e, int index)
        {
            if (index >= 0 && e.HasComponent(index))
                return e.GetComponent(index) as IGameComponent;
            return null;
        }

        public void RemoveComponent<T>() where T : IGameComponent
        {
            int index = _lookup.GetComponentIndex<T>();
            if (index >= 0)
                _entity.RemoveComponent(index);
        }

        public T GetComponent<T>() where T : IGameComponent
        {
            int index = _lookup.GetComponentIndex<T>();
            return (T) DoGetComponent(index);
        }

        public void RemoveComponent(int componentType)
        {
            int index = _lookup.GetComponentIndex(componentType);
            if (index >= 0)
                _entity.RemoveComponent(index);
        }

        private List<IGameComponent> _componentList = new List<IGameComponent>();
        private IGameComponent[] _componentListCopy;

        public ICollection<IGameComponent> ComponentList
        {
            get { return SortedComponentList; }
        }

        private int _componentListLock;
        public List<IGameComponent> SortedComponentList
        {
            get
            {
                if (_componentListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _componentListLock) != 1)
                        {
                            Interlocked.Decrement(ref _componentListLock);
                            spin.SpinOnce();
                        }

                        if (_componentListDirty)
                        {
                            _componentListDirty = false;

                            if (_componentList == null)
                                _componentList = new List<IGameComponent>(16);
                            _componentList.Clear();
                            var idxById = _lookup.IndexByComponentId;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                    if (component != null && component is IGameComponent)
                                        _componentList.Add((IGameComponent) component);
                                }
                            }

                            _componentList.Sort(GameComponentIComparer.Instance);
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _componentListLock);
                    }
                }

                return _componentList;
            }
        }

        private volatile MyDictionary<int, IGameComponent> _syncLatestList = new MyDictionary<int, IGameComponent>();
        private volatile bool _syncLatestListDirty = true;
        private volatile MyDictionary<int, IGameComponent> _playbacktList = new MyDictionary<int, IGameComponent>();
        private volatile bool _playbacktListDirty = true;
        private volatile MyDictionary<int, IGameComponent> _compensationList = new MyDictionary<int, IGameComponent>();
        private volatile bool _compensationListDirty = true;

        private int _syncLatestListLock;

        public MyDictionary<int, IGameComponent> SyncLatestComponentDictionary
        {
            get
            {
                if (_syncLatestListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _syncLatestListLock) != 1)
                        {
                            Interlocked.Decrement(ref _syncLatestListLock);
                            spin.SpinOnce();
                        }

                        if (_syncLatestListDirty)
                        {
                            if (_syncLatestList == null)
                                _syncLatestList = new MyDictionary<int, IGameComponent>();
                            _syncLatestList.Clear();
                            var idxById = _lookup.SyncLatestIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                    if (component != null && component is ILatestComponent)
                                        _syncLatestList.Add(((IGameComponent) component).GetComponentId(),
                                            (IGameComponent) component);
                                }
                            }

                            _syncLatestListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _syncLatestListLock);
                    }
                }

                return _syncLatestList;
            }
        }

        private int _playbacktListLock;

        public MyDictionary<int, IGameComponent> PlayBackComponentDictionary
        {
            get
            {
                if (_playbacktListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _playbacktListLock) != 1)
                        {
                            Interlocked.Decrement(ref _playbacktListLock);
                            spin.SpinOnce();
                        }

                        if (_playbacktListDirty)
                        {
                            if (_playbacktList == null)
                                _playbacktList = new MyDictionary<int, IGameComponent>(16);
                            _playbacktList.Clear();
                            var idxById = _lookup.PlaybackIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                    if (component != null && component is IPlaybackComponent)
                                        _playbacktList.Add(((IGameComponent) component).GetComponentId(),
                                            (IGameComponent) component);
                                }
                            }

                            _playbacktListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _playbacktListLock);
                    }
                }

                return _playbacktList;
            }
        }

        private int _compensationListLock;

        public MyDictionary<int, IGameComponent> SortedCompensationComponentList
        {
            get
            {
                if (_compensationListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _compensationListLock) != 1)
                        {
                            Interlocked.Decrement(ref _compensationListLock);
                            spin.SpinOnce();
                        }

                        if (_compensationListDirty)
                        {
                            if (_compensationList == null)
                                _compensationList = new MyDictionary<int, IGameComponent>(16);
                            _compensationList.Clear();
                            var idxById = _lookup.CompensationIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                    if (component != null && component is ICompensationComponent)
                                        _compensationList.Add(((IGameComponent) component).GetComponentId(),
                                            (IGameComponent) component);
                                }
                            }

                            _compensationListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _compensationListLock);
                    }
                }

                return _compensationList;
            }
        }


        public void MarkDestroy()
        {
            AddComponent<FlagDestroyComponent>();
        }

        public void Destroy()
        {
            _entity.Destroy();
        }

        private volatile int _selfSnapShotSeq = -1;
        private volatile IGameEntity _selfEntityCopy;
        private int _selfEntityCopyLock;

        public IGameEntity GetSelfEntityCopy(int snapShotSeq)
        {
            if (_selfSnapShotSeq != snapShotSeq)
            {
                try
                {
                    Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                    while (Interlocked.Increment(ref _selfEntityCopyLock) != 1)
                    {
                        Interlocked.Decrement(ref _selfEntityCopyLock);
                        spin.SpinOnce();
                    }

                    if (_selfSnapShotSeq != snapShotSeq)
                    {
                        if (_selfEntityCopy != null)
                        {
                            RefCounterRecycler.Instance.ReleaseReference(_selfEntityCopy);
                            _selfEntityCopy = null;
                        }

                        _selfEntityCopy = GameEntity.Allocate(EntityKey);
                        var idxById = _lookup.SelfIndexs;
                        for (int i = 0; i < idxById.Length; i++)
                        {
                            int index = idxById[i];
                            if (index >= 0)
                            {
                                var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                if (component != null && component is IGameComponent)
                                {
                                    var c = component as IGameComponent;
                                    _selfEntityCopy.AddComponent(c.GetComponentId(), c);
                                }
                            }
                        }
                    }

                    _selfSnapShotSeq = snapShotSeq;
                }
                finally
                {
                    Interlocked.Decrement(ref _selfEntityCopyLock);
                }

                return _selfEntityCopy;
            }
            else
            {
                return _selfEntityCopy;
            }
        }

        List<IGameComponent> _updateLatestComponents = new List<IGameComponent>();

        public List<IGameComponent> GetUpdateLatestComponents()
        {
            _updateLatestComponents.Clear();
            var idxById = _lookup.UpdateLatestIndexs;
            for (int i = 0; i < idxById.Length; i++)
            {
                int index = idxById[i];
                if (index >= 0)
                {
                    var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                    if (component != null && component is IGameComponent)
                    {
                        var c = component as IGameComponent;
                        _updateLatestComponents.Add(c);
                    }
                }
            }

            return _updateLatestComponents;
        }

        private volatile int _nonSelfSnapShotSeq = -1;
        private volatile IGameEntity _nonSelfEntityCopy;
        private int _nonSelfEntityCopyLock = 0;
        private volatile bool _assetComponentsListDirty;
        private volatile int _assetComponentListLock = 0;
        public IGameEntity GetNonSelfEntityCopy(int snapShotSeq)
        {
            if (_nonSelfSnapShotSeq != snapShotSeq)
            {
                try
                {
                    Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                    while (Interlocked.Increment(ref _nonSelfEntityCopyLock) != 1)
                    {
                        Interlocked.Decrement(ref _nonSelfEntityCopyLock);
                        spin.SpinOnce();
                    }

                    if (_nonSelfSnapShotSeq != snapShotSeq)
                    {
                        if (_nonSelfEntityCopy != null)
                        {
                            RefCounterRecycler.Instance.ReleaseReference(_nonSelfEntityCopy);

                            _nonSelfEntityCopy = null;
                        }

                        _nonSelfEntityCopy = GameEntity.Allocate(EntityKey);
                        var idxById = _lookup.NoSelfIndexs;
                        for (int i = 0; i < idxById.Length; i++)
                        {
                            int index = idxById[i];
                            if (index >= 0)
                            {
                                var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                if (component != null && component is IGameComponent)
                                {
                                    var c = component as IGameComponent;
                                    _nonSelfEntityCopy.AddComponent(c.GetComponentId(), c);
                                }
                            }
                        }
                    }

                    _nonSelfSnapShotSeq = snapShotSeq;
                }
                finally
                {
                    Interlocked.Decrement(ref _nonSelfEntityCopyLock);
                }


                return _nonSelfEntityCopy;
            }

            {
                return _nonSelfEntityCopy;
            }
        }
        
        private List<IAssetComponent> _assetComponentsList = new List<IAssetComponent>();
        public List<IAssetComponent> AssetComponents
        {
           
            get
            {
                if (_assetComponentsListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _assetComponentListLock) != 1)
                        {
                            Interlocked.Decrement(ref _assetComponentListLock);
                            spin.SpinOnce();
                        }

                        if (_assetComponentsListDirty)
                        {
                            _assetComponentsListDirty = false;

                            if (_assetComponentsList == null)
                                _assetComponentsList = new List<IAssetComponent>(16);
                            _assetComponentsList.Clear();
                            var idxById = _lookup.AssetComponentIndexs;
                            for (int i = 0; i < idxById.Length; i++)
                            {
                                int index = idxById[i];
                                if (index >= 0)
                                {
                                    var component = _entity.HasComponent(index) ? _entity.GetComponent(index) : null;
                                    if (component != null && component is IAssetComponent)
                                        _assetComponentsList.Add((IAssetComponent) component);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _assetComponentListLock);
                    }
                }

                return _assetComponentsList;
            }
        }
    }
}