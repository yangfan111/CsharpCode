using Core.ObjectPool;
using Utils.AssetManager;

namespace Core.CommonResource
{
    public class AssetStatus : BaseRefCounter
    {
        public class CustomObjectFactory : CustomAbstractObjectFactory
        {
            public CustomObjectFactory() : base(typeof(CustomObjectFactory))
            {
            }

            public override object MakeObject()
            {
                return new AssetStatus();
            }
        }

        public EAssetLoadStatus Status;
        public AssetInfo AssetInfo;
        public UnityObject Object;
        public int ResIndex;
        public int LastRequestTime;

        private AssetStatus()
        {
            Reset();
        }

        public void Reset()
        {
            Status = EAssetLoadStatus.None;
            AssetInfo = AssetInfo.EmptyInstance;
            Object = null;
            ResIndex = 0;
            LastRequestTime = 0;
        }

        protected override void OnCleanUp()
        {
            Reset();
        }
    }
}