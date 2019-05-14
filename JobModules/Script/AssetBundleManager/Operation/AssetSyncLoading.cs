using System;
using AssetBundleManagement;

namespace AssetBundleManager.Operation
{
    class AssetSyncLoading : AssetLoading
    {
        private bool _isDone = false;

        public AssetSyncLoading(string bundleName, string assetName, Type objectType)
            : base(AssetLoadingPattern.Sync, bundleName, assetName, objectType)
        {
        }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            LoadedAsset = ObjectType == null
                ? assetBundle.Bundle.LoadAsset(Name)
                : assetBundle.Bundle.LoadAsset(Name, ObjectType);

            _isDone = true;
        }


        public override bool IsDone()
        {
            return _isDone;
        }

        public override void Process()
        {
        }
    }
}