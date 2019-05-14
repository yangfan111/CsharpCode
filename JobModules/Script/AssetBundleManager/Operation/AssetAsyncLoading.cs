using System;
using AssetBundleManagement;
using Core.Utils;
using UnityEngine;

namespace AssetBundleManager.Operation
{
    class AssetAsyncLoading : AssetLoading
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AssetAsyncLoading));
        private AssetBundleRequest _request;

        public AssetAsyncLoading(string bundleName, string assetName, Type objectType)
            : base(AssetLoadingPattern.Async, bundleName, assetName, objectType)
        {
        }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            CreateTime = DateTime.Now;

            _request = ObjectType == null
                ? assetBundle.Bundle.LoadAssetAsync(Name)
                : assetBundle.Bundle.LoadAssetAsync(Name, ObjectType);
        }

        public DateTime CreateTime { get; set; }

        public override bool IsDone()
        {
            if (_request.isDone)
            {
                var t = (DateTime.Now - CreateTime).Milliseconds;
                if (t > 100)
                {
                    _logger.ErrorFormat("asset ;{0}:{1} take {2}", BundleName, Name, t);
                }
            }

            return _request.isDone;
        }

        public override void Process()
        {
            LoadedAsset = _request.asset;
        }
    }
}