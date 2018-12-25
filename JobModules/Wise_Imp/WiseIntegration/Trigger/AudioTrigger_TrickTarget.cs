
////namespace Core.Audio { 
    public class AudioTrigger_ListenerTraceTarget : AudioTriggerBase
    {
        
        public void OnTraceTarget( Core.Audio.AudioTriggerArgs args)
        {
            if (triggerDelegate != null )
            {
                triggerDelegate(args);
            }
        }

    }
