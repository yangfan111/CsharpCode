using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;

namespace Core.Audio
{

    public class AKAudioEntry
    {
        private static AKAudioDispatcher dispatcher;
        private static AKAudioBankLoader bankResLoader;
        private static bool prepareReady = false;
        public static readonly LoggerAdapter AudioLogger = new LoggerAdapter(typeof(AKAudioDispatcher));
        //   public static AudioLaunchNotifycation Notify = new AudioLaunchNotifycation();
        static AKAudioEntry()
        {
            //      AkSoundEngineController.Instance.StarupReadyEvt += LaunchAppAudio;
        }
        public static AKAudioDispatcher Dispatcher
        {
            get
            {
                if (AudioInfluence.IsForbidden) return null;
                if (dispatcher == null)
                {
                    if (prepareReady)
                    {
                        dispatcher = new AKAudioDispatcher(bankResLoader);
                    }
                    else
                    {
                        AudioInfluence.IsForbidden = true;
                     //   throw new AudioFrameworkException("Audio engine or resouce has not been startup yet");

                    }
                }
                return dispatcher;
            }
        }
        public static void PostEvent(int eventId, UnityEngine.GameObject target)
        {
            if (AudioInfluence.IsForbidden) return;
            Dispatcher.PostEvent(eventId, target);
        }
        public static void PostEvent(int eventId)
        {
            Dispatcher.PostEvent(eventId, WiseNotificationRoute.defaultSpatialListener.gameObject);
        }
        public static AudioPluginsDriver WiseNotificationRoute { get; private set; }
        public static void LaunchAppAudio(AudioPluginsDriver wiseNotificationRoute)
        {
            if (AudioInfluence.IsForbidden) return;
            AudioLogger.Info("[Audio=>Entry]engine audio preapared ready");
            WiseNotificationRoute = wiseNotificationRoute;
            bankResLoader = new AKAudioBankLoader();

#if UNITY_EDITOR
       //     TestCodeChunk_Interanl();

# endif
            if (AudioInfluence.AudioLoadTypeWhenStarup == "Sync")
            {

                AKRESULT result = bankResLoader.LoadInitialBnkRes();
                AudioLogger.Info("[Audio=>Entry]App audio try to preapare");
                if (result == AKRESULT.AK_Success || result == AKRESULT.AK_BankAlreadyLoaded)
                {
                    prepareReady = true;
                    Debug.Log("prepareReady");
#if UNITY_EDITOR
                    // TestCodeChunk_External();
                    //  TestCodeChunk_Interanl();
#endif
                }
            }
            else
            {
                bankResLoader.LoadInitialBnkResAsync(OnBnkAsyncFinish);
            }

        }
#if UNITY_EDITOR
        static UnityEngine.GameObject testObj;
        static UnityEngine.GameObject AudioTestObj
        {
            get
            {
                if (!testObj)
                {
                    testObj = GameObject.Find("Directional Light");
                }
                return testObj;
            }
        }

        static void TestCodeChunk_Interanl()
        {
            UnityEngine.Debug.LogFormat("-------------internal test--------------");
            uint m_BankID;
            var result = AkSoundEngine.LoadBank("Weapon_Footstep", AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);

            UnityEngine.Debug.LogFormat("Load Init bank :" + result);
            uint playingId = AkSoundEngine.PostEvent("Gun_P1911_shot", WiseNotificationRoute.gameObject);
      
            UnityEngine.Debug.LogFormat("-----------------------------------------");
        }


        static void TestCodeChunk_External()
        {
            UnityEngine.Debug.LogFormat("-------------external test--------------");
            AKRESULT result = bankResLoader.LoadInitialBnkRes();
            UnityEngine.Debug.LogFormat("Load Init bank :" + result);
            // Dispatcher.PostEvent(1,AudioTestObj);
            Dispatcher.PostEvent(2, WiseNotificationRoute.gameObject);
            UnityEngine.Debug.LogFormat("-----------------------------------------");
        }
#endif
        static void OnBnkAsyncFinish()
        {
            AudioLogger.Info("[Audio=>Entry]App audio preapared ready");
            prepareReady = true;
        }
        public static void AudioAssert(bool comparison, string errMsg)
        {
            if (!comparison)
            {
                throw new AudioFrameworkException(errMsg);
            }
        }
        public static void AudioAssert(bool comparison)
        {
            if (!comparison)
            {
                throw new AudioFrameworkException();
            }
        }
    }
}
