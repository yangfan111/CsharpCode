using System;
using System.Diagnostics;
using System.Threading;
using Core.Utils;

namespace Utils.Network
{
    public class PollServiceThread : AbstractThread
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PollServiceThread));
        private float _rate;

        private Action<bool> _poolFunc;
        private int _pollIntval;
        private double _runningTime;
        private double _totalTime;
       
        private Stopwatch _stopwatch = new Stopwatch();
      
        public PollServiceThread(Action<bool> poolFunc, int pollIntval, string name) : base(name)
        {
            _poolFunc = poolFunc;
            _pollIntval = pollIntval;
        }

        protected override void Run()
        {
            while (Running)
            {
                try
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _poolFunc(true);
                    _stopwatch.Stop();
                    var span = _stopwatch.ElapsedTicks / 10000f;
                    var sleep = (int) (_pollIntval - span);
                    _runningTime += span;
                    _stopwatch.Start();
                    if (sleep >= 0 && sleep < _pollIntval)
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
                    _logger.ErrorFormat("Thread {0} error {1}", Name, e);
                }
            }
        }

        public override float Rate
        {
            get { return _rate; }
        }
    }
}