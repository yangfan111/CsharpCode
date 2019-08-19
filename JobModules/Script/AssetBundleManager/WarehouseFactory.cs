using System;
using AssetBundleManager.Warehouse;
using Common;

namespace AssetBundleManagement
{
    static class WarehouseFactory
    {
        public static AssetBundleWarehouse CreateWarehouse(AssetBundleWarehouseAddr addr, bool isLow, string useMD5 = null)
        {
            AssetBundleWarehouse ret = null;

            switch (addr.Pattern)
            {
                case AssetBundleLoadingPattern.Simulation:
                    ret = new SimulationWarehouse(addr, isLow, useMD5);
                    break;
                case AssetBundleLoadingPattern.AsyncLocal:
                    ret = new AsyncLocalWarehouse(addr, isLow, useMD5);
                    break;
                case AssetBundleLoadingPattern.AsyncWeb:
                    ret = new AsyncWebWarehouse(addr, isLow, useMD5);
                    break;
                case AssetBundleLoadingPattern.Sync:
                    ret = new SyncWarehouse(addr, isLow, useMD5);
                    break;
                case AssetBundleLoadingPattern.EndOfTheWorld:
                    throw new ArgumentException("AssetBundleLoadingPattern.EndOfTheWorld");
            }

            return ret;
        }
    }
}