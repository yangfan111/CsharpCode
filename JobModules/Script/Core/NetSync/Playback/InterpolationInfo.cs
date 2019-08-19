using Core.Replicaton;
using Core.Utils;
using UnityEngine;

namespace Core.Playback
{
    public class InterpolationInfo : IInterpolationInfo
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(InterpolationInfo));
        private SnapshotPair _snapshotPair;
        public InterpolationInfo(SnapshotPair snapshotPair)
        {
            _snapshotPair = snapshotPair;
        }

        public int LeftServerTime
        {
            get { return _snapshotPair.LeftSnapshot.ServerTime; }
        }

        public int RightServerTime
        {
            get { return _snapshotPair.RightSnapshot.ServerTime; }
        }

        public int CurrentRenderTime
        {
            get { return _snapshotPair.RenderTime; }
        }

        public float Ratio
        {
            get
            {
                var left = _snapshotPair.LeftSnapshot.ServerTime;
                var right = _snapshotPair.RightSnapshot.ServerTime;
                var current = _snapshotPair.RenderTime;
                if (right > left)
                {
                    float ratio = 1.0f * (current - left) / (right - left);
                    return Mathf.Clamp01(ratio);
                }
                return 0;

            }
        }
        public float RatioWithOutClamp
        {
            get
            {
                var left = _snapshotPair.LeftSnapshot.ServerTime;
                var right = _snapshotPair.RightSnapshot.ServerTime;
                var current = _snapshotPair.RenderTime;
                if (right > left)
                {
                    return 1.0f * (current - left) / (right - left);
                    
                }
                return 0;


            }
        }

    }
}