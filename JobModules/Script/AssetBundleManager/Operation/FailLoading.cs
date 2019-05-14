using AssetBundleManagement;
using System;

namespace AssetBundleManager.Operation
{
    class FailLoading : AssetLoading
    {
        public FailLoading(AssetLoadingPattern LoadingPattern,bool isSceneLoading, string bundleName, string assetName, Type objectType):
            base(LoadingPattern, bundleName, assetName, objectType)
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