using System;
using AssetBundleManager.Operation;
using UnityEngine;

namespace AssetBundleManagement
{
    static class OperationFactory
    {
        public static AssetBundleSyncLoading CreateAssetBundleSyncLoading(string name, string url)
        {
            return new AssetBundleSyncLoading(name, url);
        }

        public static AssetBundleLoadingFromFile CreateAssetBundleLoadingFromFile(string name, string url,
            AssetBundleCreateRequest request)
        {
            return new AssetBundleLoadingFromFile(name, url, request);
        }

        public static AssetBundleLoadingFromWww CreateAssetBundleLoadFromWww(string name, WWW request)
        {
            return new AssetBundleLoadingFromWww(name, request);
        }

        public static AssetBundleSimulatedLoading CreateAssetBundleSimulatedLoading(string name,
            string url)
        {
            return new AssetBundleSimulatedLoading(name, url);
        }
        
        public static AssetSyncLoading CreateAssetSyncLoading(string bundleName, string name,Type objectType)
        {
            return new AssetSyncLoading(bundleName, name, objectType);
        }
        
        public static AssetAsyncLoading CreateAssetAsyncLoading(string bundleName, string name, Type objectType)
        {
            return new AssetAsyncLoading(bundleName, name, objectType);
        }

        public static AssetSimulatedLoading CreateAssetSimulationLoading(string bundleName, string name,
            Type objectType)
        {
            return new AssetSimulatedLoading(bundleName, name, objectType);
        }

        public static ManifestLoading CreateManifestLoading(string bundleName, string name)
        {
            return new ManifestLoading(bundleName, name);
        }

        public static SceneLoading CreateSceneLoading(string bundleName, string name, SynchronizationMode mode,
            bool isAdditive)
        {
            return new SceneLoading(bundleName, name, mode, isAdditive);
        }
    }
}