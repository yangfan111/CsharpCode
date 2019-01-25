using Core.Playback;
using Core.SyncLatest;

namespace Core.Components
{
    
    public class FlagSyncNonSelfComponent :  ISelfLatestComponent, INonSelfLatestComponent
    {
        public void CopyFrom(object rightComponent)
        {
        }

       

        public int GetComponentId() { { return (int)ECoreComponentIds.FlagSyncSelf; } }
        public void SyncLatestFrom(object rightComponent)
        {
            
        }
    }

   
}