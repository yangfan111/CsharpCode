using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.Sound
{
    public interface ISoundPlayer
    {
        int Preload(AudioSource clip);
        void Unload(int key);
        void Play(int key);
        void Play(int key, float offset);
        void Play(int key, float offset, bool loop);
        void Play(int key, bool loop);
        void Play(int key, Vector3 pos);
        void Play(int key, Vector3 pos, bool loop);
        void Pause(int key);
        void Resume(int key);
        void Pause();
        void Resume();
        void PlayOneShot(int key);
        void Stop(int key);
        void StopAll();
        bool IsPlaying(int key);
        void SetVolume(int key, float volume);
        void SetAllVolume(float volume);
        void Mute(int key, bool mute); 
        void MuteAll(bool mute); 
        float GetLength(int key);
    }
}
