using Shared.Scripts;
using System;
using System.Collections.Generic;
using WeaponConfigNs;
using XmlConfig;
using AssetInfo = Utils.AssetManager.AssetInfo;

namespace Utils.Configuration
{
    public class WeaponPartsConfigManager : AbstractConfigManager<WeaponPartsConfigManager>, IConfigParser
    {
        private readonly Dictionary<int, WeaponPartsConfigItem>
            partItems = new Dictionary<int, WeaponPartsConfigItem>();




        private Dictionary<int, AttachedAttributeSet> partItemAttributes = new Dictionary<int, AttachedAttributeSet>();
        private readonly List<EWeaponPartType> EmptyList = new List<EWeaponPartType>();



        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<WeaponPartsConfig>.Load(xml);
            foreach (var item in cfg.Items)
            {
                partItems[item.Id] = item;
            }

        }

        public Dictionary<int, WeaponPartsConfigItem> GetConfigs()
        {
            return partItems;
        }

        public WeaponPartsConfigItem GetConfigById(int id)
        {
            WeaponPartsConfigItem configItem;
            partItems.TryGetValue(id, out configItem);
            return configItem; 
        }

        public float GetSubType(int id)
        {
            if (id > 0)
            {
                var config = GetConfigById(id);
                if (config != null)
                {
                    return config.SubType - 1; //策划配表'1'起始
                }
            }

            return 0;
        }

        public float GetScopeOffset(int id)
        {
            if (FirstPersonOffsetScript.UpdateOffset)
            {
                return FirstPersonOffsetScript.StaticScopeOffset;
            }

            if (id > 0)
            {
                var config = GetConfigById(id);
                if (config != null)
                {
                    return config.AimOffset;
                }
            }

            return 0;
        }

        public float GetSightModelScale(int id)
        {
            if (FirstPersonOffsetScript.StaticScopeScale != 0)
            {
                return FirstPersonOffsetScript.StaticScopeScale;
            }

            var config = GetConfigById(id);
            if (null != config)
            {
                return config.Scale;
            }

            return 1;
        }


        /// <summary>
        /// 获取可用的武器槽列表
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns>可用的武器槽列表，对应的位置的值为是否可用</returns>

        public AssetInfo GetAsset(int id)
        {
            var cfg = GetConfigById(id);
            if (null == cfg)
                return AssetInfo.EmptyInstance;
            return new AssetInfo
            {
                BundleName = cfg.Bundle,
                AssetName = cfg.Res
            };
        }

        public EWeaponPartType GetPartType(int id)
        {
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                return (EWeaponPartType) cfg.Type;
            }

            return EWeaponPartType.None;
        }

        public string GetTypeStrById(int id)
        {
            string result = "";
            EWeaponPartType type = EWeaponPartType.None;
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                type = (EWeaponPartType) cfg.Type;
                switch (type)
                {
                    case EWeaponPartType.Magazine:
                        result = "弹匣";
                        break;
                    case EWeaponPartType.Muzzle:
                        result = "枪口";
                        break;
                    case EWeaponPartType.UpperRail:
                        result = "导轨";
                        break;
                    case EWeaponPartType.SideRail:
                        result = "侧导轨";
                        break;
                    case EWeaponPartType.LowerRail:
                        result = "握把";
                        break;
                    case EWeaponPartType.Stock:
                        result = "枪托";
                        break;
                    case EWeaponPartType.Bore:
                        result = "枪膛";
                        break;
                    case EWeaponPartType.Feed:
                        result = "供弹";
                        break;
                    case EWeaponPartType.Trigger:
                        result = "击发";
                        break;
                    case EWeaponPartType.Interlock:
                        result = "闭锁";
                        break;
                    case EWeaponPartType.Brake:
                        result = "制退";
                        break;
                }
            }

            return result;
        }

        public float GetPartAchiveAttachedAttributeByType(WeaponPartsAchive achive, WeaponAttributeType attributeType)
        {
            float attachedVal = 0f;
            GetPartAchiveAttachedAttributeByType(achive.UpperRail, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Stock, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Magazine, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Muzzle, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.LowerRail, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.SideRail, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Bore, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Feed, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Trigger, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Interlock, attributeType, ref attachedVal);
            GetPartAchiveAttachedAttributeByType(achive.Brake, attributeType, ref attachedVal);
            return attachedVal;
        }

        public const float AttributeInvalidConst = -999f;

        private void GetPartAchiveAttachedAttributeByType(int partId, WeaponAttributeType attributeType,
            ref float attrVal)
        {
            if (partId > 0)
            {
                var result = GetPartAttachedAttributes(partId).GetAttribute(attributeType);
                if (result != AttributeInvalidConst) // (Math.Abs(result) < Math.Abs(attrVal)|| attrVal==0f))
                {
                    attrVal = attrVal == 0f ? result : attrVal * result;
                }
            }
        }

        public float GetPartAchiveAttachedAttributeByType(int partId, WeaponAttributeType attributeType)
        {
            if (partId == 0) return 0;
            return GetPartAttachedAttributes(partId).GetAttribute(attributeType);
        }


        public AttachedAttributeSet GetPartAttachedAttributes(int id)
        {
            AttachedAttributeSet attributeSet;
            if (partItemAttributes.TryGetValue(id,out attributeSet))
                return attributeSet;
            var cfg = GetConfigById(id);
            if(cfg ==null )
                return null;
            partItemAttributes[id] = new AttachedAttributeSet();
            var attrType = typeof(WeaponAttributeType);
            var names = Enum.GetNames(attrType);
            for(int i =0; i < names.Length; i++)
            {
                var field = cfg.GetType().GetField(names[i]);
                if(null != field) 
                {
                    var val = field.GetValue(cfg);
                    var type = field.FieldType;
                    if(type == typeof(int))
                    {
                        var intVal = (int)val;
                        if(intVal != 0)
                        {
                            var item = new AttachedAttributeItem();
                            item.Type = (WeaponAttributeType)Enum.Parse(attrType, names[i]);
                            item.Val = intVal;
                            partItemAttributes[id].AddAttribute(item);
                        }
                    }
                    else if(type == typeof(float))
                    {
                        var floatVal = (float)val;
                        if(Math.Abs(floatVal) > 0)
                        {
                            var item = new AttachedAttributeItem();
                            item.Type = (WeaponAttributeType)Enum.Parse(attrType, names[i]);
                            item.Val = floatVal;
                            partItemAttributes[id].AddAttribute(item);
                        }
                    }
                    else
                    {
                        Logger.ErrorFormat("filed type is illegal {0}", type);
                    }
                }
            }
            return partItemAttributes[id];
        }
    }
}
