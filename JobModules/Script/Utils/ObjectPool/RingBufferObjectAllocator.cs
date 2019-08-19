using System.Collections.Generic;
using System.Text;
using System.Threading;
using Core.Utils;

namespace Core.ObjectPool
{
    public class RingBufferObjectAllocator : IObjectAllocator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RingBufferObjectAllocator));

        public void PrintDebugInfo(StringBuilder sb)
        {
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append(_factory);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(AllocCount);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(FreeCount);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(AllocCount - FreeCount);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(NewCount);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(_pool.Count);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(_pool.Capacity);
            sb.Append("</td>");


            sb.Append("</tr>");
        }

        private int _allocatorNumber;
        private int _freeNumber;
        private volatile RingBuffer<object> _pool = new RingBuffer<object>(64);
        private  volatile RingBuffer<object> _old = new RingBuffer<object>(1);
        private IObjectFactory _factory;

        public long NewCount;

        public long AllocCount;

        public long FreeCount;


        public RingBufferObjectAllocator(IObjectFactory factory, int initPoolSize = 0, int allocatorNumber=0)
        {
            allocatorNumber = allocatorNumber > initPoolSize/2 ? initPoolSize/2 : allocatorNumber;
            _factory = factory;
            if (initPoolSize > _pool.Capacity)
            {
                RingBuffer<object> old = _pool;
                _pool = new RingBuffer<object>(initPoolSize * 2);
            }

            List<object> temp = new List<object>();
            for (int i = 0; i < allocatorNumber; i++)
            {
                var o = Allocate();
                temp.Add(o);
            }

            foreach (var o in temp)
            {
                Free(o);
            }

            //UnityEngine.Debug.LogErrorFormat("{0}init size:{1} {2}", factory, initPoolSize, _pool.Count);
        }

        public object Allocate()
        {
            Interlocked.Increment(ref AllocCount);

#if (NET_4_6 && UNITY_2017)
            System.Threading.SpinWait spin = new System.Threading.SpinWait();
#else
            Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
#endif
            while (Interlocked.Increment(ref _allocatorNumber) != 1)
            {
                Interlocked.Decrement(ref _allocatorNumber);
                spin.SpinOnce();
            }

            object obj;
            var succ = _pool.TryDequeue(out obj);
            Interlocked.Decrement(ref _allocatorNumber);
            if (!succ || obj == null)
            {
                Interlocked.Increment(ref NewCount);
                obj = _factory.MakeObject();
            }

            _factory.ActivateObject(obj);
            return obj;
        }


        // there may be some leak while increasing cap
        // but it's ok...
        public void Free(object t)
        {
            if (t == null)
            {
                return;
            }


            _factory.DestroyObject(t);
            Interlocked.Increment(ref FreeCount);
#if (NET_4_6 && UNITY_2017)
            System.Threading.SpinWait spin = new System.Threading.SpinWait();
#else
            Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
#endif
            while (Interlocked.Increment(ref _freeNumber) != 1)
            {
                Interlocked.Decrement(ref _freeNumber);
                spin.SpinOnce();
            }
            if (_pool.Count > _pool.Capacity - 5)
            {
              _old = _pool;
              _pool = new RingBuffer<object>(_old.Capacity * 2);
              _logger.InfoFormat("ring buffer not big enough for object pool of type {0}",
                  _factory.MakeObject().GetType());
              if (_old.Count > 0)
            {
                object obj;
                var succ = _old.TryDequeue(out obj);
                while (succ && obj != null)
                {
                    _pool.Enqueue(obj);
                    succ = _old.TryDequeue(out obj);
                }
            }
              _old = null;
            }



            {


                _pool.Enqueue(t);
                Interlocked.Decrement(ref _freeNumber);
            }
            
        }

        public IObjectFactory Factory
        {
            get { return _factory; }
            set { _factory = value; }
        }
    }
}