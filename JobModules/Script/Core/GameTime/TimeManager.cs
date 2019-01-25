using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Entitas.VisualDebugging.Unity;
using UnityEngine;
using Utils.Singleton;

namespace Core.GameTime
{
    public class TimeManager : ITimeManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(TimeManager));
        private DateTime _lastFrameTime = DateTime.MinValue;
        private volatile int _serverTimeDelta;
        private volatile int _clientTime;
        private bool _firstDelta = true;
        private int _serverTime;
        private readonly ICurrentTime _currentTime;
        public int ClientTime
        {
            get { return _clientTime; }
            set { _clientTime = value; }
        }

        public int FrameInterval { get; private set; }

        public int RenderTime { get; private set; }

        public float FrameInterpolation { get; private set; }
        public int LastAvgInterpolateInterval = TimeConstant.InterpolateInterval;
        /// <summary>
        /// delta = server - client
        /// </summary>
        public int Delta
        {
            get { return _serverTimeDelta; }
            set { _serverTimeDelta = value; }
        }

        CalcFixTimeInterval _calcFixTimeInterval = new CalcFixTimeInterval();

        public void Tick(float now)
        {
            FrameInterval = _calcFixTimeInterval.Update(now);
            IncrClientTime(FrameInterval);
            UpdateRenderTime();
            _currentTime.CurrentTime = RenderTime;
        }

        private void UpdateRenderTime()
        {
            //确定当前渲染时间
            var newRenderTime = ClientTime + _serverTimeDelta -  LastAvgInterpolateInterval-
                                TimeConstant.TimeNudge;
            RenderTime = newRenderTime < RenderTime ? RenderTime : newRenderTime;
        }


        public int ServerTime
        {
            get { return _serverTime; }
            set { _serverTime = value; }
        }

        private void IncrClientTime(int frameInterval)
        {
            _clientTime += frameInterval;
        }


        private float _compensationDeltaDelta = 0;
        private const int MaxHistory = 60;
        List<int> interpolateIntervals = new List<int>(MaxHistory);
        public void SyncWithServer(int latestServerTime)
        {
            if (_serverTime > latestServerTime)
            {
                _logger.InfoFormat("sync server time invalid now {0} recv:{1}", _serverTime, latestServerTime);
                return;
            }


         
            CheckInterpolation( latestServerTime - _serverTime);
            this._serverTime = latestServerTime;

            var newDelta = latestServerTime - ClientTime;
            var deltaDelta = System.Math.Abs(newDelta - _serverTimeDelta);
             _logger.DebugFormat("sync server time invalid now {0} {1}", newDelta, _serverTimeDelta);
            if (_firstDelta)
            {
                _firstDelta = false;
                _serverTimeDelta = newDelta;
                _logger.InfoFormat("sync server time (first time) serverTime {0} delta {1} client {2}, deltaDelta {3}",
                    latestServerTime, _serverTimeDelta, ClientTime, deltaDelta);
            }
            else
            {
                if (deltaDelta > TimeConstant.ResetTime) // 变化太大了
                {
                    _logger.InfoFormat(
                        "sync server time (delta invalid) serverTime {0} delta {1} client {2}, deltaDelta {3}",
                        latestServerTime, _serverTimeDelta, ClientTime, deltaDelta);
                    _serverTimeDelta += ((newDelta - _serverTimeDelta)/4);
                }
                else if (deltaDelta > 100 && newDelta < _serverTimeDelta) //超前时不马上设置到该值，而是通过下面的函数，递进的和增加
                {
                    var nextDelta =   _serverTimeDelta +( (newDelta - _serverTimeDelta)/8);
                    _logger.InfoFormat(
                        "sync server time (delta too large) serverTime {0} client {1} delta {2} newDelta{3}  deltaDelta {4} nextDelta{5}",
                        latestServerTime, ClientTime, _serverTimeDelta,  newDelta, newDelta - _serverTimeDelta, nextDelta);
                    _serverTimeDelta = nextDelta;
                }
                else if (deltaDelta > 40 && newDelta > _serverTimeDelta)
                {
                    if (newDelta > _serverTimeDelta)
                    {
                        _compensationDeltaDelta += TimeConstant.CompensationDeltaDelta * deltaDelta;
                    }
                    else
                    {
                        _compensationDeltaDelta -= TimeConstant.CompensationDeltaDelta * deltaDelta;
                    }

                    if (_compensationDeltaDelta > 1)
                    {
                        _serverTimeDelta += 1;
                        _compensationDeltaDelta -= 1;
                    }
                    else if (_compensationDeltaDelta < -1)
                    {
                        _serverTimeDelta -= 1;
                        _compensationDeltaDelta += 1;
                    }
                }

                SingletonManager.Get<DurationHelp>().LastAvgInterpolateInterval = LastAvgInterpolateInterval;
                SingletonManager.Get<DurationHelp>().ServerClientDelta = _serverTimeDelta;
                SingletonManager.Get<DurationHelp>().LastServerTime = latestServerTime;
                SingletonManager.Get<DurationHelp>().RenderTime = RenderTime;
            }
        }

        public void UpdateFrameInterpolation(int leftServerTime, int rightServerTime)
        {
            int delta = rightServerTime - leftServerTime;
            if (delta == 0)
            {
                FrameInterpolation = 0;
            }
            else
            {
                int deltaTime = RenderTime - leftServerTime;
                FrameInterpolation = (float) (deltaTime * 1.0 / delta);
            }
        }

        private int sumIntverval = 0;

        public TimeManager(ICurrentTime currentTime)
        {
            _currentTime = currentTime;
        }

        private void CheckInterpolation(int intverval)
        {
         
            if (interpolateIntervals.Count > MaxHistory)
            {
                interpolateIntervals.RemoveAt(0);
            }
            interpolateIntervals.Add(intverval);
            sumIntverval += intverval;
            if (sumIntverval > 1000 * 10)
            {
                sumIntverval = 0;

                int sum = 0;
                foreach (var interpolateInterval in interpolateIntervals)
                {
                    sum += interpolateInterval;
                }

                var avg = sum / interpolateIntervals.Count;
                if (Mathf.Abs(avg - LastAvgInterpolateInterval) > 10)
                {
                    _logger.InfoFormat("CheckInterpolation {0} to {1}", LastAvgInterpolateInterval, avg);
                    LastAvgInterpolateInterval = avg;
                    

                }
            }
        }
    }
}