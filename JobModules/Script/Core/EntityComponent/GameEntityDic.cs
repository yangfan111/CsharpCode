using System;
using System.Collections.Generic;
using System.Threading;
using Core.Compensation;
using Core.Components;
using Core.ObjectPool;
using Core.Playback;
using Core.Prediction;
using Core.SyncLatest;
using Core.Utils;
using Core.Utils.System46;

namespace Core.EntityComponent
{
    public abstract class GameEntityBase : BaseRefCounter, IGameEntity
    {
        protected IGameComponentInfo GameCompoentInfo { get; set; }

        public int EntityId
        {
            get { return EntityKey.EntityId; }
        }

        public int EntityType
        {
            get { return EntityKey.EntityType; }
        }

        public EntityKey EntityKey { get; set; }

        public T AddComponent<T>() where T : IGameComponent, new()
        {
            return (T) AddComponent(ComponentInfo<T>.ComponentId);
        }

        public abstract IGameComponent AddComponent(int componentId);
        public abstract IGameComponent AddComponent(int componentId, IGameComponent copyValue);
        public abstract IGameComponent GetComponent(int componentId);
        public abstract void RemoveComponent(int componentId);
        public abstract ICollection<IGameComponent> ComponentList { get; }


        public abstract List<IGameComponent> SortedComponentList { get; }
        public abstract MyDictionary<int, IGameComponent> SyncLatestComponentDictionary { get; }
        public abstract MyDictionary<int, IGameComponent> PlayBackComponentDictionary { get; }
        public abstract MyDictionary<int, IGameComponent> SortedCompensationComponentList { get; }

        public void Destroy()
        {
        }

        public void MarkDestroy()
        {
        }

        public object RealEntity
        {
            get { return null; }
        }

        public PositionComponent Position
        {
            get { return GetComponent<PositionComponent>(); }
        }


        public bool IsCompensation
        {
            get { return HasComponent<FlagCompensationComponent>(); }
        }

        public bool IsDestroy
        {
            get { return HasComponent<FlagDestroyComponent>(); }
        }

        public bool IsSelf
        {
            get { return HasComponent<FlagSelfComponent>(); }
        }

        public bool IsSyncNonSelf
        {
            get { return HasComponent<FlagSyncNonSelfComponent>(); }
        }

        public bool IsSyncSelf
        {
            get { return HasComponent<FlagSyncSelfComponent>(); }
        }

        public List<IAssetComponent> AssetComponents { get; private set; }
        
        public LifeTimeComponent LifeTimeComponent
        {
            get { return GetComponent<LifeTimeComponent>(); }
        }
        public bool HasPositionFilter
        {
            get { return HasComponent<PositionFilterComponent>(); }
        }

        public PositionFilterComponent PositionFilter
        {
            get { return GetComponent<PositionFilterComponent>(); }
        }

        public bool HasOwnerIdComponent
        {
            get { return HasComponent<OwnerIdComponent>(); }
        }

        public OwnerIdComponent OwnerIdComponent
        {
            get { return GetComponent<OwnerIdComponent>(); }
        }

        public bool HasFlagImmutabilityComponent
        {
            get { return HasComponent<FlagImmutabilityComponent>(); }
        }

        public FlagImmutabilityComponent FlagImmutabilityComponent
        {
            get { return GetComponent<FlagImmutabilityComponent>(); }
        }

        public IGameEntity GetNonSelfEntityCopy(int snapShotSeq)
        {
            throw new NotImplementedException();
        }

        public IGameEntity GetSelfEntityCopy(int snapShotSeq)
        {
            throw new NotImplementedException();
        }

        public List<IGameComponent> GetUpdateLatestComponents()
        {
            throw new NotImplementedException();
        }


        protected GameEntityBase()
        {
            GameCompoentInfo = GameComponentInfo.Instance;
        }

        public void RemoveComponent<T>() where T : IGameComponent
        {
            RemoveComponent(ComponentInfo<T>.ComponentId);
        }

        public bool HasComponent<T>() where T : IGameComponent
        {
            return GetComponent<T>() != null;
        }


