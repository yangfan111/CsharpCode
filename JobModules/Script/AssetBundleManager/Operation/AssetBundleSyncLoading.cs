using System.Collections;
using AssetBundleManagement;
using UnityEngine;

namespace AssetBundleManager.Operation
{
    class AssetBundleSyncLoading : AssetBundleLoading
    {
        public AssetBundleSyncLoading(string assetBundleName, string url)
            : base(assetBundleName, url)
        { }

        public override bool IsDone()
        {
            return true;
        }

        public override void Process()
        { }

        public void SetContent(AssetBundle content)
        {
            Content = new LoadedAssetBundle(content);
        }

        public override IEnumerator Cancel()
        {
            Content.Bundle.Unload(true);
            return null;
        }
    }
}