using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;

namespace Core.Audio
{
    //public class AudioSystem
    //{
    //        }
    //public class AudioLaunchNotifycation
    //{
    //    public AudioLaunchNotifycation()
    //    {
    //       //  AkSoundEngineController.Instance.StarupReadyEvt += AKAudioEntry.LaunchAppAudio;
    //    }
    //}
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
                if (dispatcher == null)
                {
                    if (prepareReady)
                    {
                        dispatcher = new AKAudioDispatcher(bankResLoader);
                    }
                    else
                    {
                        throw new AudioFrameworkException("Audio engine or resouce has not been startup yet");

                    }
                }
                return dispatcher;
            }
        }
        public static void PostEvent(int eventId, UnityEngine.GameObject target)
        {
            Dispatcher.PostEvent(eventId, target);
        }
        
        public static WisePluginNotificationRoute WiseNotificationRoute { get; private set; }
        public static void LaunchAppAudio(WisePluginNotificationRoute wiseNotificationRoute)
        {
            AudioLogger.Info("[Audio=>Entry]engine audio preapared ready");
            WiseNotificationRoute = wiseNotificationRoute;
            bankResLoader = new AKAudioBankLoader();
            if (AudioConst.AudioLoadTypeWhenStarup == "Sync")
            {
                //   AKRESULT result1 = AkBankManagerExt.LoadBankRes("Init", false, false);
                //   AKRESULT result2 = AkBankManagerExt.LoadBankRes("Test", false, false);
                AKRESULT result = bankResLoader.LoadInitialBnkRes();
                AudioLogger.Info("[Audio=>Entry]App audio try to preapare");
                if (result == AKRESULT.AK_Success)
                {
                    prepareReady = true;
                    Dispatcher.PostEvent(1, wiseNotificationRoute.gameObject);
                   // uint playingId = AkSoundEngine.PostEvent("Gun_56_shot", wiseNotificationRoute.gameObject);
             
                }
            }
            else
            {
                bankResLoader.LoadInitialBnkResAsync(OnBnkAsyncFinish);
            }

        }
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
