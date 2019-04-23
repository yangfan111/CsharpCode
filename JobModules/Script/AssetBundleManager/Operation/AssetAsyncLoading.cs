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
        
        public AssetAsyncLoading(string bundleName, string assetName)
            : base(AssetLoadingPattern.Async, bundleName, assetName)
        { }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            CreateTime = DateTime.Now;
            
            _request = assetBundle.Bundle.LoadAssetAsync(Name);
        }

        public DateTime CreateTime { get; set; }

        public override bool IsDone()
        {
            if (_request.isDone)
            {
                var t = (DateTime.Now - CreateTime).Milliseconds;
                if (t > 50)
                {
                    _logger.ErrorFormat("asset ;{0}:{1} take {2}", BundleName,Name, t);
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