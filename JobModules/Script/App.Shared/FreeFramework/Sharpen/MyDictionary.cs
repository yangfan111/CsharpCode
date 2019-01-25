using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sharpen
{
    [Serializable]
    public class MyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public MyDictionary()
        {

        }

        public MyDictionary(SerializationInfo info, StreamingContext context): base(info, context)
        {
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue ret;
                TryGetValue(key, out ret);
                return ret;
            }
            set { base[key] = value; }
        }
    }
}
