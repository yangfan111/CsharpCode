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

        private string useMD5AB = null;

        public SyncWarehouse(AssetBundleWarehouseAddr addr, bool isLow, string useMD5 = null)
            : base(addr.Manifest, isLow)
        {
            _assetBundlePathPrefix = addr.Path;
            //_sceneQuantityLevel = sceneQuantityLevel;

            useMD5AB = useMD5;
            if (!string.IsNullOrEmpty(useMD5AB))
            {
                LoadAssetBundlesXML(_assetBundlePathPrefix, useMD5AB);
            }
        }

        public override AssetBundleLoading LoadAssetBundle(string name)
        {
            string url = _assetBundlePathPrefix + name;

            // [PPAN] Read MD5 AssetBundles
            var operation = OperationFactory.CreateAssetBundleSyncLoading(name, url);
            if (!string.IsNullOrEmpty(useMD5AB))
            {
                string strMD5 = ReadMD5FromXL(name);
                if (string.IsNullOrEmpty(strMD5))
                {
                    Debug.LogError(name + " has not MD5 file");
                }
                string strURLwithMD5 = string.Format("{0}{1}_{2}", _assetBundlePathPrefix, name, strMD5);
                //Debug.Log("SyncWarehouse strURLwithMD5 = " + strURLwithMD5);
                url = strURLwithMD5;
            }

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