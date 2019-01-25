using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Shared.Components.ServerSession
{
    [Serializable]
    public class FpsSatatus
    {
        [Serializable]
        public class FpsCalc
        {
            public int Count;
            public float Total;
            public int Fps;
            public float NextTime;
            public int IntvalTime;

            public void Tick(float time, float deltaTime)
            {
                Total += deltaTime;
                Count++;
                if (NextTime < time)
                {
                    Fps = (int) (Count / Total);
                    NextTime = time + IntvalTime;
                    Count = 0;
                    Total = 0;
                }
            }
        }

        private FpsCalc _fps5 = new FpsCalc {IntvalTime = 5};
        private FpsCalc _fps30 = new FpsCalc {IntvalTime = 30};
        private FpsCalc _fps60 = new FpsCalc {IntvalTime = 60};

        private float _totalDelta;
        private int _count;

        private float _tempMaxDleta;
        private float _nextTime;
        private float _avgDelta;
        private float _maxDelta;
        public int GcCount = 0;

        public int AvgDelta
        {
            get { return (int) (_avgDelta * 1000); }
        }

        public int MaxDelta
        {
            get { return (int) (_maxDelta * 1000); }
        }

        public int Fps5
        {
            get { return _fps5.Fps; }
        }

        public int Fps30
        {
            get { return _fps30.Fps; }
        }

        public int Fps60
        {
            get { return _fps60.Fps; }
        }
       

        private static float ComputeVariance(List<float> a)
        {
            float variance = 0; //方差
            float sum = 0, sum2 = 0;
            int i = 0, len = a.Count;
            for (; i < len; i++)
            {
                sum += a[i];
                sum2 += a[i] * a[i];
            }

            variance = sum2 / len - (sum / len) * (sum / len);
            return variance;
        }

        public void Tick(float time, float delta)
        {
            _fps5.Tick(time, delta);
            _fps30.Tick(time, delta);
            _fps60.Tick(time, delta);
            _totalDelta += delta;
            _count++;
         
            if (_tempMaxDleta < delta)
                _tempMaxDleta = delta;
            if (_nextTime < time)
            {
                _maxDelta = _tempMaxDleta;
                _avgDelta = _totalDelta / _count;
             
                _totalDelta = 0;
                _count = 0;
                _nextTime = time + 5;
                _tempMaxDleta = 0;
            }

            GcCount =System.GC.CollectionCount(0) +System.GC.CollectionCount(1)+System.GC.CollectionCount(2);;
        }

        public override string ToString()
        {
            return string.Format("ad:{0}, md: {1}, fps: {2} {3} {4} GC:{5}", AvgDelta, MaxDelta, Fps5, Fps30, Fps60,
                GcCount);
        }
    }
}