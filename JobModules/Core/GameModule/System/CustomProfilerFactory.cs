#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Core.Utils;
using Entitas.VisualDebugging.Unity;

namespace Core.GameModule.System
{

    public class CustomProfiler : ICustomProfiler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CustomProfiler));

        private Feature _owner;
        private Dictionary<string, SystemInfo> _systemInfos = new Dictionary<string, SystemInfo>();
        private Dictionary<string, double> _executeDurationBuffer = new Dictionary<string, double>();

        private Stopwatch _stopwatch;

        public CustomProfiler(Feature owner)
        {
            _owner = owner;
            _stopwatch = new Stopwatch();
        }

        public void Start(string name)
        {
            if (!_systemInfos.ContainsKey(name))
            {
                Initialize(name);
            }

            StartWatch();
        }

        public void Pause(string name)
        {
            if (CheckInitialized(name))
            {
                AddDuration(name);
            }
        }

        
        public void Stop(string name)
        {
            if (CheckInitialized(name))
            {
                AddDuration(name);
                FlushDuration(name);
            }
        }


        private void StartWatch()
        {
            if (_stopwatch.IsRunning)
            {
                StopWatch();
            }

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        private double StopWatch()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                return _stopwatch.Elapsed.TotalMilliseconds; ;
            }

            return 0;
        }

        private void Initialize(string name)
        {
            var sysInfo = new SystemInfo(new FakeExecuteSystem(name));
            _owner.AddSystemInfo(sysInfo);
            _systemInfos[name] = sysInfo;
            _executeDurationBuffer[name] = 0.0f;
        }

        private bool CheckInitialized(string name)
        {
            if (!_systemInfos.ContainsKey(name))
            {
                _logger.WarnFormat("The profiler for {0} is not initialized, please call procedure 'Start()' first!", name);
                return false;
            }

            return true;
        }

        private void AddDuration(string name)
        {
            var duration = StopWatch();
            _executeDurationBuffer[name] += duration;
        }

        private void FlushDuration(string name)
        {
            var duration = _executeDurationBuffer[name];
            _executeDurationBuffer[name] = 0;

            var sysInfo = _systemInfos[name];
            sysInfo.AddExecutionDuration(duration);
        } 
    }

    public static class CustomProfilerFactory
    {
        public static ICustomProfiler CreateProfiler<T>(AbstractFrameworkSystem<T> owner)
        {
            return new CustomProfiler(owner);
        }
    }
}
#else
namespace Core.GameModule.System
{

    public class CustomProfiler : ICustomProfiler
    {
        public void Pause(string name)
        {
           
        }

        public void Start(string name)
        {
            
        }

        public void Stop(string name)
        {
            
        }
    }

    public static class CustomProfilerFactory
    {
        public static ICustomProfiler CreateProfiler<T>(AbstractFrameworkSystem<T> owner)
        {
            return new CustomProfiler();
        }
    }
}

#endif