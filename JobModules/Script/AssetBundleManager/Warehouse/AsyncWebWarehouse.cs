using System.Collections.Generic;
using AssetBundleManagement;
using AssetBundleManager.Operation;
using UnityEngine;

namespace AssetBundleManager.Warehouse
{
    class AsyncWebWarehouse : AssetBundleWarehouse
    {
        private readonly string _assetBundlePathPrefix;
        private readonly Dictionary<string, Hash128> _allHashes = new Dictionary<string, Hash128>();
        //private readonly string _sceneQuantityLevel;

        public AsyncWebWarehouse(AssetBundleWarehouseAddr addr, bool isLow)
            : base(addr.Manifest, isLow)
        {
            _assetBundlePathPrefix = addr.Path;
            //_sceneQuantityLevel = sceneQuantityLevel;
        }

        public override void SetManifest(AssetBundleManifest obj)
        {
            foreach (var v in obj.GetAllAssetBundles())
                _allHashes.Add(v, obj.GetAssetBundleHash(v));
            
            base.SetManifest(obj);
        }

        public override AssetBundleLoading LoadAssetBundle(string name)
        {
            return LoadAssetBundle(name, false);
        }

        private AssetBundleLoading LoadAssetBundle(string name, bool isLoadingManifest)
        {
            string url = _assetBundlePathPrefix + name;

            WWW request;
            if (isLoadingManifest)
                request = new WWW(url);
            else
            {
                Hash128 hash;
                _allHashes.TryGetValue(name, out hash);
                request = WWW.LoadFromCacheOrDownload(url, hash, 0);
            }

            var ret = OperationFactory.CreateAssetBundleLoadFromWww(name, request);

            return ret;
        }
        
        public override AssetLoading LoadAsset(string bundleName, string name)
        {
            return OperationFactory.CreateAssetAsyncLoading(bundleName, name);
        }

        public override SceneLoading LoadScene(string bundleName, string name, bool isAdditive)
        {
            return OperationFactory.CreateSceneLoading(bundleName, name, SynchronizationMode.Async, isAdditive);
        }
    }
}