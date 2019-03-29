using System;
using System.Collections.Generic;
using System.Threading;
using Core.Utils;
using Utils.Singleton;

namespace Core.ThreadUtils
{
    public class Task<R, T>
    {
        public T Param { get; private set; }

        public R Ret
        {
            get { return _ret; }
            set { _ret = value; }
        }

        private R _ret = default(R);

        public Task(T param)
        {
            Param = param;
            Ret = default(R);
        }

        public bool IsComplete;
    }

    public class Job<R, T>
    {
        public List<Task<R, T>> Tasks;
        public int From;
        public int To;
        public Exception Exception;
        public string Name = "DefaultJob";
        public void Set(List<Task<R, T>> tasks, int from, int to)
        {
            if (tasks != null) Tasks = tasks;
            From = from;
            To = to;
        }
    }

    public  class MutilExecute<R, T>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(MutilExecute<R,T>));
        private int _threads;
        private List<Task<R, T>> _tasks = new List<Task<R, T>>();
        private volatile int _threadsRunning;
        private Job<R, T>[] _jobs;
        private  Func<T,R> _func;
        
        public MutilExecute(int threadCount, List<T> tasks, Func<T,R> func,string name = "MutilExecute_default")
        {
            _threads = threadCount;
            foreach (var task in tasks)
            {
                _tasks.Add(new Task<R, T>(task));
            }
           
            _jobs = new Job<R, T>[threadCount];
            for (int i = 0; i < _jobs.Length; i++) {
                _jobs[i] = new Job<R,T>();
                _jobs[i].Name = string.Format("{0}_{1}", name, i);
            }
            _func = func;
        }

        public void Start()
        {
            _threadsRunning = _threads;
            var count = _tasks.Count;
            var remainder = count % _threads;
            var slice = count / _threads + (remainder == 0 ? 0 : 1);
            for (int t = 0; t < _threads; t++)
            {
                var from = t * slice;
                var to = from + slice;
                if (to > count)
                {
                    to = count;
                }

                var job = _jobs[t];
                job.Set(_tasks, from, to);
                if (from != to)
                {
                    ThreadPool.QueueUserWorkItem(queueOnThread, _jobs[t]);
                }
                else
                {
                    Interlocked.Decrement(ref _threadsRunning);
                }
            }
        }

        

        public bool IsDone()
        {
            return _threadsRunning == 0;
        }

        public int ThreadsRunning
        {
            get { return _threadsRunning; }
        }

        public void WaitForComplete()
        {
            while (_threadsRunning != 0)
            {
            }
        }

        public Exception GetException()
        {
            foreach (var job in _jobs)
            {
                if (job.Exception != null)
                {
                    return job.Exception;
                }
            }

            return null;
        }

        void queueOnThread(object state)
        {
         
            var job = (Job<R, T>) state;
            var profile = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(job.Name);
            try
            {
                for (int i = job.From; i < job.To; i++)
                {
                    try
                    {
                        SingletonManager.Get<DurationHelp>().ProfileStart(profile);
                        job.Tasks[i].Ret = _func(job.Tasks[i].Param);
                        job.Tasks[i].IsComplete = true;
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("queueOnThread:{0}",e);
                    }
                    finally
                    {
                        SingletonManager.Get<DurationHelp>().ProfileEnd(profile);
                    }
                }
            }
            catch (Exception ex)
            {
                job.Exception = ex;
            }
            finally
            {
                Interlocked.Decrement(ref _threadsRunning);
            }
        }

        public void DoActionInResult(Action<T, R> action)
        {
            foreach (var task in _tasks)
            {
                action(task.Param, task.Ret);
            }
        }
    }
}