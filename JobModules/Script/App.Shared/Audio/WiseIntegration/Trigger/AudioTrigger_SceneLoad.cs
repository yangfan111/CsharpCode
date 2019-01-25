
    public class AudioTrigger_SceneLoad : AudioTriggerBase
    {
        
        public void OnSceneLoad(  App.Shared.Audio.AudioTriggerArgs args)
        {
            if (triggerDelegate != null )
            {
                triggerDelegate(args);
            }
        }

    }
