using System;
using AssetBundleManagement;
using Object = UnityEngine.Object;

namespace AssetBundleManager.Operation
{
    public abstract class AssetLoading : PollingOperation
    {
        public string BundleName { get; private set; }
        public string Name { get; private set; }

        public AssetLoadingPattern LoadingPattern { get; private set; }

        public Object LoadedAsset { get; protected set; }

        public Type ObjectType { get; protected set; }
        protected AssetGroup AssetGroup;
        public bool IsSceneLoading
        {
            get { return AssetGroup == AssetGroup.Scene; }
        }

        public virtual bool IsLoadFailed
        {
            get { return LoadedAsset == null; }
        }

        protected AssetLoading(AssetLoadingPattern loadingPattern, string bundleName, string assetName, Type objectType)
        {
            LoadingPattern = loadingPattern;
            BundleName = bundleName;
            Name = assetName;
            AssetGroup = AssetGroup.Asset;
            ObjectType = objectType;
        }

        public abstract void SetAssetBundle(LoadedAssetBundle assetBundle);


        public override string ToString()
        {
            return "[" + BundleName + ":" + Name + "]";
        }
    }
}