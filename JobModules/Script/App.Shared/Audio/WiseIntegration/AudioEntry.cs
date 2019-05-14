using System;
using com.wd.free.action.stage;
using Core.Utils;
using UnityEngine;

namespace App.Shared.Audio
{
    /// <summary>
    /// Defines the <see cref="AudioEntry" />
    /// </summary>
    public static class AudioEntry
    {

        /// <summary>
        /// 音频执行实例
        /// </summary>
        private static AudioDispatcher dispatcher;

        /// <summary>
        /// 音频加载实例
        /// </summary>
        private static AudioBankLoader bankResLoader;

        /// <summary>
        /// 音频插件驱动
        /// </summary>
        public static GameObject PluginsDriver { get; private set; }

        /// <summary>
        /// 音频加载完成标志位
        /// </summary>
        public static bool PrepareReady { get; private set; }

        internal static AudioDispatcher Dispatcher
        {
            get
            {
#if UNITY_EDITOR
                if (ListenerManager == null)
                    return null;
#endif
                if (SharedConfig.IsMute || SharedConfig.IsServer) return null;
                if (dispatcher == null)
                {
                    if (PrepareReady)
                    {
                        dispatcher = new AudioDispatcher(bankResLoader);
                    }
                 
                }
                return dispatcher;
            }
        }


         public static LoggerAdapter Logger = new LoggerAdapter(typeof(AudioEntry));
        private static AudioListenerManager _listenerManager;
        public static AudioListenerManager ListenerManager
        {
            get
            {
                if (_listenerManager != null && !_listenerManager.DefaultListenerObj)
                    _listenerManager = null;
                return _listenerManager;
            }
            set { _listenerManager = value; }
        }
        public static void ReloadWiseBank()
        {
            bankResLoader = new AudioBankLoader();
            AKRESULT result = bankResLoader.Initialize();
            DebugUtil.MyLog("Wise Bank  reload :"+result);
        }
        public static void LaunchAppAudio(GameObject audioObject)
        {

#if UNITY_EDITOR
        
            if (SharedConfig.IsMute)
                return;
#endif
           // AudioInfluence.StepPlayInfo = stepPlayInfo;
            PluginsDriver = audioObject;
            bankResLoader = new AudioBankLoader();
            AKRESULT result = bankResLoader.Initialize();

            if (result != AKRESULT.AK_Success)
            {
                Logger.Error("Sound engine not initialized");
                return;
            }
            PrepareReady = true;
            DebugUtil.MyLog("Audio media initialize sucess", DebugUtil.DebugColor.Green);
            Logger.Info("Audio media initialize sucess");
        }
    }
}
