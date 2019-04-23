using System.Collections;
using AssetBundleManagement;
using UnityEngine;

namespace AssetBundleManager.Operation
{
    class AssetBundleLoadingFromFile : AssetBundleLoading
    {
        private AssetBundleCreateRequest _loadHandler;

        public AssetBundleLoadingFromFile(string assetBundleName, string url,
            AssetBundleCreateRequest loadHandler)
            : base(assetBundleName, url)
        {
            _loadHandler = loadHandler;
        }

        public override bool IsDone()
        {
            return _loadHandler.isDone;
        }

        public override void Process()
        {
            if (_loadHandler.assetBundle == null)
                Error = string.Format("{0} is not a valid assetbundle.", Url);
            else
                Content = new LoadedAssetBundle(_loadHandler.assetBundle);
        }

        public override IEnumerator Cancel()
        {
            yield return _loadHandler;

            if (_loadHandler.assetBundle != null)
                _loadHandler.assetBundle.Unload(true);
        }
    }
}