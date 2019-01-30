
    public class AudioTrigger_GunSimple : AudioTriggerBase
    {
        
        public void OnGunSimple(  Core.Audio.AudioTriggerArgs args)
        {
            if (triggerDelegate != null )
            {
                triggerDelegate(args);
            }
        }

    }
