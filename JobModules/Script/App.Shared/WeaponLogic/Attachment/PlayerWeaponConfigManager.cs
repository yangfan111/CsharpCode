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
        private List<Dictionary<WeaponPartsStruct, ExpandDefaultWeaponLogicConfig>> _configCache = new List<Dictionary<WeaponPartsStruct, ExpandDefaultWeaponLogicConfig>>();

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
            if(_configCache.Count < 1)
            {
                var cacheCount = _weaponDataConfigManager.ConfigCount + 1; 
                _configCache = new List<Dictionary<WeaponPartsStruct, ExpandDefaultWeaponLogicConfig>>(cacheCount);
                // 0没有数据
                _configCache.Add(null);
                for(int i = 1; i < cacheCount; i++)
                {
                    _configCache.Add(new Dictionary<WeaponPartsStruct, ExpandDefaultWeaponLogicConfig>(WeaponPartsStructComparer.Instance));
                }
            }
        }

        public ExpandDefaultWeaponLogicConfig GetWeaponLogicConfig(int id, WeaponPartsStruct weaponParts)
        {
            return GetAndCacheConfig(id, weaponParts);
        } 

        private ExpandDefaultWeaponLogicConfig GetAndCacheConfig(int id, WeaponPartsStruct weaponParts)
        {
            InitCache();
            if (id < 1 || id >= _configCache.Count)
            {
                Logger.ErrorFormat("id {0} is illegal", id);
                return null;
            }
            if(_configCache[id].ContainsKey(weaponParts))
            {
                return _configCache[id][weaponParts];
            }

            Prepare(weaponParts);
            var weaponConfig = _weaponDataConfigManager.GetConfigById(id);
            if(null == weaponConfig)
            {
                return null;
            }
            var baseConfig = weaponConfig.WeaponLogic;
            var srcConfig = (baseConfig as DefaultWeaponLogicConfig).FireLogic as DefaultFireLogicConfig;
            EFireMode[] srcFireModes = null; 
            if(null != srcConfig)
            {
                var cfg = srcConfig.FireModeLogic as DefaultFireModeLogicConfig;
                srcFireModes = cfg.AvaiableModes;
            }
            var targetConfig = baseConfig.Copy();
            //TODO 查找Copy失败的原因
            var tarConfig = (targetConfig as DefaultWeaponLogicConfig).FireLogic as DefaultFireLogicConfig;
            if(null != tarConfig)
            {
                var cfg = tarConfig.FireModeLogic as DefaultFireModeLogicConfig;
                cfg.AvaiableModes = new EFireMode[srcFireModes.Length];
                for(int i = 0; i < srcFireModes.Length; i++)
                {
                    cfg.AvaiableModes[i] = srcFireModes[i];
                }
            }
            ApplyAttachment(baseConfig, targetConfig);
            if(null != tarConfig)
            {
                var cfg = tarConfig.FireModeLogic as DefaultFireModeLogicConfig;
            }
 
            var detailConfig = new ExpandDefaultWeaponLogicConfig(targetConfig as DefaultWeaponLogicConfig);
            _configCache[id][weaponParts] = detailConfig;
            return detailConfig;
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
