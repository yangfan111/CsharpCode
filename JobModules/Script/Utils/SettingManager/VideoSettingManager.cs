using System;
using System.Collections.Generic;
using ArtPlugins;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Utils.SettingManager
{
    public enum EVideoSettingType
    {
        GlobalShadow = 1,
        ModuleDetail,
        ShaderDetail,
        AntiAliasing,
        VerticalSync,
        RealTimeLight,
        EffectDetails
    }

    public class VideoSettingManager : Singleton<VideoSettingManager>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VideoSettingManager));

        public static VideoSettingManager GetInstance()
        {
            return SingletonManager.Get<VideoSettingManager>();
        }

        private static string LocalVideoCacheKey = "VideoSetting";


        public void SaveLocalVideoSetting(Dictionary<int, float> list)
        {
            try
            {
                string newStr = SettingConfigUtil.DictionaryToString(list);
                PlayerPrefs.SetString(LocalVideoCacheKey, newStr);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public void FlushVideoSettingData(Dictionary<int, float> sendValList)
        {
            AllVideoSettingHelper.Instance.UpdateVideoSettingData(sendValList);
            SaveLocalVideoSetting(sendValList);
        }

        public Dictionary<int, float> LoadLocalVideoSetting()
        {
            Dictionary<int, float> dict = new Dictionary<int, float>();
            string str = string.Empty;
            try
            {
                str = PlayerPrefs.GetString(LocalVideoCacheKey, string.Empty);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            var sourceDic = SettingConfigUtil.CovertString(str);
            if (sourceDic == null)
                return null;
            foreach (var pair in sourceDic)
            {
                //SetSettingValue(pair.Key, pair.Value);
                dict.Add(pair.Key, float.Parse(pair.Value));
            }

            return dict;
        }

        private Dictionary<EVideoSettingType, string> _typeNameDict = new Dictionary<EVideoSettingType, string>()
        {
            {EVideoSettingType.GlobalShadow,"全局阴影"},
            {EVideoSettingType.ModuleDetail,"模型/贴图细节"},
            {EVideoSettingType.ShaderDetail,"光影细节"},
            {EVideoSettingType.AntiAliasing,"多重采样抗锯齿"},
            {EVideoSettingType.VerticalSync,"垂直同步"},
            {EVideoSettingType.RealTimeLight,"实时光源"},
            {EVideoSettingType.EffectDetails,"效果细节"}
        };

        public string GetTypeNameByType(EVideoSettingType type)
        {
            string res = string.Empty;
            _typeNameDict.TryGetValue(type, out res);
            return res;
        }
    }
}
