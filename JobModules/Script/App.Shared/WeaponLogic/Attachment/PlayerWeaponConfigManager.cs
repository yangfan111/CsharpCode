using Core.Configuration;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using Utils.Configuration;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace Core.WeaponLogic.Attachment
{
    /// <summary>
    /// 动态缓存的武器配置，通过武器Id和配件来索引
    /// 缓存了配置数据，一般来说配置不变的话，不需要清理缓存的数据
    /// 目前只支持DefaultWeaponLogicConfig
    /// </summary>
    public class PlayerWeaponConfigManager : IPlayerWeaponConfigManager
    {

        private class IdComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            public int GetHashCode(int obj)
            {
                return obj;
            }
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponConfigManager));
        private delegate void Action<T1,T2,T3,T4,T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        IWeaponPartsConfigManager _attachConfigManager;
        IWeaponDataConfigManager _weaponDataConfigManager;

        public PlayerWeaponConfigManager(IWeaponPartsConfigManager configManager, IWeaponDataConfigManager weaponDataConfigManager)
        {
            _attachConfigManager = configManager;
            _weaponDataConfigManager = weaponDataConfigManager;
        }

        private Dictionary<WeaponAttributeType, float?> _attachAttributeDic = new Dictionary<WeaponAttributeType, float?>(CommonIntEnumEqualityComparer<WeaponAttributeType>.Instance);
        private Dictionary<int, Dictionary<WeaponPartsStruct, ExpandWeaponLogicConfig>> _configCache = new Dictionary<int, Dictionary<WeaponPartsStruct, ExpandWeaponLogicConfig>>(new IdComparer());

        private List<int> _attachmentList = new List<int>();

        private void Prepare(WeaponPartsStruct attachments)
        {
            Reset();
            _attachmentList.Add(attachments.UpperRail);
            _attachmentList.Add(attachments.LowerRail);
            _attachmentList.Add(attachments.Muzzle);
            _attachmentList.Add(attachments.Magazine);
            _attachmentList.Add(attachments.Stock);
            for(var i = 0; i < _attachmentList.Count; i++)
            {
                if(_attachmentList[i] < 1)
                {
                    continue;
                }
                var modifiedInfos = _attachConfigManager.GetModifyInfos(_attachmentList[i]);
                if(null == modifiedInfos)
                {
                    continue;
                }
                foreach (var info in modifiedInfos)
                {
                    if(_attachAttributeDic[info.Type].HasValue)
                    {
                        _attachAttributeDic[info.Type] += info.Val; 
                    }
                    else
                    {
                        _attachAttributeDic[info.Type] = info.Val; 
                    }
                }
            }
        }

        private void InitCache()
        {
            if(null == _configCache)
            {
                _configCache = new Dictionary<int, Dictionary<WeaponPartsStruct, ExpandWeaponLogicConfig>>(new IdComparer());
            }
        }

        public ExpandWeaponLogicConfig GetWeaponLogicConfig(int id, WeaponPartsStruct weaponParts)
        {
            return GetAndCacheConfig(id, weaponParts);
        } 

        private ExpandWeaponLogicConfig GetAndCacheConfig(int id, WeaponPartsStruct weaponParts)
        {
            InitCache();
            Dictionary<WeaponPartsStruct, ExpandWeaponLogicConfig> partsDic;
            if(_configCache.TryGetValue(id, out partsDic))
            {
                ExpandWeaponLogicConfig config;
                if(partsDic.TryGetValue(weaponParts, out config))
                {
                    return config;
                }
            }

            Prepare(weaponParts);
            var weaponConfig = _weaponDataConfigManager.GetConfigById(id);
            if(null == weaponConfig)
            {
                Logger.ErrorFormat("WeaponConfig is null for {0}", id);
                return null;
            }
            var baseConfig = weaponConfig.WeaponLogic;
            var targetConfig = baseConfig.Copy();
            ApplyAttachment(baseConfig, targetConfig);
            if(!_configCache.ContainsKey(id))
            {
                _configCache[id] = new Dictionary<WeaponPartsStruct, ExpandWeaponLogicConfig>(WeaponPartsStructComparer.Instance);
            }
 
            var defaultWeaponLogicCfg = targetConfig as DefaultWeaponLogicConfig;
            if(null != defaultWeaponLogicCfg)
            {
                var expandConfig  = new ExpandWeaponLogicConfig(defaultWeaponLogicCfg);
                _configCache[id][weaponParts] = expandConfig;
                return expandConfig;
            }
            else
            {
                var tacticWeaponLogicCfg = targetConfig as TacticWeaponLogicConfig;
                if(null != tacticWeaponLogicCfg)
                {
                    var expandConfig  = new ExpandWeaponLogicConfig(tacticWeaponLogicCfg);
                    _configCache[id][weaponParts] = expandConfig;
                    return expandConfig;
                }
            }
            Logger.ErrorFormat("illegal weapon config for {0}", id);
            return null;
        }

        private void Reset()
        {
            _attachmentList.Clear();
            for(var type =  WeaponAttributeType.Bullet; type < WeaponAttributeType.Length; type++)
            {
                _attachAttributeDic[type] = null;
            }
        }

        private void ApplyAttachment(WeaponLogicConfig baseConfig, WeaponLogicConfig targetConfig)
        {
            FindAndChangePartRecursive(baseConfig, targetConfig, _attachAttributeDic);
        }

        private Dictionary<Type, Action<PropertyInfo, object, object, float, PartModifyType>> _modifyActionDic = new Dictionary<Type, Action<PropertyInfo, object, object, float, PartModifyType>>
        {
            {
                typeof(float), (property, srcConfig, config, val, modifyType)=>
                {
                    var last = (float)property.GetValue(srcConfig, null);
                    switch(modifyType)
                    {
                        case PartModifyType.Scale:
                            if(last == 0)
                            {
                                last = 1;
                            }
                            property.SetValue(config, last * val, null);
                            break;
                        case PartModifyType.Add:
                            property.SetValue(config, last + val, null);
                            break;
                        case PartModifyType.Replace:
                            property.SetValue(config, val, null);
                            break;
                    }
                }
            },
            {
                typeof(int), (property, srcConfig, config, val, modifyType)=>
                {
                    var last = (int)property.GetValue(srcConfig, null);
                    switch(modifyType)
                    {
                        case PartModifyType.Scale:
                            if(last == 0)
                            {
                                last = 1;
                            }
                            property.SetValue(config, (int)(last * val), null);
                            break;
                        case PartModifyType.Add:
                            property.SetValue(config, (int)(last + val), null);
                            break;
                        case PartModifyType.Replace:
                            if(val != 0)
                            {
                                property.SetValue(config, (int)val, null);
                            }
                            break;
                    }
                }
            },
        };

        private Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();
        private Dictionary<PropertyInfo, object[]> _customAttributeCache = new Dictionary<PropertyInfo, object[]>();

        private void FindAndChangePartRecursive(Object baseConfig, object targetConfig, Dictionary<WeaponAttributeType, float?> changeDic)
        {
            var type = baseConfig.GetType();
            if(!_propertyCache.ContainsKey(type))
            {
                _propertyCache[type] = type.GetProperties();
            }
            var properties = _propertyCache[type];
            foreach(var property in properties)
            {
                var baseSubConfig = property.GetValue(baseConfig, null);
                var targetSubConfig = property.GetValue(targetConfig, null);
                if(!_customAttributeCache.ContainsKey(property))
                {
                    _customAttributeCache[property] = property.GetCustomAttributes(typeof(ChangeByPartAttribute), false);
                }
                var changeByPartAttributes = _customAttributeCache[property];
                if(changeByPartAttributes.Length > 0 )
                {
                    foreach(ChangeByPartAttribute attribute in changeByPartAttributes)
                    {
                        float? val; 
                        if (changeDic.TryGetValue(attribute.AttributeType, out val))
                        {
                            if(!val.HasValue)
                            {
                                property.SetValue(targetConfig, property.GetValue(baseConfig, null), null);
                            }
                            else
                            {
                                var valType = baseSubConfig.GetType();
                                if(_modifyActionDic.ContainsKey(valType))
                                {
                                    _modifyActionDic[valType](property, baseConfig, targetConfig, val.Value, attribute.ModifyType);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(null == baseSubConfig)
                    {
                        continue;
                    }
                    else 
                    {
                        var subType = baseSubConfig.GetType();
                        //只处理子配置，不处理列表和数组
                        if(subType.IsClass && !subType.IsArray && !subType.IsGenericType)
                        {
                            FindAndChangePartRecursive(baseSubConfig, targetSubConfig, changeDic);
                        }
                    }
                }
            }
        }
    }
}
