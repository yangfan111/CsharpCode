using System;
using AssetBundleManagement;
using AssetBundleManager.Operation;
using UnityEngine;

namespace AssetBundleManager.Warehouse
{
    class SyncWarehouse : AssetBundleWarehouse
    {
        private readonly string _assetBundlePathPrefix;
        //private readonly string _sceneQuantityLevel;

        public SyncWarehouse(AssetBundleWarehouseAddr addr, bool isLow)
            : base(addr.Manifest, isLow)
        {
            _assetBundlePathPrefix = addr.Path;
            //_sceneQuantityLevel = sceneQuantityLevel;
        }

        public override AssetBundleLoading LoadAssetBundle(string name)
        {
            string url = _assetBundlePathPrefix + name;

            var operation = OperationFactory.CreateAssetBundleSyncLoading(name, url);
            operation.SetContent(AssetBundle.LoadFromFile(url));

            return operation;
        }

        public override AssetLoading LoadAsset(string bundleName, string name, Type objectType)
        {
            return OperationFactory.CreateAssetSyncLoading(bundleName, name, objectType);
        }

        public override SceneLoading LoadScene(string bundleName, string name, bool isAdditive)
        {
            return OperationFactory.CreateSceneLoading(bundleName, name, SynchronizationMode.Async, isAdditive);
        }
    }
}