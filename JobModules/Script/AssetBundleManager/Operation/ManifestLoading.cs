using System;
using System.Collections;
using AssetBundleManagement;

namespace AssetBundleManager.Operation
{
    class ManifestLoading : AssetAsyncLoading, IEnumerator
    {
        private bool _assetBundleReady;

        public ManifestLoading(string bundleName, string assetName)
            : base(bundleName, assetName)
        { }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public object Current { get { return null; } }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            base.SetAssetBundle(assetBundle);
            _assetBundleReady = true;
        }

        public override bool IsDone()
        {
            return _assetBundleReady && base.IsDone();
        }
    }
}