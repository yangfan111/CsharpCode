using System.Collections;
using System.Collections.Generic;

namespace Assets.Sources.Utils
{

    public class ObjectPool<TValue> : IEnumerable<TValue>
    {
        
        private Queue<TValue> _queue = new Queue<TValue>();

        public TValue GetOrNull()
        {
            var ret = default(TValue);
            if (_queue.Count > 0)
                ret = _queue.Dequeue();
            return ret;
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Add(TValue value)
        {
            _queue.Enqueue(value);
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}
