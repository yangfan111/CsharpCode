using System;
using System.Diagnostics;
using UnityEngine;

namespace Lib.lib
{
    public class TimeUtil
    {
        protected static Stopwatch stopwatch;

        public static TimeSpan Tick(Action action)
        {
            stopwatch.Reset();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static bool FrameCountExec(int frameCountStep)
        {
            if (Time.frameCount % frameCountStep == 0)
            {
                return true;
            }
            return false;
        }
      
    }
}