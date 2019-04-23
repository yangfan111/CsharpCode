using System;
using System.Threading;
using Core.Utils;
using Utils.Concurrent;
using Utils.Singleton;

namespace Utils.Utils
{
    public class ConsumerThread<Job, Result> : AbstractThread
    {
        private LoggerAdapter _logger;
        private BlockingQueue<Job> _queue = new BlockingQueue<Job>(32);
        private float _rate;
        private Func<Job, Result> _action;
        private CustomProfileInfo _profile;
        private CustomProfileInfo _profile2;
        private volatile int _count = 0;

        public ConsumerThread(string name, Func<Job, Result> action) :base(name)
        {
            _action = action;
            _profile = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(name);
            _profile2 = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(name+"_all");
            _logger =
                new LoggerAdapter(Name);
        }

        public void Offer(Job job)
        {
            _queue.Enqueue(job);
            Interlocked.Increment(ref _count);
        }

        public bool IsDone()
        {
            return _count == 0;
        }

      
        
        protected override void Run()
        {
            Thread.CurrentThread.Name = Name;
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            while (Running)
            {
                try
                {
                    Job task = _queue.Dequeue(1);
                    if (task == null) continue;
                    _logger.Debug("start");
                    try
                    {
                        SingletonManager.Get<DurationHelp>().ProfileStart(_profile);
                        _action(task);
                        SingletonManager.Get<DurationHelp>().ProfileStart(_profile2);
                        Interlocked.Decrement(ref _count);
                    }
                    finally
                    {
                        SingletonManager.Get<DurationHelp>().ProfileEnd(_profile);
                        SingletonManager.Get<DurationHelp>().ProfileEnd(_profile2);
                    }
                    _logger.Debug("end");
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Run Error:{0}", e);
                }
            }
        }

        public override float Rate
        {
            get { return _rate; }
        }
    }
}