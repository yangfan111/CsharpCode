using AssetBundleManagement;

namespace AssetBundleManager.Operation
{
    class AssetSyncLoading : AssetLoading
    {

        private bool _isDone =  false;
        public AssetSyncLoading(string bundleName, string assetName)
            : base(AssetLoadingPattern.Sync, bundleName, assetName)
        { }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            LoadedAsset = assetBundle.Bundle.LoadAsset(Name);
            _isDone = true;
        }

        public override bool IsDone()
        {
            return _isDone;
        }

        public override void Process()
        { }
    }
}