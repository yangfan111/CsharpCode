using App.Shared.CommonResource.Base;
using Core.CommonResource;
using Core.ObjectPool;

namespace App.Shared.CommonResource
{
    public class RemoveAssetStatus : BaseRefCounter
    {
        public ICommonResourceActions Actions;
        public AssetStatus Status;

        public static RemoveAssetStatus Allocate(AssetStatus status, ICommonResourceActions actions)
        {
            var ret = ObjectAllocatorHolder<RemoveAssetStatus>.Allocate();
            ret.Actions = actions;
            ret.Status = status;
            return ret;
        }

        protected override void OnCleanUp()
        {
            if (Status != null) Status.ReleaseReference();

            Status = null;
            Actions = null;
        }

        public class CustomObjectFactory : CustomAbstractObjectFactory
        {
            public CustomObjectFactory() : base(typeof(CustomObjectFactory))
            {
            }

            public override object MakeObject()
            {
                return new RemoveAssetStatus();
            }
        }
    }
}