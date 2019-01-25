using System.Collections.Generic;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.UpdateLatest
{
    public class UpdateLatestPacakge : BaseRefCounter
    {
        public static UpdateLatestPacakge Allocate()
        {
            return ObjectAllocatorHolder<UpdateLatestPacakge>.Allocate();
        }
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(UpdateLatestPacakge)){}
            public override object MakeObject()
            {
                return new UpdateLatestPacakge();
            }
        }
        
        public UpdateLatestHead Head = new UpdateLatestHead();
        public List<IUpdateComponent> UpdateComponents = new List<IUpdateComponent>();
       

        protected override void OnCleanUp()
        {
            Head.ReInit();
            foreach (var component in UpdateComponents)
            {
                GameComponentInfo.Instance.Free(component.GetComponentId(), component);
            }
            UpdateComponents.Clear();
            ObjectAllocatorHolder<UpdateLatestPacakge>.Free(this);
        }

        protected override void OnReInit()
        {
            UpdateComponents.Clear();
            Head.ReInit();
        }

        public void CopyUpdateComponentsFrom(List<IUpdateComponent> updateComponents)
        {
            foreach (var updateComponent in updateComponents)
            {
                IUpdateComponent c = (IUpdateComponent)GameComponentInfo.Instance.Allocate(updateComponent.GetComponentId());
                c.CopyFrom(updateComponent);
                UpdateComponents.Add(c);
            }
        }
    }
}