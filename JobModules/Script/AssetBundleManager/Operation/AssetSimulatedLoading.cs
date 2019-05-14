using System;
using AssetBundleManagement;
using Object = UnityEngine.Object;

namespace AssetBundleManager.Operation
{
    class AssetSimulatedLoading : AssetLoading
    {
        public AssetSimulatedLoading(string bundleName, string assetName, Type objectType)
            : base(AssetLoadingPattern.Simulation, bundleName, assetName, objectType)
        { }

        public override bool IsDone()
        {
            return true;
        }

        public override void Process()
        { }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        { }

   
        public void SetAsset(Object obj)
        {
            LoadedAsset = obj;
        }
    }
}