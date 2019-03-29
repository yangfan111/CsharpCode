using Core.GameModule.Interface;
using Entitas;

namespace App.Client.GameModules.Sound
{
    public class SoundLimitSystem : IPlaybackSystem
    {
        private IGroup<SoundEntity> _playOnceSoundGroup;
        private const int Limit = 30;
        public SoundLimitSystem(SoundContext soundContext)
        {
            _playOnceSoundGroup = soundContext.GetGroup(SoundMatcher.PlayOnce);
        }

        public void OnPlayback()
        {
            if(_playOnceSoundGroup.count > Limit)
            {
                var i = 0;
                foreach(var sound in _playOnceSoundGroup)
                {
                    if(i >= Limit)
                    {
                        sound.isFlagDestroy = true;
                    }
                    i++;
                }
            }
        }
    }
}