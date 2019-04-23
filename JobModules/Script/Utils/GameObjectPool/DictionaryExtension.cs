using System.Collections.Generic;

namespace Assets.Sources.Utils
{
    public static class DictionaryExtension
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue ret;
            dict.TryGetValue(key, out ret);
            return ret;
        }
    }

}
