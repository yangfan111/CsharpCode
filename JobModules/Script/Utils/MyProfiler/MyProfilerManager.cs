using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.MyProfiler
{
    class ProfilerInfo
    {
        public int Id;
        public int Interval;
        public int Times;
        public int GC;

        public ProfilerInfo(int id, int interval, int times, int gc)
        {
            Id = id;
            Interval = interval;
            Times = times;
            GC = gc;
        }

        public ProfilerInfo(int id)
        {
            Id = id;
        }

        public void Clean()
        {
            Interval = 0;
            Times = 0;
            GC = 0;
        }

        public void Record(int interval, int times, int gc)
        {
            Interval += interval;
            Times += times;
            GC += gc;
        }
    }

    public class MyProfilerManager : Singleton<MyProfilerManager>
    {
        private static LoggerAdapter _logger = new LoggerAdapter("voyager.profiler");

        public MyProfilerManager()
        {
            for (int i = 0; i < 256; i++)
            {
                _infos[i] = new ProfilerInfo(i);
            }
        }

        private Dictionary<string, int> _durationInfoNameToId = new Dictionary<string, int>(256);
        private ProfilerInfo[] _infos = new ProfilerInfo[256];
        private object _lock = new object();
        private int _maxId;
        public bool IsRecordOn = false;
        public void Add(string name)
        {
            if (!_durationInfoNameToId.ContainsKey(name))
            {
                lock (_lock)
                {
                    if (!_durationInfoNameToId.ContainsKey(name))
                    {
                        var index = _maxId++;
                        _durationInfoNameToId[name] = index;
                        _logger.InfoFormat("name:{0},id:{1}", name, index);
                    }
                }
            }
        }
        public void Record(string name, float d, int gc)
        {
           
            if (!IsRecordOn) return;
            int id = _durationInfoNameToId[name];
            _infos[id].Record((int) (d * 10), 1, gc);
        }

        StringBuilder sb = new StringBuilder(4096);
        private Dictionary<int, string> _intToStringCache = new Dictionary<int, string>(2018);

        private string IntToStringCache(int i)
        {
            if (_intToStringCache.ContainsKey(i)) return _intToStringCache[i];
            var s = i.ToString();
            _intToStringCache[i] = s;
            return s;
        }

        private int[] _entityCount = new int[0];
        public IContexts Contexts { get; set; }

        public int[] GetEntityCount()
        {
            if (Contexts != null)
            {
                int c = Contexts.allContexts.Length;
                if (_entityCount.Length != c) _entityCount = new int[c];
                for (int i = 0; i < c; i++)
                {
                    _entityCount[i] = Contexts.allContexts[i].count;
                }
            }

            return _entityCount;
        }

        public void RecordToLog(int seq)
        {
            if (!IsRecordOn) return;
            sb.Length = 0;
            sb.Append("{\"seq\":").Append(seq).Append(",").Append("\"entity\":[");
            var ecs = GetEntityCount();
            for (int i = 0; i < ecs.Length; i++)
            {
                sb.Append(IntToStringCache(ecs[i]));
                if (i < ecs.Length - 1)
                {
                    sb.Append(",");
                }
                else
                {
                    sb.Append("],");
                }
            }

            for (int i = 0; i < _maxId; i++)
            {
                if (_infos[i].Times > 0 && _infos[i].Interval > 0)
                {
                    sb.Append("\"").Append(IntToStringCache(i)).Append("\":[").Append(IntToStringCache(_infos[i].Times)).Append(",")
                        .Append(IntToStringCache(_infos[i].Interval));
                    if (_infos[i].GC > 0)
                        sb.Append(",").Append(IntToStringCache(_infos[i].GC));
                    sb.Append("],");
                            
                    _infos[i].Clean();
                }
            }

            sb.Remove(sb.Length - 1, 1);

            sb.Append("}");
            _logger.Debug(sb.ToString());
        }

        public void EnableProfiler()
        {
            IsRecordOn = true;
        }

        public void DisableProfiler()
        {
            IsRecordOn = false;
        }

       
    }
}