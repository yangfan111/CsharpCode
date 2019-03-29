
    public class AudioTrigger_GunSimple : AudioTriggerBase
    {
        
        public void OnGunSimple(  App.Shared.Audio.AudioTriggerArgs args)
        {
            if (triggerDelegate != null )
            {
                triggerDelegate(args);
            }
        }

    }
