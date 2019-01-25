using System;
using System.Collections.Generic;

namespace   App.Shared.Audio
{
    public class AudioRegulator
    {
        private float VolumeRate;
        private float TransitionSecond;
        private bool Mute;
        //备选
        //internal  float DefaultPitch = 1f;
        //internal  float DefaultPanStereo = 0f;
        //internal  float DefaultSpatialBlend = 0f;
        //internal  float DefaultMaxDistance = 100f;
        //private float m_Time;
        //private bool m_MuteInSoundGroup;
        //private bool m_Loop;
        //private int m_Priority;
        //private float m_VolumeInSoundGroup;
        //private float m_FadeInSeconds;
        //private float m_Pitch;
        //private float m_PanStereo;
        //private float m_SpatialBlend;
        //private float m_MaxDistance;
        public System.Action<float> onTransistionVary;
        public System.Action<float> onVolumeRateVary;
        public System.Action<bool> onMuteStateVary;
       

        public AudioRegulator()
        {
            VolumeRate = AudioInfluence.DefualtVolumeRate;
            TransitionSecond = AudioInfluence.DefaultTransitionDuration;
        }
        public void SetVolumeRate(float rate)
        {
            if (VolumeRate != rate)
            {
                VolumeRate = rate;
                if (onVolumeRateVary != null)
                {
                    onVolumeRateVary(rate);
                }
            }

        }
        public void SetTransistionSecond(float transistioin)
        {
            if (TransitionSecond != transistioin)
            {
                TransitionSecond = transistioin;
                if (onTransistionVary != null)
                {
                    onTransistionVary(transistioin);
                }
            }

        }
        public void SetMute(bool mute)
        {
            if (Mute != mute)
            {
                Mute = mute;
                if (onMuteStateVary != null)
                {
                    onMuteStateVary(mute);
                }
            }
        }
    }
}