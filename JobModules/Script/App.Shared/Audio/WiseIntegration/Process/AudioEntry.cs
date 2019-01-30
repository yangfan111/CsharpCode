using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Utils;
using UnityEngine;

namespace App.Shared.Audio
{

    public class AKAudioEntry
    {
        private static AKAudioDispatcher dispatcher;
        private static AKAudioBankLoader bankResLoader;
        public static bool PrepareReady { get; private set; }

   
        public static AKAudioDispatcher Dispatcher
        {
            get
            {
                if (AudioInfluence.IsForbidden) return null;
                if (dispatcher == null)
                {
                    if (PrepareReady)
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
   //         Dispatcher.PostEvent(eventId, target);
        }
        public static void PostEvent(int eventId)
        {
         //   Dispatcher.PostEvent(eventId, PluginsDriver.defaultSpatialListener.gameObject);
        }
        public static AudioPluginsDriver PluginsDriver { get; private set; }
        public static void LaunchAppAudio(AudioPluginsDriver pluginsDriver)
        {
            if (AudioInfluence.IsForbidden) return;
        
            PluginsDriver = pluginsDriver;
            bankResLoader = new AKAudioBankLoader();
            AKRESULT result = bankResLoader.Initialize();
            
            if (result != AKRESULT.AK_Success)
            {
                AudioUtil.ELog("Sound Engine Not Initialized");
                return;
            }
            PrepareReady = true;
            AudioUtil.NLog("Audio Asset preapared ready");
        }
      
        public static void AudioAssert(bool comparison, string errMsg)
        {
            if (!comparison)
            {
                throw new AudioFrameworkException(errMsg);
            }
        }
        //#if UNITY_EDITOR
        //        static UnityEngine.GameObject testObj;
        //        static UnityEngine.GameObject AudioTestObj
        //        {
        //            get
        //            {
        //                if (!testObj)
        //                {
        //                    testObj = GameObject.Find("Directional Light");
        //                }
        //                return testObj;
        //            }
        //        }
        //        [Conditional("UNITY_EDITOR")]
        //        static void TestCodeChunk_Interanl()
        //        {
        //            UnityEngine.Debug.LogFormat("-------------internal test--------------");
        //            uint m_BankID;
        //            var result = AkSoundEngine.LoadBank("Weapon_Footstep", AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);

        //            UnityEngine.Debug.LogFormat("Load Init bank :" + result);
        //            uint playingId = AkSoundEngine.PostEvent("Gun_P1911_shot", PluginsDriver.gameObject);

        //            UnityEngine.Debug.LogFormat("-----------------------------------------");
        //        }

        //        [Conditional("UNITY_EDITOR")]
        //        static void TestCodeChunk_External()
        //        {
        //            UnityEngine.Debug.LogFormat("-------------external test--------------");
        //            AKRESULT result = bankResLoader.Initialize();
        //            UnityEngine.Debug.LogFormat("Load Init bank :" + result);
        //            // Dispatcher.PostEvent(1,AudioTestObj);
        //            //Dispatcher.PostEvent(2, PluginsDriver.gameObject);
        //            UnityEngine.Debug.LogFormat("-----------------------------------------");
        //        }
        //#endif


    }
}
