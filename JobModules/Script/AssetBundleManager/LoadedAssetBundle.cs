using UnityEngine;

namespace AssetBundleManagement
{
    public class LoadedAssetBundle
    {
        public AssetBundle Bundle { get; private set; }
        private int _referencedCount;

        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            Bundle = assetBundle;
            _referencedCount = 1;
        }
        
        public void IncreaseReference()
        {
            _referencedCount++;
        }

        public void DecreaseReference()
        {
            _referencedCount--;
        }
        
        public bool NoLongerInUse()
        {
            return _referencedCount <= 0;
        }
    }
}