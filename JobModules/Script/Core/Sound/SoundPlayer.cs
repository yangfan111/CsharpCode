using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Core.Sound
{
    public class SoundPlayer : ISoundPlayer
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SoundPlayer));
        private readonly Dictionary<int, AudioSource> _audioSources = new Dictionary<int, AudioSource>();
        private static int _index = 0;
        public int Preload(AudioSource src)
        {
            _audioSources[_index] = src;
            unchecked
            {
                return _index++;
            }
        }

        public void Play(int key, float offset)
        {
            ActionIfExist(key, offset, SetAudioTime);
            Play(key);
        }

        public void Unload(int key)
        {
            _audioSources.Remove(key);
        }

        public void Play(int key)
        {
            ActionIfExist(key, PlayAudio);
        }

        public void Play(int key, bool loop)
        {
            if(loop)
            {
                ActionIfExist(key, LoopAudio, PlayAudio);
            }
            else
            {
                ActionIfExist(key, PlayAudio);
            }
        }

        public void Play(int key, float offset, bool loop)
        {
            ActionIfExist(key, offset, SetAudioTime);
            if(loop)
            {
                ActionIfExist(key, LoopAudio, PlayAudio);
            }
            else
            {
                ActionIfExist(key, PlayAudio);
            }
        }

        public float GetLength(int key)
        {
            if (_audioSources.ContainsKey(key))
            {
                return _audioSources[key].clip.length;
            }

            return 0;
        }

        public void Play(int key, Vector3 pos)
        {
            Play(key, pos, false);
        }

        public void Play(int key, Vector3 pos, bool loop)
        {
            AudioSource src;
            if (loop)
            {
                src = ActionIfExist(key, LoopAudio, PlayAudio);
            }
            else
            {
                src = ActionIfExist(key, UnLoopAudio, PlayAudio);
            }
            if (null != src)
            {
                src.gameObject.transform.position = pos;
            }
        }

        public void Pause(int key)
        {
            ActionIfExist(key, PauseAuido);
        }

        public void Resume(int key)
        {
            ActionIfExist(key, ResumeAudio);
        }

        public void Pause()
        {
            foreach (var pair in _audioSources)
            {
                ActionIfExist(pair.Value, PauseAuido);
            }
        }

        public void Resume()
        {
            foreach (var pair in _audioSources)
            {
                ActionIfExist(pair.Value, ResumeAudio);
            }
        }

        public void PlayOneShot(int key)
        {
            ActionIfExist(key, PlayAuidoOneShot);
        }

        public void Stop(int key)
        {
            ActionIfExist(key, StopAudio);
        }

        public bool IsPlaying(int key)
        {
            var src = GetSource(key);
            if (null != src)
            {
                return src.isPlaying;
            }
            else
            {
                return false;
            }
        }

        public void StopAll()
        {
            foreach (var pair in _audioSources)
            {
                ActionIfExist(pair.Value, StopAudio);
            }
        }

        public void SetVolume(int key, float volume)
        {
            ActionIfExist(key, volume, SetAudioVolume);
        }

        public void SetAllVolume(float volume)
        {
            foreach (var pair in _audioSources)
            {
                ActionIfExist(pair.Value, volume, SetAudioVolume);
            }
        }

        public void Mute(int key, bool mute)
        {
            ActionIfExist(key, mute, SetAudioMute);
        }

        public void MuteAll(bool mute)
        {
            foreach (var pair in _audioSources)
            {
                ActionIfExist(pair.Value, mute, SetAudioMute);
            }
 
        }

        private static void PlayAudio(AudioSource src)
        {
            if (!src.isPlaying)
            {
                src.Play();
            }
        }

        private static void PauseAuido(AudioSource src)
        {
            src.Pause();
        }

        private static void LoopAudio(AudioSource src)
        {
            src.loop = true;
        }

        private static void UnLoopAudio(AudioSource src)
        {
            src.loop = false;
        }

        private static void ResumeAudio(AudioSource src)
        {
            src.UnPause();
        }

        private static void PlayAuidoOneShot(AudioSource src)
        {
            src.PlayOneShot(src.clip);
        }

        private static void StopAudio(AudioSource src)
        {
            src.Stop();
        }

        private static void SetAudioVolume(AudioSource src, float volume)
        {
            src.volume = volume;
        }

        private static void SetAudioMute(AudioSource src, bool mute)
        {
            src.mute = mute;
        }

        private static void SetAudioTime(AudioSource src, float time)
        {
            src.time = time;
        }

        private AudioSource GetSource(int key)
        {
            if (_audioSources.ContainsKey(key))
            {
                return _audioSources[key];
            }
            else
            {
                return null;
            }
        }

        private AudioSource ActionIfExist(int key, params Action<AudioSource>[] actions)
        {
            var src = GetSource(key);
            if (null != src)
            {
                for (var i = 0; i < actions.Length; i++)
                {
                    actions[i](src);
                }
            }
            else
            {
                Logger.ErrorFormat("key {0} does not exist ", key);
            }

            return src;
        }

        private void ActionIfExist(AudioSource src, params Action<AudioSource>[] actions)
        {
            if (null != src)
            {
                for (var i = 0; i < actions.Length; i++)
                {
                    actions[i](src);
                }
            }
            else
            {
                Logger.ErrorFormat("audio srouce is null ");
            }
        }

        private void ActionIfExist<T>(AudioSource src, T param, params Action<AudioSource, T>[] actions)
        {
            if (null != src)
            {
                for (var i = 0; i < actions.Length; i++)
                {
                    actions[i](src, param);
                }
            }
            else
            {
                Logger.ErrorFormat("audio srouce is null ");
            }
        }
        private void ActionIfExist<T>(int key, T arg, params Action<AudioSource, T>[] actions)
        {
            var src = GetSource(key);
            if (null != src)
            {
                for (var i = 0; i < actions.Length; i++)
                {
                    actions[i](src, arg);
                }
            }
            else
            {
                Logger.ErrorFormat("audio srouce is null ");
            }
        }
    }
}
