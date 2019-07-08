
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtPlugins;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Utils.SettingManager
{
    public enum QualityLevel
    {
        Undefined,
        Low = 1,
        MediumLow,
        Medium,
        MediumHigh,
        High,
    }
    public enum VideoSettingId
    {
        Begin = 96,
        Quality = 100,
        End = 109,
    }
    public class PlayerSettingItem
    {

        public SettingConfigItem item;
        public string value;
       
    }
    public class VideoSettingItem : PlayerSettingItem
    {

    }


    public class SettingManager : Singleton<SettingManager>
    {
        public static int[] videoEffectRange =  { 102, 103, 104, 105, 107, 108 };
        public const int CUSTOM_OPTION_INDEX = 5;
        public const int VIDEO_CONTROLID = 100;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SettingManager));
        private static string LocalCacheKey = "Setting20190618";

        private Action onchangedCallback = null;
        private Dictionary<int, Action> callbackDic = new Dictionary<int, Action>();


        private Dictionary<int, PlayerSettingItem> _configDic;
        private Dictionary<int, VideoSetting> _videoSettingDic;


        private Dictionary<int, string> _localCache;
        private bool maxQualityMode = false;


        VideoSettingHandler videoSettingHandler = new VideoSettingHandler();

        public SettingManager()
        {
            _localCache = new Dictionary<int, string>();
           

            maxQualityMode = QualitySettings.names[QualitySettings.GetQualityLevel()].ToLower().StartsWith("max_");
            _logger.InfoFormat("MaxQualityMode {0}", maxQualityMode ? "yes" : "not");


        }

        public static SettingManager GetInstance()
        {
            return SingletonManager.Get<SettingManager>();
        }
        #region manager init


        public void InitConfig(List<SettingConfigItem> list, List<SettingConfigVideoItem> videolist, Dictionary<int, VideoSetting> videoConfigs )
        {

            _configDic = new Dictionary<int, PlayerSettingItem>();
            _videoSettingDic = new Dictionary<int, VideoSetting>();
            foreach (var item in list)
            {
                var config = AddItemToManager(item);
                _configDic[config.item.Id] = config;
            }

            foreach (var item in videolist)
            {
                var config = AddItemToManager(item);
                _configDic[config.item.Id] = config;
            }
            

            GameQualitySettingManager.VideoSettingDic = videoConfigs;
        }



        public bool IsInitialized()
        {
            return _configDic != null;
        }

        private PlayerSettingItem AddItemToManager(SettingConfigItem item)
        {
            var settingItem = new PlayerSettingItem();
            settingItem.item = item;
            settingItem.value = item.DefaultValue;
            return settingItem;
        }

        private PlayerSettingItem AddItemToManager(SettingConfigVideoItem item)
        {
            var settingItem = new VideoSettingItem();
            var baseConfig = new SettingConfigItem();
            baseConfig.Id = item.Id;
            baseConfig.Type = item.Type;
            baseConfig.TypeName = item.TypeName;
            baseConfig.Desription = item.Desription;
            baseConfig.ControlType = item.ControlType;
            baseConfig.ComboxItemNames = item.ComboxItemNames;
            baseConfig.DefaultValue = item.DefaultValue;
            settingItem.item = baseConfig;
            settingItem.value = item.DefaultValue;
            return settingItem;
        }

        #endregion

        #region config operate
        public void ResetSetting()
        {
            var dic = new Dictionary<int, string>();
            foreach (var item in _configDic)
            {
                if (item.Value.value != item.Value.item.DefaultValue)
                {
                    item.Value.value = item.Value.item.DefaultValue;
                    //SetSettingValue(item.Key, item.Value.value);
                }
                dic[item.Key] = item.Value.value;
            }
            SetSetting(dic);
        }

        public void ResetSetting(List<int> ids)
        {
            var dic = new Dictionary<int, string>();
            foreach (var id in ids)
            {
                PlayerSettingItem item=null;
                _configDic.TryGetValue(id,out item);
                if (item == null)
                    continue;
                if (item.value != item.item.DefaultValue)
                {
                    item.value = item.item.DefaultValue;
                    //SetSettingValue(item.Key, item.Value.value);
                }
                dic[id] = item.value;
            }
            SetSetting(dic);
        }

        public void FlushData()
        {
            _localCache.Clear();
            List<int> changedSettingList = new List<int>();
            foreach (var item in _configDic)
            {
                if(FilterSaveType(item.Key))
                {
                    if (item.Value.value != null && item.Value.value.Equals(item.Value.item.DefaultValue) == false)
                        _localCache[item.Value.item.Id] = item.Value.value.ToString();
                }
            }

            SaveSettingToLocal();
        }


        public bool FilterSaveType(int id)
        {
            var item = GetSettingItem(id);

            var type = item.item.Type;

            if (type == 3 || type == 4)
                return true;
            else
                return false;
        }

        public Dictionary<int, object> GetInvalidSettingValue(Dictionary<int, object> dic = null)
        {
            if (dic == null) dic = new Dictionary<int, object>();
            foreach (var item in _configDic)
            {
                if (item.Value.value != item.Value.item.DefaultValue)
                {
                    dic[item.Key] = item.Value.item.DefaultValue;
                }
            }
            return dic;
        }

        public string GetSettingValue(int id)
        {
            PlayerSettingItem ret = null;
            if (_configDic.TryGetValue(id, out ret))
                return ret.value;
            else
                return "";
        }
        public PlayerSettingItem GetSettingItem(int id)
        {
            PlayerSettingItem ret = null;
            _configDic.TryGetValue(id, out ret);
            return ret;
        }


        public void SetSetting(Dictionary<int, string> dic)
        {

            UnityProfiler.StartProfiler("CHANGE SETTING");
            foreach (var item in dic)
            {
                SetSettingValue(item.Key, item.Value);
            }
            //callback
            foreach (var item in dic)
            {
                if(item.Value!=null)
                {
                    DoCallback(item.Key, item.Value);
                }
            }


            UnityProfiler.StopProfiler();
        }

      
        public void SetSetting(int id, object value)
        {
            if (_configDic.ContainsKey(id) == false)
            {
                _logger.WarnFormat("invalid id, setting id {0}", id);
                return;
            }
            if (value != null)
            {
                SetSettingValue(id,value);
                DoCallback(id, value);
            }

        }

        private void DoCallback(int id, object value)
        {
            SettingHandler handler = null;
            handler = GetSettingHandler(id);
            if (handler != null)
            {
                handler.OnApplySetting(id, value.ToString());
            }
            if (callbackDic.ContainsKey(id))
            {
                if (callbackDic[id] != null)
                {
                    callbackDic[id]();
                }
            }
        }

        // 把视频设置分离出来
        private void SetSettingValue(int id, object value)
        {
            if (value != null)
            {
                _configDic[id].value = value.ToString();
            }

        }

        public SettingHandler GetSettingHandler(int settingId)
        {
            if (IsVideoSetting(settingId))
                return videoSettingHandler;
            return null;
        }
        bool IsVideoSetting(int settingId)
        {
            return settingId > (int)VideoSettingId.Begin && settingId < (int)VideoSettingId.End;
        }


        #endregion

        #region update notify
        public string GetConfigById(int id)                                       //setting.xlsl 的Id字段
        {

            if (_configDic.ContainsKey(id) == false) return null;

            var item = _configDic[id];
            if(id == 98)
            {
                var setting = GetSettingItem(id).item;
                var value = setting.ComboxItemNames[int.Parse(item.value)];
                return value;
            }

            return item.value.ToString();


        }

        public void RegisterSettingCallBack(int id, Action func)
        {
            Action action;
            callbackDic.TryGetValue(id, out action);
            action += func;
            callbackDic[id] = action;
        }

        public void UnRegisterSettingCallBack(int id, Action func)
        {
            Action action;
            callbackDic.TryGetValue(id, out action);
            action -= func;
            callbackDic[id] = action;
        }
        #endregion

        #region local Cache

        private bool haveQualityLevelCache = true;
        public bool HaveQualityLevelCache
        {
            get { return haveQualityLevelCache; }
            set { haveQualityLevelCache = value; }
        }

        public void DeleteLocalCache()
        {
            PlayerPrefs.DeleteKey(LocalCacheKey);
        }

        public void SetSettingFromLocal(bool isSet = true)
        {
            _localCache.Clear();
            string str = string.Empty;
            try
            {
                str = PlayerPrefs.GetString(LocalCacheKey, string.Empty);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }


            var sourceDic = SettingConfigUtil.CovertString(str);

            // 应用一次所有设置项
            // 读取本地配置 替换默认配置 并应用
            var configDic = new Dictionary<int, string>();
            if(_configDic!=null)
            {
                if (!configDic.ContainsKey((int) VideoSettingId.Quality))
                {
                    haveQualityLevelCache = false;
                }
                foreach (var item in _configDic)
                {
                    configDic[item.Key] = item.Value.value;
                }
            }
            
            if(sourceDic!=null)
            {
                foreach (var item in sourceDic)
                {
                    configDic[item.Key] = item.Value;
                }
            }

            if (isSet)
            {
                SetSetting(configDic);
            }

            foreach (var pair in configDic)
            {
                _localCache[pair.Key] = pair.Value;
            }
        }

        public void SaveSettingToLocal()
        {
            try
            {
                string newStr = SettingConfigUtil.DictionaryToString(_localCache);
                PlayerPrefs.SetString(LocalCacheKey, newStr);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

        }
        #endregion

        #region Quality
        public void SetQuality(QualityLevel level)
        {
            if (level == QualityLevel.Undefined) return;
            SetSetting((int)VideoSettingId.Quality, (5-(int)level));

            FlushData();
        }


        public QualityLevel GetQualityBeforeInit()
        {
            var customQuality = GetQualityCustomLevel();
            if (customQuality != QualityLevel.Undefined)
                return customQuality;
            if (_localCache.Count == 0)
                SetSettingFromLocal(false);
            var id = (int)VideoSettingId.Quality;
            if (_localCache.ContainsKey((id)))
            {
                var cache = _localCache[id];
                return (QualityLevel)(5-int.Parse(cache.ToString()));
            }
            else
            {
                // 这里就是表里配的默认画质 
                return QualityLevel.High;
            }
        }


        public QualityLevel GetQuality()
        {
            if (_configDic == null || !_configDic.ContainsKey((int)VideoSettingId.Quality))
            {
                return QualityLevel.Undefined;
            }
            var setting = _configDic[(int)VideoSettingId.Quality];
            return (QualityLevel)(5-(int.Parse(setting.value.ToString())));
        }

        public void SetQualityByCustomLevel()
        {
            var quality = GetQualityCustomLevel();
            if (quality != QualityLevel.Undefined)
            {
                SetQuality(quality);
            }
        }

        private QualityLevel GetQualityCustomLevel()
        {
            var name = QualitySettings.names[QualitySettings.GetQualityLevel()];
            if (!name.StartsWith("QL_"))
                return QualityLevel.Undefined;
            var res = QualityLevel.Undefined;
            _qualityDict.TryGetValue(name, out res);
            return res;
        }

        public bool UseQualityCustomLevel()
        {
            return GetQualityCustomLevel() != QualityLevel.Undefined;
        }

        private Dictionary<string, QualityLevel> _qualityDict = new Dictionary<string, QualityLevel>
        {
            {"QL_Low", QualityLevel.Low},
            {"QL_MediumLow", QualityLevel.MediumLow},
            {"QL_Medium", QualityLevel.Medium},
            {"QL_MediumHigh", QualityLevel.MediumHigh},
            {"QL_High", QualityLevel.High},

        };

        public Dictionary<int, PlayerSettingItem> ConfigDic
        {
            get
            {
                return _configDic;
            }

            set
            {
                _configDic = value;
            }
        }

       
        #endregion

    }

    public class SettingConfigUtil
    {
        #region string util
        static public List<string> CovertAdditional(string str, char separator = ';')
        {
            if (str.Length < 1) return null;

            List<string> list = new List<string>();
            string[] subStrs = str.Split(separator);

            foreach (var subStr in subStrs)
            {
                if (str.IndexOf('=') > -1)
                {
                    string[] subsubStr = subStr.Split('=');
                    list.Add(subsubStr[1]);
                }
                else
                {
                    string[] subsubStr = subStr.Split(',');
                    foreach (var strValue in subsubStr)
                    {
                        list.Add(strValue);
                    }
                }
            }
            return list;
        }

        static public Dictionary<int, string> CovertString(string str, char separator = ';')
        {
            if (str.Length < 1)
            {
                return null;
            }

            Dictionary<int, string> dicInfo = new Dictionary<int, string>();
            string[] subStrs = str.Split(separator);

            foreach (var subStr in subStrs)
            {
                string[] subsubStr = subStr.Split('=');

                int key = int.Parse(subsubStr[0]);
                string value = subsubStr[1];
                dicInfo.Add(key, value);
            }

            return dicInfo;
        }

        public static string DictionaryToString(Dictionary<int, string> infoDic)
        {
            if (infoDic.Count == 0)
            {
                return string.Empty;
            }
            string str = string.Empty;
            foreach (var item in infoDic)
            {
                str += item.Key + "=" + item.Value;
                str += ";";
            }
            str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static string FloatListToString(float[] list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < list.Length; i++)
            {
                sb.Append(i + "=" + list[i] + ";");
            }

            string res = sb.ToString();
            return res.Substring(0, res.Length - 1);
        }

        public static string DictionaryToString(Dictionary<int, float> infoDic)
        {
            if (infoDic.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in infoDic)
            {
                sb.Append(item.Key + "=" + item.Value + ";");
            }

            string str = sb.ToString();
            str = str.Substring(0, str.Length - 1);
            return str;
        }
        #endregion
    }
}