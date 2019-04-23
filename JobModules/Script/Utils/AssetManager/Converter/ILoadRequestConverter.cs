using UnityEngine;

namespace Utils.AssetManager.Converter
{
    public interface ILoadRequestConverter<T>
    {
        T Convert(Object obj, AssetInfo assetInfo);
        bool IsMultiThreadSupport { get; }
    }
}