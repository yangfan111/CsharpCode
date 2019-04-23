using AssetBundleManagement;
using UnityEngine;

namespace AssetBundleManager.Operation
{
    abstract class AssetLoading : PollingOperation
    {
        public string BundleName { get; private set; }
        public string Name { get; private set; }

        public AssetLoadingPattern LoadingPattern { get; private set; }

        public Object LoadedAsset { get; protected set; }

        protected AssetGroup AssetGroup;
        public bool IsSceneLoading
        {
            get { return AssetGroup == AssetGroup.Scene; }
        }

        public virtual bool IsLoadFailed
        {
            get { return LoadedAsset == null; }
        }

        protected AssetLoading(AssetLoadingPattern loadingPattern, string bundleName, string assetName)
        {
            LoadingPattern = loadingPattern;
            BundleName = bundleName;
            Name = assetName;
            AssetGroup = AssetGroup.Asset;
        }

        public abstract void SetAssetBundle(LoadedAssetBundle assetBundle);
    }
}