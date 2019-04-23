using System;
using AssetBundleManager.Warehouse;
using Common;

namespace AssetBundleManagement
{
    static class WarehouseFactory
    {
        public static AssetBundleWarehouse CreateWarehouse(AssetBundleWarehouseAddr addr, bool isLow)
        {
            AssetBundleWarehouse ret = null;

            switch (addr.Pattern)
            {
                case AssetBundleLoadingPattern.Simulation:
                    ret = new SimulationWarehouse(addr, isLow);
                    break;
                case AssetBundleLoadingPattern.AsyncLocal:
                    ret = new AsyncLocalWarehouse(addr, isLow);
                    break;
                case AssetBundleLoadingPattern.AsyncWeb:
                    ret = new AsyncWebWarehouse(addr, isLow);
                    break;
                case AssetBundleLoadingPattern.Sync:
                    ret = new SyncWarehouse(addr, isLow);
                    break;
                case AssetBundleLoadingPattern.EndOfTheWorld:
                    throw new ArgumentException("AssetBundleLoadingPattern.EndOfTheWorld");
            }

            return ret;
        }
    }
}