using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetBundleManagement;
using Core.Utils;

namespace Utils.AssetManager
{
    public interface IAssetLoadStat
    {
        bool AssetNotFound(string bundleName, string assetName);
        void SceneLoaded(string bundleName, string sceneName, bool loadSuccess);
    }

    public class AssetBundleStat : IAssetBundleStat
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AssetBundleStat));

        private List<IAssetLoadStat> _loadStats = new List<IAssetLoadStat>();

        public void Add(IAssetLoadStat stat)
        {
            _loadStats.Add(stat);
        }

        public void Remove(IAssetLoadStat stat)
        {
            _loadStats.Remove(stat);
        }

        public void AssetBundleLoaded(string name)
        {
            _logger.InfoFormat("Load Asset Bundle {0} Successfully", name);
        }

        public void AssetBundleNotFound(string name, string errorInfo)
        {
            _logger.ErrorFormat("Load Asset Bundle {0} Failed {1}", name, errorInfo);
        }

        public void AssetBundleUnloaded(string name)
        {
            _logger.DebugFormat("Unload Asset Bundle {0} Successfully", name);
        }

        public void AssetLoaded(AssetLoadingPattern loadingPattern, string bundleName, string assetName)
        {
            _logger.DebugFormat("Load Asset {0}:{1} Pattern {2} Successfully", bundleName, assetName, loadingPattern);
        }

        public void AssetNotFound(AssetLoadingPattern loadingPattern, string bundleName, string assetName)
        {
            _logger.ErrorFormat("Load Asset {0}:{1} Pattern {2} Failed", bundleName, assetName, loadingPattern);

            int count = _loadStats.Count;
            bool processed = false;
            for (int i = 0; i < count; ++i)
            {
                if (_loadStats[i].AssetNotFound(bundleName, assetName))
                {
                    processed = true;
                    break;
                }
            }

            if (!processed)
            {
                _logger.WarnFormat("Can not found processor for Not Found Asset {0}:{1}", bundleName, assetName);                
            }
        }

        public void SceneLoaded(string bundleName, string sceneName)
        {
            _logger.InfoFormat("Load Scene {0}:{1} Successfully", bundleName, sceneName);

            SceneLoaded(bundleName, sceneName, true);
        }

        public void SceneNotFound(string bundleName, string sceneName)
        {
            _logger.ErrorFormat("Load Scene {0}:{1} Failed", bundleName, sceneName);

            SceneLoaded(bundleName, sceneName, false);
        }

        private void SceneLoaded(string bundleName, string sceneName, bool loadSuccess)
        {
            int count = _loadStats.Count;
            for (int i = 0; i < count; ++i)
            {
                _loadStats[i].SceneLoaded(bundleName, sceneName, loadSuccess);
            }
        }
    }
}
