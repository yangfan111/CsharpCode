using System;
using System.Collections;
using System.Collections.Generic;

namespace gnu.trove.map.hash
{
    

    public class THashMap<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public TValue Get(TKey key)
        {
            TValue rc;
            TryGetValue(key, out rc);
            return rc;
        }

        public void Put(TKey key, TValue value)
        {
            Add(key, value);
            
        }

        public TIterator<TKey, TValue> Iterator()
        {
            return new TIterator<TKey, TValue>(GetEnumerator());
        }

        public Object[] Keys()
        {
            TKey[] a = new TKey[base.Count];
            base.Keys.CopyTo(a, 0);

            Object[] rc = new Object[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                rc[i] = a[i];
            }
            return rc;
        }
    }

    public class TIterator<TKey, TValue>
    {
        public TIterator(Dictionary<TKey, TValue>.Enumerator e)
        {
            this.e = e;
        }
        private Dictionary<TKey, TValue>.Enumerator e;
        public Boolean HasNext()
        {
            return e.MoveNext();
        }

        public void Advance()
        {
            
        }

        public TKey Key()
        {
            return e.Current.Key;
        }

        public TValue Value()
        {
            return e.Current.Value;
        }


    }

    public class TIntIntIterator : TIterator<int, int>
    {
        public TIntIntIterator(Dictionary<int, int>.Enumerator e) : base(e)
        {
        }


    }
    public class TObjectIntHashMap<T> : THashMap<T, int>
    {
    }

    public class TIntObjectHashMap<T> : THashMap<int, T>
    {
    }

    public class TIntIntHashMap : THashMap<int, int>
    {
        public TIntIntIterator Iterator()
        {
            return new TIntIntIterator(GetEnumerator());
        }
    }


    public class TIntIntInterator : TIterator<int, int>
    {
        public TIntIntInterator(Dictionary<int, int>.Enumerator e) : base(e)
        {
        }
    }

    public class TLongObjectHashMap<T> : THashMap<long, T>
    {
    }

}