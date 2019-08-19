using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.sql;

namespace Assets.App.Client.Tools
{
    public interface IResetable
    {
        void Reset();
    }

    public class SampleDataPool<T> where T: IResetable, new()
    {
        public Queue<T> _pool = new Queue<T>();
        public T Get()
        {
            if (_pool.Count == 0)
            {
                return new T();
            }

            return _pool.Dequeue();
        }

        public void Return(T val)
        {
            if (val != null)
            {
                val.Reset();
                _pool.Enqueue(val);
            }
        }
    }
}
