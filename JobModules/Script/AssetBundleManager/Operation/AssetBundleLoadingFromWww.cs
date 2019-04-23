using System;
using System.Collections;
using AssetBundleManagement;
using UnityEngine;

namespace AssetBundleManager.Operation
{
    class AssetBundleLoadingFromWww : AssetBundleLoading
    {
        private WWW _loadHandler;

        public AssetBundleLoadingFromWww(string assetBundleName, WWW loadHandler)
            : base(assetBundleName, loadHandler.url)
        {
            if (loadHandler == null)
                throw new ArgumentNullException();
            
            _loadHandler = loadHandler;
        }

        public override bool IsDone()
        {
            return _loadHandler.isDone;
        }

        public override void Process()
        {
            Error = _loadHandler.error;
            if (!string.IsNullOrEmpty(Error))
                return;

            if (_loadHandler.assetBundle == null)
                Error = string.Format("{0} is not a valid assetbundle.", Url);
            else
                Content = new LoadedAssetBundle(_loadHandler.assetBundle);
        }

        public override IEnumerator Cancel()
        {
            _loadHandler.Dispose();
            return null;
        }
    }
}