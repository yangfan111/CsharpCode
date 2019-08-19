using System.Collections.Generic;
using System.Text;
using Core.Utils;
using UnityEngine.Profiling;
using Utils.Singleton;

namespace Utils.StatsMonitor.Scripts.GPU
{
    public class GPUProfilerManager : DisposableSingleton<GPUProfilerManager>
    { 
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(GPUProfilerManager));

        private static List<GPUProfiler> _profilers = new List<GPUProfiler>();
        private static GPUProfiler _mainThreadProfiler = new GPUProfiler("MainThreadTime");
        private static GPUProfiler _renderThreadProfiler = new GPUProfiler("RenderThreadTime");
        private static GPUProfiler _gpuProfiler = new GPUProfiler("GPUTime");
#if HAVE_FAST_GPU_PROFILER
        private List<GpuTimerQuerySample> _samples = new List<GpuTimerQuerySample>();
#endif

        private static Dictionary<string, List<string>> _stackLevelNameBuffer = new Dictionary<string, List<string>>();

        private static bool _initialized = false;

        private void Initialize()
        {
            if (!_initialized)
            {
                RegisterProfiler(_mainThreadProfiler);
                RegisterProfiler(_renderThreadProfiler);
                RegisterProfiler(_gpuProfiler);

                _initialized = true;
            }
        }

        protected override void OnDispose()
        {
            Cleanup();
        }

        protected void Cleanup()
        {
            if (_initialized)
            {
                UnregisterProfiler(_mainThreadProfiler);
                UnregisterProfiler(_renderThreadProfiler);
                UnregisterProfiler(_gpuProfiler);

                foreach (var profiler in _profilers)
                {
                    UnregisterProfiler(profiler);
                }
                _profilers.Clear();

                _initialized = false;
            }
        }

        private void RegisterProfiler(GPUProfiler profiler)
        {
            if (!profiler.IsRegistered)
            {
                global::StatsMonitor.StatsMonitor.Instance.RegistProfiler(profiler);
                profiler.IsRegistered = true;
            }
        }

        private void UnregisterProfiler(GPUProfiler profiler)
        {
            if (profiler.IsRegistered)
            {
                global::StatsMonitor.StatsMonitor.Instance.UnRegistProfiler(profiler);
                profiler.IsRegistered = false;
            }
        }

        private string GetStackLevelName(string name, int stackLevel)
        {
            const string Prefix = "+";

            List<string> stackLevelNames = null;
            if (!_stackLevelNameBuffer.TryGetValue(name, out stackLevelNames))
            {
                stackLevelNames = new List<string>();
                _stackLevelNameBuffer.Add(name, stackLevelNames);
            }

            while (stackLevelNames.Count < stackLevel + 1)
            {
                stackLevelNames.Add(null);
            }

            var stackLevelName = stackLevelNames[stackLevel];
            if (stackLevelName == null)
            {
                var sb = new StringBuilder();

                for (int i = 0; i < stackLevel; ++i)
                {
                    sb.Append(Prefix);
                }
                sb.Append(name);
                stackLevelName = sb.ToString();
                stackLevelNames[stackLevel] = stackLevelName;
            }

            return stackLevelName;
        }

        public void Update()
        {
#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
            if (!Profiler.profileGPUFast)
            {
                Cleanup();
                return;
            }

            Initialize();

            _mainThreadProfiler.SetData(Profiler.MainThreadFrameTime);
            _renderThreadProfiler.SetData(Profiler.RenderThreadFrameTime);
            _gpuProfiler.SetData(Profiler.FastGPUTime);

            _samples.Clear();

            if (Profiler.GetFastGpuProfilerSamples(_samples))
            {
                bool reset = false;
                if (_samples.Count != _profilers.Count)
                {
                    foreach (var profiler in _profilers)
                    {
                        UnregisterProfiler(profiler);
                    }
                    _profilers.Clear();

                    reset = true;
                }

                for (int i = 0; i < _samples.Count; ++i)
                {
                    var sample = _samples[i];
                    var name = GetStackLevelName(sample.StatName, sample.StackLevel);

                    GPUProfiler profiler = null;
                    if (reset)
                    {
                        profiler = new GPUProfiler(name);
                        _profilers.Add(profiler);
                        RegisterProfiler(profiler);
                    }

                    profiler = _profilers[i];
                    profiler.Name = name;
                    profiler.SetData(sample.ElapsedTime);
                }
            }
#endif
        }
    }
}
