using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Utils;
using UnityEngine;

namespace App.Shared.Audio
{

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
        public static AudioPluginsDriver PluginsDriver { get; private set; }
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
                        AudioUtil.ELog("Dispather call in unexpected");
                    }
                }
                return dispatcher;
            }
        }

        public static void LaunchAppAudio(AudioPluginsDriver pluginsDriver)
        {
#if UNITY_EDITOR
            AudioInfluence.IsForbidden = SharedConfig.IsMute;
            if (SharedConfig.IsMute)
                return;
#endif
            PluginsDriver = pluginsDriver;
            bankResLoader = new AudioBankLoader();
            AKRESULT result = bankResLoader.Initialize();

            if (result != AKRESULT.AK_Success)
            {
                AudioUtil.ELog("Sound engine not initialized");
                return;
            }
            PrepareReady = true;
            AudioUtil.NLog("Initial asset preapared ready");
        }




    }
}
