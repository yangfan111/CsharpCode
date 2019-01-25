using System;

namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public class SimulationTimeSyncInfo
    {
        private static DateTime BaseTime;
        public volatile bool IsWaitingSync;
        public volatile int LastSyncTimeDelta;

        
        public volatile int FirstRecvServerTime;
        private volatile float _firstSendTime;
        private volatile float _firstRecvTime;

        public SimulationTimeSyncInfo()
        {
            BaseTime = DateTime.Now;;
            IsWaitingSync = false;
            LastSyncTimeDelta = 0;

            FirstRecvServerTime = 0;
            _firstSendTime = 0.0f;
            _firstRecvTime = 0.0f;
        }

        public void SetFirstSendTime(DateTime sendTime)
        {
            _firstSendTime = (float) (sendTime - BaseTime).TotalMilliseconds;
        }

        public void SetFirstRecvTime(DateTime recvTime)
        {
            _firstRecvTime = (float)(recvTime - BaseTime).TotalMilliseconds;
        }

        public float GetFirstDelayTime()
        {
            return _firstRecvTime - _firstSendTime;
        }
    }
}
