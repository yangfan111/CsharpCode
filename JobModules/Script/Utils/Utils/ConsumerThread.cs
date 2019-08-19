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
        private BlockingQueue<Job> _queue;
        private float _rate;
        private Func<Job, Result> _action;
        private CustomProfileInfo _profile;
        private CustomProfileInfo _profile2;
        public ConsumerThread(string name, Func<Job, Result> action, BlockingQueue<Job> queue=null) :base(name)
        {
            _action = action;
            _profile = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(name);
            _profile2 = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(name+"_all");
            _logger =
                new LoggerAdapter(Name);
            if (queue == null)
            {
                _queue = new BlockingQueue<Job>(32);
            }
            else
            {
                _queue = queue;
            }
        }

        public void Offer(Job job)
        {
            _queue.AddRef();
            _queue.Enqueue(job);
            
        }

        public bool IsDone()
        {
            return _queue.Count == 0 && _queue.Ref == 0;
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
                    try
                    {
                        _profile.BeginProfileOnlyEnableProfile();
                        _action(task);
                       // _profile2.BeginProfileOnlyEnableProfile();
                    }
                    finally
                    {
                        _queue.DelRef();
                        _profile.EndProfileOnlyEnableProfile();
                       // _profile2.EndProfileOnlyEnableProfile();
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