using AssetBundleManagement;
using AssetBundleManager.Operation;

namespace AssetBundleManager.Warehouse
{
    class SimulationWarehouse : AssetBundleWarehouse
    {
        public SimulationWarehouse(AssetBundleWarehouseAddr addr, bool isLow)
            : base(addr.Manifest, isLow)
        {
        }

        public override InitRetValue Init()
        {
            return null;
        }

        public override AssetBundleLoading LoadAssetBundle(string name)
        {
            return OperationFactory.CreateAssetBundleSimulatedLoading(name, name);
        }

        public override AssetLoading LoadAsset(string bundleName, string name)
        {
            AssetSimulatedLoading operation = null;
#if UNITY_EDITOR
            UnityEngine.Object asset = null;
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, name);

            if (assetPaths.Length == 0)
            {
                if (name.StartsWith("Assets/"))
                    asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(name);
            }
            else
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPaths[0]);

            if (asset != null)
            {
                operation = OperationFactory.CreateAssetSimulationLoading(bundleName, name);
                operation.SetAsset(asset);
            }
#endif
            return operation;
        }

        public override SceneLoading LoadScene(string bundleName, string name, bool isAdditive)
        {
            return OperationFactory.CreateSceneLoading(bundleName, name, SynchronizationMode.Simulated, isAdditive);
        }

        public override string RemapBundleName(string name)
        {
            return name;
        }
    }
}