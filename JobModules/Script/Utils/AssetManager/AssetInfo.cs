using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils.AssetManager
{
    public struct AssetInfo
    {
        public static readonly AssetInfo EmptyInstance = new AssetInfo(string.Empty, string.Empty);
            
        public class AssetInfoComparer : IEqualityComparer<AssetInfo>
        {
            public bool Equals(AssetInfo x, AssetInfo y)
            {
                return string.Equals(x.AssetName, y.AssetName, System.StringComparison.Ordinal) 
                       && string.Equals(x.BundleName, y.BundleName, System.StringComparison.Ordinal);
            }

            public int GetHashCode(AssetInfo obj)
            {
                unchecked
                {
                    return ((obj.BundleName != null ? obj.BundleName.GetHashCode() : 0) * 397) ^ (obj.AssetName != null ? obj.AssetName.GetHashCode() : 0);
                }
            }

            public static readonly AssetInfoComparer Instance = new AssetInfoComparer();
        }


        public class AssetInfoIngoreCaseComparer : IEqualityComparer<AssetInfo>
        {
            public bool Equals(AssetInfo x, AssetInfo y)
            {
                return string.Equals(x.AssetName, y.AssetName, System.StringComparison.OrdinalIgnoreCase)
                       && string.Equals(x.BundleName, y.BundleName, System.StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(AssetInfo obj)
            {
                unchecked
                {
                    return ((obj.BundleName != null ? HashCodeProvider.GetHashCode(obj.BundleName) : 0) * 397) ^ (obj.AssetName != null ? HashCodeProvider.GetHashCode(obj.AssetName) : 0);
                }
            }

            private static readonly CaseInsensitiveHashCodeProvider HashCodeProvider = new CaseInsensitiveHashCodeProvider();
            public static readonly AssetInfoIngoreCaseComparer Instance = new AssetInfoIngoreCaseComparer();
        }


        public int GetHashCode()
        {
            unchecked
            {
                return ((BundleName != null ? BundleName.GetHashCode() : 0) * 397) ^ (AssetName != null ? AssetName.GetHashCode() : 0);
            }
        }
        public bool Equals(AssetInfo obj)
        {
            return string.Equals(AssetName, obj.AssetName, System.StringComparison.Ordinal) & string.Equals(BundleName, obj.BundleName, System.StringComparison.Ordinal);
        }

        public string BundleName;

        public string AssetName;

        public AssetInfo(string bundleName, string assetName)
        {
            BundleName = bundleName;
            AssetName = assetName;
        }

        public override string ToString()
        {
            return "[" + BundleName + ":" + AssetName + "]";
        }

        public static bool operator ==(AssetInfo left, AssetInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetInfo left, AssetInfo right)
        {
            return !left.Equals(right);
        }
    }
}