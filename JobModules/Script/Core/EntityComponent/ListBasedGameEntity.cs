using System;
using System.Collections.Generic;
using Core.EntitasAdpater;
using Core.ObjectPool;
using Core.Prediction;
using Core.Utils.System46;

namespace Core.EntityComponent
{
    [Serializable]
    public class ListBasedGameEntity : GameEntityBase
    {
        private volatile List<IGameComponent> _list = new List<IGameComponent>();
        private volatile List<ComponentWrapper> _wrapperLists = new List<ComponentWrapper>();
        private volatile bool _componentListDirty = true;
        private List<IGameComponent> _sortedPlayBackCompensationList;


        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(ListBasedGameEntity)){}
            public override object MakeObject()
            {
                return new ListBasedGameEntity();
            }

        }

        public static ListBasedGameEntity Allocate(EntityKey entityKey)
        {
            var rc = ObjectAllocatorHolder<ListBasedGameEntity>.Allocate();
            rc.EntityKey = entityKey;
            return rc;
        }

        protected ListBasedGameEntity()
        {
        }

        protected ListBasedGameEntity(EntityKey entityKey)
        {
            EntityKey = entityKey;
        }


        public override IGameComponent AddComponent(int componentId, IGameComponent copyValue)
        {
            var component = GameCompoentInfo.Allocate(componentId);
            ((ICloneableComponent)component).CopyFrom(copyValue);
           
            _list.Add(component);
            var wrapper = ComponentWrapper.Allocate(component);
            _wrapperLists.Add(wrapper);
            _componentListDirty = true;
            return component;
        }
        public override IGameComponent AddComponent(int componentId)
        {
            var component = GameCompoentInfo.Allocate(componentId);
            _list.Add(component);
            var wrapper = ComponentWrapper.Allocate(component);
            _wrapperLists.Add(wrapper);
            _componentListDirty = true;
            return component;
        }

        protected override void OnCleanUp()
        {
            int count = _list.Count;
            for (var i = 0; i < count; i++)
            {
                //var component = _list[i];
                //GameCompoentInfo.Free(component.GetComponentId(), component);
                _wrapperLists[i].ReleaseReference();
            }
            _list.Clear();
            _wrapperLists.Clear();
            
            ObjectAllocatorHolder<ListBasedGameEntity>.Free(this);
        }

        protected override void OnReInit()
        {
            _list.Clear();
            _list.Clear();
            _wrapperLists.Clear();
            _componentListDirty = true;
        }

        protected int ComponentsLength()
        {
            return _list.Count;
        }

        public override IGameComponent GetComponent(int componentId)
        {
            var fake = ObjectAllocatorHolder<FakeGameComponent>.Allocate();
            fake.ComponentId = componentId;
            int idx = SortedComponentList.BinarySearch(fake, GameComponentIComparer.Instance);
            ObjectAllocatorHolder<FakeGameComponent>.Free(fake);
            if (idx >= 0)
            {
                return SortedComponentList[idx];
            }
            return null;
        }



        public override void RemoveComponent(int componentId)
        {
            int len = _list.Count;
            int index = -1;
            for (int i = 0; i < len; i++)
            {
                if (_list[i].GetComponentId() == componentId)
                {
                    index = i;
                    _wrapperLists[index].ReleaseReference();
                    //GameCompoentInfo.Free(_list[i]);
                    break;
                }
            }
           
            if (index >= 0)
            {
                _wrapperLists.RemoveAt(index);
                _list.RemoveAt(index);
            }

            _componentListDirty = true;
        }

      

        public override ICollection<IGameComponent> ComponentList
        {
            get
            {
                return _list;
            }
        }

        

        public override List<IGameComponent> SortedComponentList
        {
            get
            {
                if (_componentListDirty)
                {
                    lock (this)
                    {
                        if (_componentListDirty)
                        {
                            _list.Sort(GameComponentIComparer.Instance);
                            _wrapperLists.Sort(ComponentWrapperComparer.Instance);
                            _componentListDirty = false;
                        }
                    }
                }
                return _list;
            }
        }

        public List<ComponentWrapper> ComponentWapperList
        {
            get { return _wrapperLists; }
        }

        public override void AcquireReference()
        {
            base.AcquireReference();
            int count = _wrapperLists.Count;
            for (int i = 0; i < count; i++)
            {
                _wrapperLists[i].AcquireReference();
            }
        }

        public override void ReleaseReference()
        {
            base.ReleaseReference();
            int count = _wrapperLists.Count;
            for (int i = 0; i < count; i++)
            {
                _wrapperLists[i].ReleaseReference();
            }
        }
        public override MyDictionary<int, IGameComponent> SyncLatestComponentDictionary
        {
            get {  throw new NotImplementedException();}
        }

        public override MyDictionary<int,IGameComponent> PlayBackComponentDictionary
        {
            get {  throw new NotImplementedException();}
        }

        public override MyDictionary<int,IGameComponent> SortedCompensationComponentList
        {
            get {  throw new NotImplementedException();}
        }
    }
}