using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Utils
{
    public class ThreadInfo
    {
        public string Name;
        public Type Type;

        public ThreadState State;
        public float Rate;
    }

    public class ThreadStatistics
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(ThreadStatistics));
        private HashSet<AbstractThread> _runningThreads  = new HashSet<AbstractThread>();
        private HashSet<AbstractThread> _createdThreads = new HashSet<AbstractThread>();

        public void OnThreadCreate(AbstractThread thread)
        {
            _createdThreads.Add(thread);
        }

        public void OnThreadStart(AbstractThread thread)
        {
            lock (this)
            {
                _runningThreads.Add(thread);
            }
        }

        public void OnThreadStop(AbstractThread thread)
        {
            lock (this)
            {
                _runningThreads.Remove(thread);
            }
        }

        public void OnThreadDispose(AbstractThread thread)
        {
            lock (this)
            {
                _createdThreads.Remove(thread);
            }
        }

        public void Foreach(Action<AbstractThread> doAction)
        {
            lock (this)
            {
                var threadList = new List<AbstractThread>(_createdThreads);
                foreach (var thread in threadList)
                {
                    try
                    {
                        doAction(thread);
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("Thread  {0} {1} Error {2} Erorr", thread.Name,  thread.GetType());
                        _logger.Error("Error:", e);
                    }
                }
            }
        }

        public List<ThreadInfo> AllThreadInfos
        { 
            get { return GetThreadInfosFrom(_createdThreads); }
        }

        private List<ThreadInfo> GetThreadInfosFrom(HashSet<AbstractThread> set)
        {
            var infos = new List<ThreadInfo>();
            lock (this)
            {
                foreach (var thread in set)
                {
                    infos.Add(new ThreadInfo()
                    {
                        Name = thread.Name,
                        Type = thread.GetType(),

                        State = thread.State,
                        Rate = thread.Rate
                    });
                }
            }

            return infos;
        }
    }

    public abstract class AbstractThread : IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractThread));

        public static ThreadStatistics Statistics  = new ThreadStatistics();

        private Thread _thread;
        protected bool Running;
        public bool Disposed;

        public string Name { get; private set; }

        public AbstractThread(string name)
        {
            Name = name;
            Statistics.OnThreadCreate(this);
        }
        

        public virtual  void Start()
        {
            if (!Disposed && !Running)
            {
                Running = true;
                _thread = new Thread(RunWrapper){IsBackground = true};
                _thread.Start();

                Statistics.OnThreadStart(this);
            }
        }
        

        public virtual  void Stop()
        {
            Statistics.OnThreadStop(this);
            if (Running)
            {
                Running = false;
//                _thread.Interrupt();
                _thread.Join();
            }
        }

        public void Dispose()
        {
            Disposed = true;
            Statistics.OnThreadDispose(this);
            Stop();
        }

        public void RunWrapper()
        {
            _logger.InfoFormat("Thread Started {1} {0} {2}", Name, GetType(), Thread.CurrentThread.ManagedThreadId);
            Run();
            _logger.InfoFormat("Thread Exiting {1} {0} {2}", Name, GetType(), Thread.CurrentThread.ManagedThreadId);
        }
        protected abstract void Run();
        public abstract float Rate { get; }
        public ThreadState State
        {
            get { return _thread.ThreadState; }
        }
    }
}