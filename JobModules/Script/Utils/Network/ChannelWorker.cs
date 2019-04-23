using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.Utils;

namespace Core.Network
{
    public abstract class ChannelWorker : AbstractThread
    {
	    private static LoggerAdapter _logger = new LoggerAdapter(typeof(ChannelWorker));
        public static bool IsSuspend = false;
        protected ChannelWorker(ReadWriteList<AbstractNetowrkChannel> holder, int threadIdx, int threadCount, string name = "default"): base(name)
        {
            _holder = holder;
            _threadIdx = threadIdx;
            _threadCount = threadCount;
        }

       
        private double _runningTime;
        private double _totalTime;
        private readonly ReadWriteList<AbstractNetowrkChannel> _holder;
        private readonly int _threadIdx;
        private readonly int _threadCount; 
        private Stopwatch _stopwatch = new Stopwatch();
        private float _rate;
        private int _pollIntval=2;
        protected override void Run()
        {
	        Thread.CurrentThread.Name = GetType().ToString();
            while (Running)
            {
                try
                {
                    var copy = _holder.ForRead().FindAll(x => x.Id % _threadCount == _threadIdx);

                  
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    bool hasWork = false;
                    if (!IsSuspend)
                    {
                        hasWork = DoProcesssAll();
                        foreach (var channel in copy)
                        {
                            hasWork = DoProcessChannel(channel) || hasWork;
                        }
                    }

                    _stopwatch.Stop();
                    var span = _stopwatch.ElapsedTicks / 10000f;
                    var sleep = (int) (_pollIntval - span);
                    _runningTime += span;
                    _stopwatch.Start();
                    if (!hasWork && sleep >= 0 && sleep < _pollIntval)
                    {
                        Thread.Sleep(sleep);
                    }
                    _stopwatch.Stop();
                    _totalTime += _stopwatch.ElapsedTicks / 10000f;
                    if (_totalTime >= 5000)
                    {
                        
                        _rate = (float) (100 * _runningTime / (_totalTime));
                        _totalTime = 0;
                        _runningTime = 0;
                    }
                    
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Thread {0} error {1}",Name, e);
                }

            }
        }

        public override float Rate
        {
            get { return _rate; }
        }

        protected abstract bool DoProcessChannel(AbstractNetowrkChannel channel);

        protected virtual bool DoProcesssAll()
        {
            return false;
        }
    }
}