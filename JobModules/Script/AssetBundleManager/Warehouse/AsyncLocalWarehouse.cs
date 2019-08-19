using System;
using System.Collections.Generic;
using System.IO;
using AssetBundleManagement;
using AssetBundleManager.Operation;
using UnityEngine;

namespace AssetBundleManager.Warehouse
{
    class AsyncLocalWarehouse : AssetBundleWarehouse
    {
        private readonly string _assetBundlePathPrefix;
        //private readonly string _sceneQuantityLevel;

        private string useMD5AB = null;

        public AsyncLocalWarehouse(AssetBundleWarehouseAddr addr, bool isLow, string useMD5 = null)
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

            // [PPAN] º”‘ÿMD5 AssetBundles
            if (!string.IsNullOrEmpty(useMD5AB))
            {
                string strMD5 = ReadMD5FromXL(name);
                if (string.IsNullOrEmpty(strMD5))
                {
                    Debug.LogError(name + " has not MD5 file");
                }
                string strURLwithMD5 = string.Format("{0}{1}_{2}", _assetBundlePathPrefix, name, strMD5);
                //Debug.Log("AsyncLocalWarehouse strURLwithMD5 = " + strURLwithMD5);
                return OperationFactory.CreateAssetBundleLoadingFromFile(name, url, AssetBundle.LoadFromFileAsync(strURLwithMD5));
            }
            else
            {
                return OperationFactory.CreateAssetBundleLoadingFromFile(name, url,
                AssetBundle.LoadFromFileAsync(url));
            }
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