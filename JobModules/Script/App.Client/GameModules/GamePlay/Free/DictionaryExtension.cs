using System.Collections.Generic;

namespace Assets.Sources.Free
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
