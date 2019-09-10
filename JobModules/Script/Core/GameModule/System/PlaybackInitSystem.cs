using Core.Playback;
using Core.SessionState;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class PlaybackInitSystem : AbstractStepExecuteSystem
    {
        private PlaybackManager _playbackManager;

        public PlaybackInitSystem(PlaybackManager playbackManager)
        {
            _playbackManager = playbackManager;
            
        }


        protected override void InternalExecute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit);
                _playbackManager.DoPlaybackInit();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit);
            }
        }

       

      
    }
}
