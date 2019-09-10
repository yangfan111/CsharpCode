using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.GameTime
{
    public class TimeManager : ITimeManager
    {
        private const int MaxHistory = 60;
        private static LoggerAdapter logger = new LoggerAdapter(typeof(TimeManager));

        private readonly CompensationFixTimer fixTimer = new CompensationFixTimer();
        private readonly ICurrentTime sessionTimeComponent;


        private float compensationDeltaInterval;
        private DateTime _lastFrameTime = DateTime.MinValue;
        private volatile int lastScDelta;
        private bool firstDelta = true;
        List<int> interpolateIntervals = new List<int>(MaxHistory);
        public int LastAvgInterpolateInterval = TimeConstant.InterpolateInterval;
        private int latestServerTime;

        private int sumIntverval;

        public TimeManager(ICurrentTime sessionTimeComponent)
        {
            this.sessionTimeComponent = sessionTimeComponent;
        }

        public int ClientTime { get; private set; }


        public int FrameInterval { get; private set; }

        public int RenderTime { get; set; }

        public float FrameInterpolation { get; private set; }

   
        public void Tick(float now)
        {
            FrameInterval = fixTimer.Update(now);
            IncrClientTime(FrameInterval);
            UpdateRenderTime();
            sessionTimeComponent.CurrentTime = RenderTime;
        }


        public void SyncWithServer(int serverTime)
        {
            if (latestServerTime > serverTime)
            {
                logger.InfoFormat("sync server time invalid now {0} recv:{1}", latestServerTime, serverTime);
                return;
            }


            CheckInterpolation(serverTime - latestServerTime);
            latestServerTime = serverTime;

            var newScDelta = serverTime - ClientTime;
            var absDeltaInterval  = Math.Abs(newScDelta - lastScDelta);
            var deltaInterval = newScDelta - lastScDelta;
            var realDelta = 0;

            logger.DebugFormat("sync server time invalid now {0} {1}", absDeltaInterval, lastScDelta);
            if (firstDelta)
            {
                firstDelta       = false;
                lastScDelta = absDeltaInterval;
                logger.InfoFormat("sync server time (first time) serverTime {0} delta {1} client {2}, deltaDelta {3}",
                    serverTime, lastScDelta, ClientTime, absDeltaInterval);
                return;
            }
            // 变化太大了 >500ms    realDelta =  deltaInterval / 4;
            if (absDeltaInterval > TimeConstant.ResetTime) 
            {
                logger.InfoFormat(
                    "sync server time (delta invalid) serverTime {0} delta {1} client {2}, deltaDelta {3}",
                    serverTime, lastScDelta, ClientTime, absDeltaInterval);
                realDelta =  deltaInterval / 4;
            }
            //时间超前: >100ms时处理   realDelta= deltaInterval/ 8;
            else if (absDeltaInterval > 100 && newScDelta < lastScDelta) //超前时不马上设置到该值，而是通过下面的函数，递进的和增加
            {
                realDelta= deltaInterval/ 8;
                logger.InfoFormat(
                    "sync server time (delta too large) serverTime {0} client {1} delta {2} newDelta{3}  deltaDelta {4} ",
                    serverTime, ClientTime, lastScDelta, absDeltaInterval, absDeltaInterval - lastScDelta);
            }
            //数据滞后:>40ms时处理 以absDeltaInterval/s缓慢增加
            else if (absDeltaInterval > 40 && newScDelta > lastScDelta)
            {
                if (newScDelta > lastScDelta)
                    compensationDeltaInterval += TimeConstant.CompensationDeltaDelta * absDeltaInterval;
                else
                    compensationDeltaInterval -= TimeConstant.CompensationDeltaDelta * absDeltaInterval;

                if (compensationDeltaInterval > 1)
                {
                    lastScDelta        += 1;
                    compensationDeltaInterval -= 1;
                }
                else if (compensationDeltaInterval < -1)
                {
                    lastScDelta        -= 1;
                    compensationDeltaInterval += 1;
                }
            }

            lastScDelta += realDelta;
            SingletonManager.Get<DurationHelp>().LastAvgInterpolateInterval = LastAvgInterpolateInterval;
            SingletonManager.Get<DurationHelp>().ServerClientDelta          = lastScDelta;
            SingletonManager.Get<DurationHelp>().LastServerTime             = serverTime;
            SingletonManager.Get<DurationHelp>().RenderTime                 = RenderTime;
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

        private void UpdateRenderTime()
        {
            //确定当前渲染时间
            var newRenderTime = ClientTime + lastScDelta - LastAvgInterpolateInterval - TimeConstant.TimeNudge;
            RenderTime = newRenderTime < RenderTime ? RenderTime : newRenderTime;
        }

        private void IncrClientTime(int frameInterval)
        {
            ClientTime += frameInterval;
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
                    logger.InfoFormat("CheckInterpolation {0} to {1}", LastAvgInterpolateInterval, avg);
                    LastAvgInterpolateInterval = avg;
                }
            }
        }
    }
}