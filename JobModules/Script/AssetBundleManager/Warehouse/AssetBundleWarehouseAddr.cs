using System.Collections.Generic;
using Common;

namespace AssetBundleManager.Warehouse
{
    public class AssetBundleWarehouseAddr
    {
        public AssetBundleLoadingPattern Pattern;
        public string Path;
        public string Manifest;

        public override string ToString()
        {
            return string.Format("[Pattern: {0} Path: {1}, Manifest: {2}]", Pattern, Path ?? "null", Manifest ?? "null");
        }

        class EqualityComparer : IEqualityComparer<AssetBundleWarehouseAddr>
        {
            public bool Equals(AssetBundleWarehouseAddr x, AssetBundleWarehouseAddr y)
            {
                return x == y || x != null && y != null
                                           && string.Equals(x.Path, y.Path) && string.Equals(x.Manifest, y.Manifest);
            }

            public int GetHashCode(AssetBundleWarehouseAddr obj)
            {
                unchecked
                {
                    return (((obj.Pattern.GetHashCode() * 397) ^ (obj.Path != null ? obj.Path.GetHashCode() : 0)) * 397) ^ (obj.Manifest != null ? obj.Manifest.GetHashCode() : 0);
                }
            }
        }
        
        public static IEqualityComparer<AssetBundleWarehouseAddr> Comparer = new EqualityComparer();
    }
}