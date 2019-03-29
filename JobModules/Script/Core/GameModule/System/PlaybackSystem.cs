using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.Utils;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class PlaybackSystem : AbstractFrameworkSystem<IPlaybackSystem>
    {
        private IList<IPlaybackSystem> _systems;
        public PlaybackSystem(IGameModule module)
        {
            
            _systems = module.PlaybackSystems;

            Init();
        }


        public override IList<IPlaybackSystem> Systems { get { return _systems; } }

        public override void SingleExecute(IPlaybackSystem module)
        {
            module.OnPlayback();
        }

        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Playback);
                base.Execute();
            }
            finally {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Playback);
            }
           
        }
        
    }
}
