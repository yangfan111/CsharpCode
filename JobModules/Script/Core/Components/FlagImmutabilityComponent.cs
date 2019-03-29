using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas;

namespace Core.Components
{
    
    public class FlagImmutabilityComponent : INonSelfLatestComponent, IResetableComponent
    {
        [NetworkProperty] public int LastModifyServerTime;
        public bool NeedSkipPlayBack { get; private set; }
        public bool NeedSkipSyncLatest { get; private set; }
        public bool NeedSkipUpdate
        {
            get { return NeedSkipPlayBack && NeedSkipSyncLatest; }
        }
        public int InitTime { get; private set; }
        public const int MinLastModifyOffset = 100;
        public const int ChangeInitTime = 1000;
        public static bool DisableImmutability;
        private int _lastSkipPlaybackTime;
        private int _lastSkipSyncLatestTime;
        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as FlagImmutabilityComponent;
            LastModifyServerTime = right.LastModifyServerTime;
        }

        public int GetComponentId()
        {
            return (int) ECoreComponentIds.FlagImmutability;
        }

        public bool JudgeNeedSkipPlayback(int leftServerTime, bool isPlayback = false)
        {
            //开关&&保证1秒内不会进入状态，判断差值左值在范围内，判断改次修改有没有生效过
            if (!DisableImmutability && InitTime <= 0 &&  LastModifyServerTime + MinLastModifyOffset< leftServerTime && _lastSkipPlaybackTime ==LastModifyServerTime)
            {
                NeedSkipPlayBack = true;
            }
            else
            {
                if(isPlayback)
                    _lastSkipPlaybackTime = LastModifyServerTime;
                NeedSkipPlayBack = false;
            }

            return NeedSkipPlayBack;
        }

        public bool JudgeNeedSkipSyncLatest(int rightServerTime)
        {
            if (!DisableImmutability && InitTime <= 0 && LastModifyServerTime + MinLastModifyOffset < rightServerTime && _lastSkipSyncLatestTime == LastModifyServerTime)
            {
                NeedSkipSyncLatest = true;
            }
            else
            {
                _lastSkipSyncLatestTime = LastModifyServerTime;
                NeedSkipSyncLatest = false;
            }

            return NeedSkipSyncLatest;
        }

        public void Reset()
        {
            InitTime = ChangeInitTime;
            _lastSkipSyncLatestTime = -1;
            _lastSkipPlaybackTime = -1;

        }

        public void Tick(int interval)
        {
            if (InitTime > 0)
                InitTime -= interval;
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}