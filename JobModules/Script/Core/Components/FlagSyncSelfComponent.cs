using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SyncLatest;

namespace Core.Components
{
    
    public class FlagSyncSelfComponent :   ISelfLatestComponent, INonSelfLatestComponent
    {
        public void CopyFrom(object rightComponent)
        {
        }

       

       
        public int GetComponentId() { { return (int)ECoreComponentIds.FlagSyncNonSelf; } }
        public void SyncLatestFrom(object rightComponent)
        {
            
        }
    }
}