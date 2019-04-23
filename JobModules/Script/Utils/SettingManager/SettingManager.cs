
using System;
using System.Collections.Generic;
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
        MediumHight,
        High,
    }
    public enum SettingId
    {
        Quality = 100,
        Shadow = 101,
        Chartlet = 103,
        ShaderDetail = 104,
        AntiAliasing = 105,
        VerticalSynchronization = 106,
        Lighting = 107,
        Details = 108,
    }
    public class SettingItem
    {
        public int Id;
        public int Type;
        public int ControlType;
        public string Description;
        public object Value;
        public object DefaultValue;
        public List<Dictionary<int,string>> SettingId;
        public List<int> Levels;
        public List<List<string>> Additional;
        public bool Invalid = false;
    }
    public class SettingManager : AbstractConfigManager<SettingManager>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SettingManager));
        private static string LocalCacheKey = "Setting";

        private Action<List<int>> onchangedCallback = null;

        private Dictionary<int, SettingItem> _configDic;
        private Dictionary<int, string> _localCache;
        private bool maxQualityMode = false;
        public SettingManager()
        {
            _localCache = new Dictionary<int, string>();
            GameQualitySettingManager.getValueListFunc = this.GetConfigById;
            maxQualityMode = QualitySettings.names[QualitySettings.GetQualityLevel()].ToLower().StartsWith("max_");
            _logger.InfoFormat("MaxQualityMode {0}", maxQualityMode? "yes": "not");
        }

        public static SettingManager GetInstance()
        {
            return SingletonManager.Get<SettingManager>();
        }
        #region manager init
        public override void ParseConfig(string xml)
        {
            var _config = XmlConfigParser<SettingConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }

            _configDic = new Dictionary<int, SettingItem>();
            foreach (var item in _config.Items)
            {
                var settingItem = new SettingItem()
                {
                    Id = item.Id,
                    Type = item.Type,
                    ControlType = item.ControlType,
                    Description = item.Desription,
                    DefaultValue = item.DefaultValue,
                    Value = item.DefaultValue,
                    Levels = item.ComboxItemKeys
                };
                settingItem.SettingId = new List<Dictionary<int, string>>();
                if (item.SettingId != null)
                {
                    for (var i = 0; i < item.SettingId.Count; i++)
                    {
                        settingItem.SettingId.Add(SettingConfigUtil.CovertString(item.SettingId[i], ','));
                    }
                }
                settingItem.Additional = new List<List<string>>();
                if (item.Additional != null)
                {
                    for (var i = 0; i < item.Additional.Count; i++)
                    {
                        settingItem.Additional.Add(SettingConfigUtil.CovertAdditional(item.Additional[i], ','));
                    }
                }

                _configDic[settingItem.Id] = settingItem;
            }
        }

        public void InitConfig(Dictionary<int, SettingItem> config)
        {
            _configDic = config;
        }

        public bool IsInitialized()
        {
            return _configDic != null;
        }
        

        #endregion

        #region config operate
        public void ResetSetting()
        {
            foreach (var item in _configDic)
            {
                if (item.Value.Value != item.Value.DefaultValue)
                {
                    item.Value.Value = item.Value.DefaultValue;
                    item.Value.Invalid = true;
                }
            }
        }

        public void FlushData(bool isForce = false)
        {
            _localCache.Clear();
            List<int> changedSettingList = new List<int>();
            foreach (var item in _configDic)
            {
                if (isForce || item.Value.Invalid)
                {
                    if(!maxQualityMode ||(maxQualityMode && item.Value.Type != 3))
                        changedSettingList.Add(item.Value.Id);
                }

                item.Value.Invalid = false;
                if (item.Value.Type == 3 || item.Value.Type == 4)
                {
                    if (item.Value.Value != null &&  item.Value.Value.Equals(item.Value.DefaultValue) == false) _localCache[item.Value.Id] = item.Value.Value.ToString();
                }
            }

            try
            {
                GameQualitySettingManager.onChanges(changedSettingList);
                if (onchangedCallback != null)
                    onchangedCallback(changedSettingList);
            }
            catch (Exception)
            {

            }

            SaveSettingToLocal();
        }

        public Dictionary<int, object> GetInvalidSettingValue(Dictionary<int , object> dic = null)
        {
            if (dic == null) dic = new Dictionary<int, object>();
            foreach (var item in _configDic)
            {
                if (item.Value.Invalid)
                {
                    dic[item.Key] = item.Value.Value;
                }
            }
            return dic;
        }

        public object GetSettingValue(int id)
        {
            return _configDic[id].Value;
        }
        public SettingItem GetSettingItem(int id)
        {
            return _configDic[id];
        }

        public void SetSettingValue(int id, object value)
        {
            if (_configDic.ContainsKey(id) == false)
            {
                _logger.WarnFormat("invalid id, setting id {0}", id);
                return;
            }

            if (_configDic[id].Value.Equals(value)) return;
            _configDic[id].Value = value;
            _configDic[id].Invalid = true;
            if (_configDic[id].SettingId != null && _configDic[id].SettingId.Count > 0)
            {
                int index = 0;
                try
                {
                    index = int.Parse(value.ToString());
                }
                catch (Exception e)
                {
                }

                foreach (var item in _configDic[id].SettingId[index])
                {
                    SetSettingValue(item.Key, item.Value);
                }
            }
        }
        #endregion

        #region update notify
        public string[] GetConfigById(int id)                                       //setting.xlsl 的Id字段
        {
            List<string> strList = new List<string>();
            if (_configDic.ContainsKey(id) == false) return strList.ToArray();

            var item = _configDic[id];
            strList.Add(item.Value.ToString());

            if (item.Additional != null && item.Additional.Count > 0)
            {
                var addis = item.Additional[int.Parse(item.Value.ToString())];
                foreach (var addi in addis)
                {
                    strList.Add(addi);
                }
            }
            
            return strList.ToArray();
        }

        public void RegisterSettingCallBack(Action<List<int>> func)
        {
            onchangedCallback += func;
        }

        public void UnRegisterSettingCallBack(Action<List<int>> func)
        {
            onchangedCallback -= func;
        }
        #endregion

        #region local Cache

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
            if (sourceDic == null)
                return;
            foreach (var pair in sourceDic)
            {
                if(isSet)
                SetSettingValue(pair.Key, pair.Value);
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

        public void SetQuality(QualityLevel level)
        {
            if (level == QualityLevel.Undefined) return;
            SetSettingValue((int)SettingId.Quality, (int)level - 1);
            FlushData();
        }

        public QualityLevel GetQualityBeforeInit()
        {
            if(_localCache.Count == 0)
            SetSettingFromLocal(false);
            var id = (int) (int) SettingId.Quality;
            if (_localCache.ContainsKey((id)))
            {
                var cache = _localCache[id];
                return (QualityLevel)(int.Parse(cache.ToString()) + 1);
            }
            else
            {
                return QualityLevel.Low;
            }
        }


        public QualityLevel GetQuality()
        {
            if (_configDic == null || !_configDic.ContainsKey((int) SettingId.Quality))
            {
                return QualityLevel.Undefined;
            }
            var setting = _configDic[(int) SettingId.Quality];
            return (QualityLevel)(int.Parse(setting.Value.ToString()) + 1);
        }
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
            return res.Substring(0,res.Length - 1);
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