        public T GetComponent<T>() where T : IGameComponent
        {
            return (T) GetComponent(ComponentInfo<T>.ComponentId);
        }
    }

    [Serializable]
    //为了方便查泄漏使用三个之类分别缓存
    public class CompensationGameEntity : GameEntity
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CompensationGameEntity))
            {
            }

            public override object MakeObject()
            {
                return new CompensationGameEntity();
            }
        }

        public static CompensationGameEntity Allocate(EntityKey entityKey)
        {
            var rc = ObjectAllocatorHolder<CompensationGameEntity>.Allocate();
            rc.EntityKey = entityKey;
            return rc;
        }

        protected override void OnCleanUp()
        {
            foreach (var component in _components.Values)
            {
                GameCompoentInfo.Free(component.GetComponentId(), component);
            }

            _components.Clear();
            ObjectAllocatorHolder<CompensationGameEntity>.Free(this);
        }
    }

    [Serializable]
    //为了方便查泄漏使用三个之类分别缓存
    public class CompensatioSnapshotGameEntity : GameEntity
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CompensatioSnapshotGameEntity))
            {
            }

            public override object MakeObject()
            {
                return new CompensatioSnapshotGameEntity();
            }
        }

        public static CompensatioSnapshotGameEntity Allocate(EntityKey entityKey)
        {
            var rc = ObjectAllocatorHolder<CompensatioSnapshotGameEntity>.Allocate();
            rc.EntityKey = entityKey;
            return rc;
        }

        protected override void OnCleanUp()
        {
            foreach (var component in _components.Values)
            {
                GameCompoentInfo.Free(component.GetComponentId(), component);
            }

            _components.Clear();
            ObjectAllocatorHolder<CompensatioSnapshotGameEntity>.Free(this);
        }
    }

    [Serializable]
    public class GameEntity : GameEntityBase
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(GameEntity))
            {
            }

            public override object MakeObject()
            {
                return new GameEntity();
            }
        }

        protected MyDictionary<int, IGameComponent> _components = new MyDictionary<int, IGameComponent>(16);
        private volatile List<IGameComponent> _list = new List<IGameComponent>();
        private volatile bool _componentListDirty = true;
        private volatile MyDictionary<int, IGameComponent> _syncLatestList = new MyDictionary<int, IGameComponent>();
        private volatile bool _syncLatestListDirty = true;
        private volatile MyDictionary<int, IGameComponent> _playbacktList = new MyDictionary<int, IGameComponent>();
        private volatile bool _playbacktListDirty = true;
        private volatile MyDictionary<int, IGameComponent> _compensationList = new MyDictionary<int, IGameComponent>();
        private volatile bool _compensationListDirty = true;



        public static GameEntity Allocate(EntityKey entityKey)
        {
            var rc = ObjectAllocatorHolder<GameEntity>.Allocate();
            rc.EntityKey = entityKey;
            return rc;
        }

        protected GameEntity()
        {
        }

        protected GameEntity(EntityKey entityKey)
        {
            EntityKey = entityKey;
        }

        public override IGameComponent AddComponent(int componentId)
        {
            var component = GameCompoentInfo.Allocate(componentId);
            SetComponent(componentId, component);
            return component;
        }

        public override IGameComponent AddComponent(int componentId, IGameComponent copyValue)
        {
            var component = GameCompoentInfo.Allocate(componentId);
            if (component is ICloneableComponent && copyValue is ICloneableComponent)
            {
                ((ICloneableComponent) component).CopyFrom(copyValue);
            }

            SetComponent(componentId, component);
            return component;
        }

        protected override void OnCleanUp()
        {
            foreach (var component in _components.Values)
            {
                GameCompoentInfo.Free(component.GetComponentId(), component);
            }

            _components.Clear();
            _list.Clear();
            ObjectAllocatorHolder<GameEntity>.Free(this);
        }

        protected int ComponentsLength()
        {
            return _components.Count;
        }

        protected void SetComponent(int id, IGameComponent component)
        {
            if (component != null)
            {
                AssertUtility.Assert(!_components.ContainsKey(id));
                AssertUtility.Assert(component.GetComponentId() == id);
            }

            SetDirty();
            _components[id] = component;
        }

        private void SetDirty()
        {
            _componentListDirty = true;
            _playbacktListDirty = true;
            _syncLatestListDirty = true;
        }

        public override IGameComponent GetComponent(int componentId)
        {
            IGameComponent rc;
            _components.TryGetValue(componentId, out rc);
            return rc;
        }


        protected override void OnReInit()
        {
            SetDirty();
        }


        public override void RemoveComponent(int componentId)
        {
            var comp = GetComponent(componentId);
            if (comp != null)
            {
                GameCompoentInfo.Free(comp);
                _components.Remove(componentId);
            }
        }

        private int _lockNum = 0;

        public override MyDictionary<int, IGameComponent> SyncLatestComponentDictionary
        {
            get
            {
                if (_syncLatestListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _lockNum) != 1)
                        {
                            Interlocked.Decrement(ref _lockNum);
                            spin.SpinOnce();
                        }

                        if (_syncLatestListDirty)
                        {
                            _syncLatestList.Clear();
                            foreach (var component in _components.Values)
                            {
                                if (component is ILatestComponent)
                                    _syncLatestList.Add(component.GetComponentId(), component);
                            }


                            _syncLatestListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _lockNum);
                    }
                }

                return _syncLatestList;
            }
        }


        public override MyDictionary<int, IGameComponent> PlayBackComponentDictionary
        {
            get
            {
                if (_playbacktListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _lockNum) != 1)
                        {
                            Interlocked.Decrement(ref _lockNum);
                            spin.SpinOnce();
                        }

                        if (_playbacktListDirty)
                        {
                            _playbacktList.Clear();
                            foreach (var component in _components.Values)
                            {
                                if (component is IPlaybackComponent)
                                    _playbacktList.Add(component.GetComponentId(), component);
                            }


                            _playbacktListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _lockNum);
                    }
                }

                return _playbacktList;
            }
        }


        public override MyDictionary<int, IGameComponent> SortedCompensationComponentList
        {
            get
            {
                if (_compensationListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _lockNum) != 1)
                        {
                            Interlocked.Decrement(ref _lockNum);
                            spin.SpinOnce();
                        }

                        if (_compensationListDirty)
                        {
                            _compensationList.Clear();

                            foreach (var component in _components.Values)
                            {
                                if (component is ICompensationComponent)
                                    _compensationList.Add(component.GetComponentId(), component);
                            }


                            _compensationListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _lockNum);
                    }
                }

                return _compensationList;
            }
        }

        public override List<IGameComponent> SortedComponentList
        {
            get
            {
                if (_componentListDirty)
                {
                    try
                    {
                        Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                        while (Interlocked.Increment(ref _lockNum) != 1)
                        {
                            Interlocked.Decrement(ref _lockNum);
                            spin.SpinOnce();
                        }

                        if (_componentListDirty)
                        {
                            _list.Clear();
                            foreach (var component in _components.Values)
                            {
                                _list.Add(component);
                            }

                            _list.Sort(GameComponentIComparer.Instance);
                            _componentListDirty = false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _lockNum);
                    }
                }

                return _list;
            }
        }

        public override ICollection<IGameComponent> ComponentList
        {
            get { return _components.Values; }
        }
    }

    public class GameComponentIComparer : IComparer<IGameComponent>
    {
        public static GameComponentIComparer Instance = new GameComponentIComparer();

        public int Compare(IGameComponent x, IGameComponent y)
        {
            return x.GetComponentId() - y.GetComponentId();
        }
    }

    public class FakeGameComponent : IGameComponent
    {
        private int _componentId;

        public FakeGameComponent()
        {
            _componentId = (int) ECoreComponentIds.Fake;
        }

        public int ComponentId
        {
            get { return _componentId; }
            set { _componentId = value; }
        }

        public FakeGameComponent(int componentId)
        {
            _componentId = componentId;
        }

        public int GetComponentId()
        {
            return _componentId;
        }
    }
}