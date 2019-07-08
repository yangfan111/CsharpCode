using Core.GameModule.System;
using Core.Utils;
using System.Collections.Generic;
using Utils.AssetManager;

namespace App.Shared.SessionStates
{
    internal class PreLoadAssetProvider : IAssetInfoProvider
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PreLoadAssetProvider));

        private static readonly List<AssetInfo> CommonPreLoadAsset = new List<AssetInfo>()
        {
           
        };

        private ICommonSessionObjects _commonSessionObjects;
        public PreLoadAssetProvider(ICommonSessionObjects commonSessionObejects)
        {
            _commonSessionObjects = commonSessionObejects;
        }

        public List<AssetInfo> AssetInfos
        {
            get
            {
                var assetInfosStr = _commonSessionObjects.RoomInfo.PreLoadAssetInfo;
                if (assetInfosStr == null)
                {
                    _logger.ErrorFormat("PreLoadAssetInfo is Null!");
                    return CommonPreLoadAsset;
                }

                var assetInfos = ParseAssetInfosStr(assetInfosStr);
                assetInfos.AddRange(CommonPreLoadAsset);
                return assetInfos;
            }
           
        }

        private List<AssetInfo> ParseAssetInfosStr(List<string> assetInfosStr)
        {
            var assetInfoList = new List<AssetInfo>();
            foreach (string asset in assetInfosStr)
            {
                int bundleNameLen = asset.LastIndexOf('/');
                if (bundleNameLen <= 0) continue;
                var bundleName = asset.Substring(0, bundleNameLen);
                var assetName = asset.Substring(bundleNameLen + 1);
                assetInfoList.Add(new AssetInfo(bundleName, assetName));
            }
            /*var assetInfoArray = assetInfosStr.Split(",");
            foreach (var info in assetInfoArray)
            {
                var assetInfo = info.Trim();
                int bundleNameLen = assetInfo.LastIndexOf('/');
                if (bundleNameLen <= 0)
                {
                    _logger.ErrorFormat("Invalid assetbundle name of asset info {0}", assetInfo);
                    continue;
                }

                var bundleName = assetInfo.Substring(0, bundleNameLen);
                var assetName = assetInfo.Substring(bundleNameLen + 1);

                if (assetName.Length == 0)
                {
                    _logger.ErrorFormat("Invalid asset name of asset info {0}", assetInfo);
                    continue;
                }

                assetInfoList.Add(new AssetInfo(bundleName, assetName));
            }*/

            return assetInfoList;
        }
    }
}
