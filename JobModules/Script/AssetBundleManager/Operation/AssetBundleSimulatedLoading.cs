using System.Collections;
using AssetBundleManagement;

namespace AssetBundleManager.Operation
{
    class AssetBundleSimulatedLoading : AssetBundleLoading
    {
        public AssetBundleSimulatedLoading(string assetBundleName, string url)
            : base(assetBundleName, url)
        { }

        public override bool IsDone()
        {
            return true;
        }

        public override void Process()
        {
            Content = new LoadedAssetBundle(null);
        }

        public override IEnumerator Cancel()
        {
            return null;
        }
    }
}