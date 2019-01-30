
    public class AudioTrigger_SceneLoad : AudioTriggerBase
    {
        
        public void OnSceneLoad(  Core.Audio.AudioTriggerArgs args)
        {
            if (triggerDelegate != null )
            {
                triggerDelegate(args);
            }
        }

    }
