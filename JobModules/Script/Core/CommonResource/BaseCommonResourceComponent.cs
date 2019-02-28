using Core.CommonResource;
using Core.ObjectPool;

namespace App.Shared.CommonResource
{
    public abstract class AbstractCommonResourceComponent : ICommonResourceComponent
    {
        private AssetStatus[] _resources;

        public abstract int GetComponentId();

        public AssetStatus[] Resources
        {
            get { return _resources; }
        }


        protected abstract int ResourceLength { get; }

        public void Reset()
        {
            if (_resources == null)
            {
                _resources = new AssetStatus[ResourceLength];
            }


            for (var i = 0; i < _resources.Length; i++)
            {
                if (_resources[i] == null)
                {
                    _resources[i] = ObjectAllocatorHolder<AssetStatus>.Allocate();
                }

                _resources[i].Reset();
            }
        }

       
    }
}