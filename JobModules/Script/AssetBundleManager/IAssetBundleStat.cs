namespace AssetBundleManagement
{
    public interface IAssetBundleStat
    {
        void AssetBundleLoaded(string name);
        void AssetBundleNotFound(string name, string errorInfo);
        void AssetBundleUnloaded(string name);
        void AssetLoaded(AssetLoadingPattern loadingPattern, string bundleName, string assetName);
        void AssetNotFound(AssetLoadingPattern loadingPattern, string bundleName, string assetName);
        void SceneLoaded(string bundleName, string sceneName);
        void SceneNotFound(string bundleName, string sceneName);
    }

    class AssetBundleStatPlaceHolder : IAssetBundleStat
    {
        public void AssetBundleLoaded(string name)
        { }

        public void AssetBundleNotFound(string name, string errorInfo)
        { }

        public void AssetBundleUnloaded(string name)
        { }

        public void AssetLoaded(AssetLoadingPattern loadingPattern, string bundleName, string assetName)
        { }

        public void AssetNotFound(AssetLoadingPattern loadingPattern, string bundleName, string assetName)
        { }

        public void SceneLoaded(string bundleName, string sceneName)
        { }

        public void SceneNotFound(string bundleName, string sceneName)
        { }
    }
}