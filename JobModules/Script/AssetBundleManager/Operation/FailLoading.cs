using AssetBundleManagement;

namespace AssetBundleManager.Operation
{
    class FailLoading : AssetLoading
    {
        public FailLoading(bool isSceneLoading, string bundleName, string assetName):
            base(AssetLoadingPattern.Unkown, bundleName, assetName)
        {
            AssetGroup = isSceneLoading ? AssetGroup.Scene : AssetGroup.Asset;
        }

        public override bool IsDone()
        {
            return true;
        }

        public override void Process()
        {
            
        }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            
        }
    }
}