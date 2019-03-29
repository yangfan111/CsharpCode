using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameTime;
using Core.Sound;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Sound
{
    public class SoundPlaySystem : IGamePlaySystem 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SoundPlaySystem)); 
        private ISoundPlayer _soundPlayer;
        private IGroup _group;
        private List<SoundEntity> _playList = new List<SoundEntity>();
        private ICurrentTime _currentTime;

        public SoundPlaySystem(Contexts contexts)
        {
            _group = contexts.sound.GetGroup(SoundMatcher.AnyOf(SoundMatcher.AudioSourceKey).NoneOf(SoundMatcher.Playing));
            _soundPlayer = contexts.session.clientSessionObjects.SoundPlayer;
            _currentTime = contexts.session.currentTimeObject;
        }

        public void OnGamePlay()
        {
            foreach (SoundEntity soundEntity in _group)
            {
                var key = soundEntity.audioSourceKey.Value;
                if (!soundEntity.flagPlaying && _currentTime.CurrentTime > soundEntity.timeInfo.StartTime)
                {
                    if(null == soundEntity.unityObj.UnityObject)
                    {
                        Logger.Error("sound entity's unityobj is null");
                        soundEntity.isFlagDestroy = true;
                        continue;
                    }

                    if(soundEntity.hasPosition)
                    {
                        soundEntity.unityObj.UnityObject.AsGameObject.transform.position = soundEntity.position.Value;
                    }
                    
                    if(soundEntity.hasSoundPlayInfo)
                    {
                        _soundPlayer.Play(key, soundEntity.soundPlayInfo.Loop);
                    }
                    else
                    {
                        _soundPlayer.Play(key);
                    }
                    _playList.Add(soundEntity);
                }
            }

            foreach (var soundEntity in _playList)
            {
                soundEntity.flagPlaying = true;
            }
            _playList.Clear();
        }
    }
}