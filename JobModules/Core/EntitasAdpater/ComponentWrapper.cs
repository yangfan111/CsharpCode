using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.EntitasAdpater
{
    public class ComponentWrapperComparer : IComparer<ComponentWrapper>
    {
        public static ComponentWrapperComparer Instance = new ComponentWrapperComparer();

        public int Compare(ComponentWrapper x, ComponentWrapper y)
        {
            return x.GetComponentId() - y.GetComponentId();
        }
    }
    public class ComponentWrapper : BaseRefCounter, IGameComponent, FakeComponent
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(ComponentWrapper)){}
            public override object MakeObject()
            {
                return new ComponentWrapper();
            }

        }
        public static ComponentWrapper Allocate(IGameComponent component)
        {
            var rc = ObjectAllocatorHolder<ComponentWrapper>.Allocate();
            rc._component = component;
            return rc;
        }
        public static ComponentWrapper Allocate(int componentId)
        {
            var rc = ObjectAllocatorHolder<ComponentWrapper>.Allocate();
            rc._component = _gameCompoentInfo.Allocate(componentId);
            return rc;
        }
        private static IGameComponentInfo _gameCompoentInfo = GameComponentInfo.Instance;
        private IGameComponent _component;
        public IGameComponent Component
        {
            get { return _component; }
        }

        public ComponentWrapper()
        {
            
        }

        protected override void OnReInit()
        {
            base.OnReInit();
            if (_component != null)
            {
                _gameCompoentInfo.Free(_component.GetComponentId(), _component);
            }
        }

        protected override void OnCleanUp()
        {
            _gameCompoentInfo.Free(_component.GetComponentId(), _component);
            _component = null;
            ObjectAllocatorHolder<ComponentWrapper>.Free(this);
        }

        public int GetComponentId()
        {
            return _component.GetComponentId();
        }
    }
}