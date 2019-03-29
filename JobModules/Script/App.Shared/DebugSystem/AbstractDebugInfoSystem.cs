using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Core.GameModule.Interface;

namespace App.Shared.DebugSystem
{
    public abstract class AbstractDebugInfoSystem<T, TInfo> : IGamePlaySystem where T: AbstractDebugInfoSystem<T, TInfo>
    {
        private static int _prevStartKey;
        private static int _startKey;

        private static TInfo _DebugInfo;
        private static object _param;

        private static void Start()
        {

            Interlocked.Add(ref _startKey, 1);
        }

        private static bool Ready
        {
            get { return _prevStartKey == _startKey; }
        }

        public void OnGamePlay()
        {
            if (_prevStartKey != _startKey)
            {
                _DebugInfo = GetDebugInfo(_param);
                _prevStartKey = _startKey;
            }
        }

        protected abstract TInfo GetDebugInfo(object param);

        public static TInfo GetDebugInfoOnBlock(object param = null)
        {
            _param = param;
            Start();

            int sleepCount = 0;
            while (!Ready)
            {
                Thread.Sleep(1000);
                sleepCount++;
                if (sleepCount > 60)
                {
                    return default(TInfo);
                }
            }

            return _DebugInfo;
        }
    }
}
