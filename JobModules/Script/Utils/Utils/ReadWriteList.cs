using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Utils
{
    public class ReadWriteList<TItem>
    {
        private List<TItem> _channels = new List<TItem>();
        private volatile List<TItem>  _copy = new List<TItem>();
        private volatile bool _dirty = true;
        private int _lockNum;
        public void Add(TItem channel)
        {
            try
            {
                Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                while (Interlocked.Increment(ref _lockNum) != 1)
                {
                    Interlocked.Decrement(ref _lockNum);
                    spin.SpinOnce();
                }
                _channels.Add(channel);
                _dirty = true;
            }
            finally
            {
                Interlocked.Decrement(ref _lockNum);
            }
        }

        public void Remove(TItem channel)
        {
            try
            {
                Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                while (Interlocked.Increment(ref _lockNum) != 1)
                {
                    Interlocked.Decrement(ref _lockNum);
                    spin.SpinOnce();
                }
                _channels.Remove(channel);
                _dirty = true;
            }
            finally
            {
                Interlocked.Decrement(ref _lockNum);
            }
        }

        public List<TItem> ForRead()
        {
            if (_dirty)
            {
                _dirty = false;
                try
                {
                    Core.Utils.SpinWait spin = new Core.Utils.SpinWait();
                    while (Interlocked.Increment(ref _lockNum) != 1)
                    {
                        Interlocked.Decrement(ref _lockNum);
                        spin.SpinOnce();
                    }

                    _copy = _channels.ToList();
                }
                finally
                {
                    Interlocked.Decrement(ref _lockNum);
                }
            }
            return _copy;
        }
    }
}