using Core.Utils;
using UnityEngine;

namespace App.Shared.Audio
{
    /// <summary>
    /// Defines the <see cref="AKAudioEntry" />
    /// </summary>
    public static class AKAudioEntry
    {
        /// <summary>
        /// 音频执行实例
        /// </summary>
        private static AKAudioDispatcher dispatcher;

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

        public static AKAudioDispatcher Dispatcher
        {
            get
            {
                if (SharedConfig.IsMute) return null;
                if (dispatcher == null)
                {
                    if (PrepareReady)
                    {
                        dispatcher = new AKAudioDispatcher(bankResLoader);
                    }
                    else
                    {
                        AudioInfluence.IsForbidden = true;
                        //AudioUtil.ELog("Dispather call in unexpected");
                    }
                }
                return dispatcher;
            }
        }

        public static readonly LoggerAdapter AudioLogger = new LoggerAdapter(typeof(AKAudioEntry));

        public static void LaunchAppAudio(GameObject audioObject,AudioStepPlayInfo stepPlayInfo)
        {

#if UNITY_EDITOR
            AudioInfluence.IsForbidden = SharedConfig.IsMute;
            if (SharedConfig.IsMute)
                return;
#endif
            AudioInfluence.StepPlayInfo = stepPlayInfo;
            PluginsDriver = audioObject;
            bankResLoader = new AudioBankLoader();
            AKRESULT result = bankResLoader.Initialize();

            if (result != AKRESULT.AK_Success)
            {
                AudioLogger.Error("Sound engine not initialized");
                return;
            }
            PrepareReady = true;
            DebugUtil.MyLog("Audio media initialize sucess", DebugUtil.DebugColor.Green);
            AudioLogger.Info("Audio media initialize sucess");
        }
    }
}
