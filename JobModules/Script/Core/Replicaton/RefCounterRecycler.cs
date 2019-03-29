using System;
using System.Collections;
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
        private int _rate;


        public void ReleaseReference(IRefCounter obj)
        {
            _disposeQueue.Enqueue(obj);
        }


        protected override void Run()
        {
	        Thread.CurrentThread.Name = "RefCounterDisposeThread";
			while (Running)
            {
                try
                {
                    var obj = _disposeQueue.Dequeue();
                    if (obj == null) continue;
                    obj.ReleaseReference();
                    
                }
                catch (Exception e)
                {
                    _logger.InfoFormat("error while delRef {0}", e);
                }
               
            }
        }

        public override float Rate
        {
            get { return _disposeQueue.Count; }
        }
    }
}
#pragma warning restore RefCounter001, RefCounter002 // possible reference counter error