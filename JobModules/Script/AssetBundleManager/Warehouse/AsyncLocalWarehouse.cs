using System;
using AssetBundleManagement;
using AssetBundleManager.Operation;
using UnityEngine;

namespace AssetBundleManager.Warehouse
{
    class AsyncLocalWarehouse : AssetBundleWarehouse
    {
        private readonly string _assetBundlePathPrefix;
        //private readonly string _sceneQuantityLevel;

        public AsyncLocalWarehouse(AssetBundleWarehouseAddr addr, bool isLow)
            : base(addr.Manifest, isLow)
        {
            _assetBundlePathPrefix = addr.Path;
            //_sceneQuantityLevel = sceneQuantityLevel;
        }

        public override AssetBundleLoading LoadAssetBundle(string name)
        {
            string url = _assetBundlePathPrefix + name;

            return OperationFactory.CreateAssetBundleLoadingFromFile(name, url,
                AssetBundle.LoadFromFileAsync(url));
        }

        public override AssetLoading LoadAsset(string bundleName, string name, Type objectType)
        {
            return OperationFactory.CreateAssetAsyncLoading(bundleName, name, objectType);
        }

        public override SceneLoading LoadScene(string bundleName, string name, bool isAdditive)
        {
            return OperationFactory.CreateSceneLoading(bundleName, name, SynchronizationMode.Async, isAdditive);
        }
    }
}