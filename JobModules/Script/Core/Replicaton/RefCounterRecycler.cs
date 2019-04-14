using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Core.ObjectPool;
using Core.Utils;
using Utils.Concurrent;

namespace Core.Replicaton
{
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
    public class RefCounterRecycler : AbstractThread
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RefCounterRecycler));
        public static RefCounterRecycler Instance;

        static RefCounterRecycler()
        {
            Instance = new RefCounterRecycler();
            Instance.Start();
        }

        public RefCounterRecycler() : base("RefCounterRecycler")
        {
        }

        private BlockingQueue<IRefCounter> _disposeQueue = new BlockingQueue<IRefCounter>(2048);


        public void ReleaseReference(IRefCounter obj)
        {
            _disposeQueue.Enqueue(obj);
        }

        private double _runningTime;
        private double _totalTime;
        private Stopwatch _stopwatch = new Stopwatch();
        private float _rate = 0;

        protected override void Run()
        {
            Thread.CurrentThread.Name = "RefCounterDisposeThread";
            while (Running)
            {
                try
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    var obj = _disposeQueue.Dequeue();
                    if (obj == null) continue;
                    _stopwatch.Stop();
                    _runningTime += _stopwatch.ElapsedTicks;
                    _stopwatch.Start();
                    obj.ReleaseReference();

                    _totalTime += _stopwatch.ElapsedTicks;
                    if (_totalTime >= 5000 * 10000)
                    {
                        _rate = (float) (100f * (1f - _runningTime / _totalTime));
                        _totalTime = 0;
                        _runningTime = 0;
                    }
                }
                catch (Exception e)
                {
                    _logger.InfoFormat("error while delRef {0}", e);
                }
            }
        }

        public override float Rate
        {
            get { return _rate; }
        }
    }
}
#pragma warning restore RefCounter001, RefCounter002 // possible reference counter error