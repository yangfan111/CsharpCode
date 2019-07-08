//using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.SettingManager;
using XmlConfig;

namespace ArtPlugins
{
    public partial class GameQualitySettingManager
    {

        public static Dictionary<int, int> TypeValueDic = new Dictionary<int, int>();
        public static Dictionary<EVideoSettingId, float> valueDic = new Dictionary<EVideoSettingId, float>();
        public static Dictionary<EVideoSettingId, HashSet<Action<float>>> SetHandlerDic = new Dictionary<EVideoSettingId, HashSet<Action<float>>>();
        public static Dictionary<EVideoSettingId, Func<string>> GetHandlerDic = new Dictionary<EVideoSettingId, Func<string>>();
        static Dictionary<int, VideoSetting> _videoSettingDic = null;
        public static Dictionary<int, VideoSetting> VideoSettingDic
        {
            get
            {

                return _videoSettingDic;
            }

            set
            {
                _videoSettingDic = value;
            }
        }


        static GameQualitySettingManager()
        {
            Init();
        }

        public static void ApplyVideoEffect(EVideoSettingId EVideoSettingId, float value)
        {
            HashSet<Action<float>> setters = null;

            valueDic[EVideoSettingId] = value;
            if (SetHandlerDic.TryGetValue(EVideoSettingId, out setters))
            {
                if (setters.Count == 0)
                    return;

                foreach (var callback in setters)
                {
                    callback(value);
                }

            }
        }

        public static float? GetVideoSourceValue(EVideoSettingId id)
        {
            if (valueDic.ContainsKey(id))
                return valueDic[id];
            return null;
        }

        public static string GetVideoValue(EVideoSettingId id)
        {
            Func<string> getter = null;
            if (GetHandlerDic.TryGetValue(id, out getter))
            {
                return getter();
            }

            return null;
        }

        public static void ApplyTypeSetting(int type)
        {
            int value = GameQualitySettingManager.GetSettingByType(type);

            var list = GetSubOptions(type);

            foreach (var item in list)
            {
                VideoSetting data = GameQualitySettingManager.VideoSettingDic[item];
                if (data.ValuePerLevel != null && data.ValuePerLevel.Count > 0)
                {
                    if (value>-1 && value < data.ValuePerLevel.Count)
                    {
                        int index = data.ValuePerLevel[value];
                        if(index>-1&&index<data.LevelDatas.Count)
                        {
                            float fdata = data.LevelDatas[index];
                            ApplyVideoEffect((EVideoSettingId)item, fdata);
                        }
                        else
                        {
                            Debug.Log("error videosetting id" + item + " value " + value +"index" + index);
                        }
                        
                    }
                    else
                    {

                        Debug.Log("error videosetting id" + item + " value " + value);
                    }
                }

            }
        }




        public static void RegSettingChangeCallback(EVideoSettingId typeId, Action<float> setter, Func<string> getter)
        {
            if (setter != null)
            {
                HashSet<Action<float>> list = null;
                if (!SetHandlerDic.ContainsKey(typeId))
                {
                    SetHandlerDic[typeId] = new HashSet<Action<float>>();
                    SetHandlerDic[typeId].Add(setter);
                }
                else
                {
                    list = SetHandlerDic[typeId];
                    list.Add(setter);
                }

            }

            if (getter != null)
            {
                GetHandlerDic[typeId] = getter;
            }


        }

        public static void UnRegSettingChangeCallback(EVideoSettingId settingId, Action<float> setter, Func<string> getter)
        {
            if (setter != null)
            {
                HashSet<Action<float>> setters = null;
                if (SetHandlerDic.TryGetValue(settingId, out setters))
                {
                    setters.Remove(setter);
                }
            }


            if (getter != null)
            {
                //Func<float> temp = null ;
                //GetHandlerDic.TryGetValue(settingId, out temp);
                GetHandlerDic[settingId] = null;
            }


        }



        static Dictionary<EVideoSettingId, float> GetSubOptionValues(int type)
        {
            Dictionary<EVideoSettingId, float> dic = new Dictionary<EVideoSettingId, float>();
            var suboptions = GetSubOptions(type);
            var typeValue = GetSettingByType(type);

            foreach (var item in suboptions)
            {
                var videoSettingConfig = VideoSettingDic[item];
                var level = videoSettingConfig.ValuePerLevel[typeValue];
                var value = videoSettingConfig.LevelDatas[level];
                dic[(EVideoSettingId)item] = value;
            }

            return dic;
        }



        public static void SetVideoSettingConifg(Dictionary<int, VideoSetting> dic)
        {
            VideoSettingDic = dic;
        }

        public static void SetSettingByType(int type, int value)
        {
            TypeValueDic[type] = value;
        }

        static int GetSettingByType(int type)
        {
            return TypeValueDic[type];
        }

        public static List<int> GetSubOptions(int type)
        {
            List<int> res = new List<int>();
            var configlist = VideoSettingDic;
            foreach (var item in configlist)
            {
                if (item.Value.Type == type)
                    res.Add(item.Key);
            }
            return res;
        }

        //通知控制台设置最高级质量
        public static Action<string> maxQualityAction = null;
        public static void SetMaxQualityFromControl(string type)
        {
            if (maxQualityAction != null)
                maxQualityAction(type);
        }

        public static Func<bool> getMaxQualityStatue = null;
        public static string GetMaxQualityStatue()
        {
            if (getMaxQualityStatue != null)
            {
                var result = getMaxQualityStatue();
                return result == true ? "yes" : "not";
            }
            return "getMaxQualityStatue == null";
        }

    }
}
