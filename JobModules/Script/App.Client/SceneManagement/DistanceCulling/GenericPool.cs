using System.Collections.Generic;

namespace App.Client.SceneManagement.DistanceCulling
{
    class GenericPool<T> : IGenericPool<T> where T : ICacheableElement
    {
        private T _meta;
        private readonly Queue<T> _pool = new Queue<T>();

        public void SetMeta(T meta)
        {
            _meta = meta;
        }

        public T Get()
        {
            if (_pool.Count <= 0)
            {
                var ret = (T) _meta.Clone();
                ret.Reset();

                return ret;
            }

            return _pool.Dequeue();
        }

        public void Reuse(T m)
        {
            m.Reset();
            _pool.Enqueue(m);
        }
    }
}