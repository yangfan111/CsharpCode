using Core.GameModule.Interface;
using Core.Sound;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Sound
{
    public class ClientSoundAutoStopSystem : IGamePlaySystem 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientSoundAutoStopSystem));
        private IGroup<SoundEntity> _playingSoundGroup;
        private ISoundPlayer _soundPlayer;
        private SoundParentController _soundParentController;
        public ClientSoundAutoStopSystem(SoundContext soundContext, ISoundPlayer soundPlayer, SoundParentController soundParentController)
        {
            _playingSoundGroup = soundContext.GetGroup(SoundMatcher.Playing);
            _soundPlayer = soundPlayer;
            _soundParentController = soundParentController;
        }

        public void OnGamePlay()
        {
            foreach(var soundEntity in _playingSoundGroup)
            {
                if(!soundEntity.hasAudioSourceKey || soundEntity.isFlagPreventDestroy)
                {
                    continue;
                }
                if (soundEntity.hasParent)
                {
                    _soundParentController.CleanUpWithParent(soundEntity);
                }
                var key = soundEntity.audioSourceKey.Value;
                if(!_soundPlayer.IsPlaying(key))
                {
                    soundEntity.isFlagDestroy = true;
                }
            }
        }
    }
}
