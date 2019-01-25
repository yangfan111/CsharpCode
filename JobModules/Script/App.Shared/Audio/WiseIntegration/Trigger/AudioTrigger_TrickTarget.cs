
////namespace App.Shared.Audio { 
    public class AudioTrigger_ListenerTraceTarget : AudioTriggerBase
    {
        
        public void OnTraceTarget( App.Shared.Audio.AudioTriggerArgs args)
        {
            if (triggerDelegate != null )
            {
                triggerDelegate(args);
            }
        }

    }
