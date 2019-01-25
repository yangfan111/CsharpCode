using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.Components.Sound;
using Core.Configuration;
using Core.IFactory;
using Core.Sound;
using Core.Utils;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.Sound
{
    public class ClientVehicleSoundManager
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientVehicleSoundManager));

        private VehicleSoundConfigManager _soundConfigManager;
        private ISoundEntityFactory _soundEntityFactory;
        private ISoundPlayer _soundPlayer;
        private Dictionary<int, SoundEntity> _soundEntityCache;
        private List<int> _actionSoundIdList;
        public ClientVehicleSoundManager(ISoundEntityFactory soundEntityFactory, ISoundPlayer soundPlayer)
        {
            _soundConfigManager = SingletonManager.Get<VehicleSoundConfigManager>();
            _soundEntityFactory = soundEntityFactory;
            _soundPlayer = soundPlayer;
            _soundEntityCache = new Dictionary<int, SoundEntity>();
            _actionSoundIdList = new List<int>();
        }

        public bool Play(int soundId, float offset)
        {
            if (soundId > (int)EVehicleSoundId.Invalid)
            {
                //mute other sounds
                MuteSoundExcept(soundId);

                // if it is the first time audio-play then create the specified sound entity.
                SoundEntity soundEntity = null;
                if (!_soundEntityCache.ContainsKey(soundId))
                {
                    soundEntity = CreateSoundEntity(soundId);
                    _soundEntityCache[soundId] = soundEntity;
                }

                soundEntity = _soundEntityCache[soundId];
                if (soundEntity == null)
                {
                    return true;
                }

                //play the speicified sound
                if (soundEntity.hasAudioSourceKey)
                {
                    var soundKey = soundEntity.audioSourceKey.Value;
                    if (_soundPlayer.IsPlaying(soundKey))
                    {
                        _soundPlayer.Mute(soundKey, false);
                    }
                    else
                    {
                        var isLoopSound = _soundConfigManager.IsLoopSound(soundId);
                        if (isLoopSound && offset > 0)
                        {
                            var clipLength = _soundPlayer.GetLength(soundKey);
                            if (clipLength > 0)
                            {
                                offset %= clipLength;
                            }
                        }
                        _soundPlayer.Play(soundKey, offset, isLoopSound);
                    }
                    
                    return true;
                }

                return false;
            }

            //mute all sound if try play Invalid Sound
            MuteSoundExcept((int)EVehicleSoundId.Invalid);

            return true;
        }

        private void MuteSoundExcept(int soundId)
        {
            ActionSoundExcept(soundId, MuteAction);
        }

        public void StopSound()
        {

            if (_soundEntityCache.Count > 0)
            {
                ActionSoundExcept((int)EVehicleSoundId.Invalid, StopAction);
                //clear cache
                _soundEntityCache.Clear();
            }
            
        }

        private SoundEntity CreateSoundEntity(int soundId)
        {
            var isLoopSound = _soundConfigManager.IsLoopSound(soundId);
            var soundEntity = (SoundEntity)_soundEntityFactory.CreateSelfOnlySound(soundId, isLoopSound);
            if (soundEntity != null)
            {
                soundEntity.isFlagPreventDestroy = true;
            }
            else
            {
                _logger.ErrorFormat("Configuration Error: The Sound Id {0} for Vehicle does not exist!", soundId);
            }

            return soundEntity;
        }

        private void ActionSoundExcept(int exceptId, Action<int> action)
        {
            _actionSoundIdList.Clear();
            foreach (var soundId in _soundEntityCache.Keys)
            {
                if (soundId != exceptId)
                {
                    _actionSoundIdList.Add(soundId);
                }
            }

            int count = _actionSoundIdList.Count;
            for (int i = 0; i < count; ++i)
            {
                var soundId = _actionSoundIdList[i];
                action(soundId);
            }
        }

        private void MuteAction(int soundId)
        {
            var soundEntity = _soundEntityCache[soundId];
            if (soundEntity != null && soundEntity.hasAudioSourceKey)
            {
                var key = soundEntity.audioSourceKey.Value;
                if (_soundPlayer.IsPlaying(key))
                {
                    _soundPlayer.Mute(key, true);
                }
                else
                {
                    RemoveSoundEntity(soundId);
                }
            }
        }

        private void StopAction(int soundId)
        {
            var soundEntity = _soundEntityCache[soundId];
            if (soundEntity != null && soundEntity.hasAudioSourceKey)
            {
                var key = soundEntity.audioSourceKey.Value;
                if (_soundPlayer.IsPlaying(key))
                {
                    _soundPlayer.Stop(key);
                }
            }

            RemoveSoundEntity(soundId);
        }

        private void RemoveSoundEntity(int soundId)
        {
            var soundEntity = _soundEntityCache[soundId];
            if (soundEntity != null)
            {
                soundEntity.isFlagPreventDestroy = false;
                soundEntity.isFlagDestroy = true;
                _soundEntityCache[soundId] = null;
            }
        }
       
    }
}
