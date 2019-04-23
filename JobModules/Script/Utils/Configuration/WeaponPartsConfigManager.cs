using System.Collections.Generic;
using XmlConfig;
using System;
using Shared.Scripts;
using WeaponConfigNs;
using AssetInfo = Utils.AssetManager.AssetInfo;
using Core.Utils;

namespace Utils.Configuration
{



    public class WeaponPartsConfigManager : AbstractConfigManager<WeaponPartsConfigManager>, IConfigParser
    {
        private readonly Dictionary<int, WeaponPartsConfigItem>
            partItems = new Dictionary<int, WeaponPartsConfigItem>();

        /// <summary>
        /// 武器-可用配件Id列表
        /// </summary>
        private readonly Dictionary<int, List<int>> matchedWeaponPartIds = new Dictionary<int, List<int>>();

        /// <summary>
        /// 武器-可用配件槽位列表
        /// </summary>
        private readonly Dictionary<int, List<EWeaponPartType>> matchedWeaponPartDefaultSlots =
            new Dictionary<int, List<EWeaponPartType>>();


        private Dictionary<int, AttachedAttributeSet> partItemAttributes = new Dictionary<int, AttachedAttributeSet>();
        private readonly List<EWeaponPartType> EmptyList = new List<EWeaponPartType>();



        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<WeaponPartsConfig>.Load(xml);
            foreach (var item in cfg.Items)
            {
                partItems[item.Id] = item;
                if (!matchedWeaponPartIds.ContainsKey(item.Apply))
                    matchedWeaponPartIds[item.Apply] = new List<int>();
                matchedWeaponPartIds[item.Apply].Add(item.Id);
                if (!matchedWeaponPartDefaultSlots.ContainsKey(item.Apply))
                    matchedWeaponPartDefaultSlots[item.Apply] = new List<EWeaponPartType>();
                if (item.Default < 1)
                    matchedWeaponPartDefaultSlots[item.Apply].Add((EWeaponPartType) item.Type);
            }

        }

        public Dictionary<int, WeaponPartsConfigItem> GetConfigs()
        {
            return partItems;
        }

        public WeaponPartsConfigItem GetConfigById(int id)
        {
            if (partItems.ContainsKey(id))
            {
                return partItems[id];
            }

            Logger.WarnFormat("config for id {0} does not exist ", id);
            return null;
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

        public bool IsPartMatchWeapon(int partId, int weaponId)
        {
            if (!matchedWeaponPartIds.ContainsKey(weaponId))
            {
                return false;
            }

            return matchedWeaponPartIds[weaponId].Contains(partId);

        }

        /// <summary>
        /// 获取可用的武器槽列表
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns>可用的武器槽列表，对应的位置的值为是否可用</returns>
        public List<EWeaponPartType> GetAvaliablePartTypes(int weaponId)
        {

            if (matchedWeaponPartDefaultSlots.ContainsKey(weaponId))
            {
                return matchedWeaponPartDefaultSlots[weaponId];
            }

            return EmptyList;
        }

        public AssetInfo GetAsset(int id)
        {
            var cfg = GetConfigById(id);
            if (null == cfg)
            {
                return new AssetInfo();
            }

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
                    case XmlConfig.EWeaponPartType.Magazine:
                    {
                        result = "弹匣";
                    }
                        break;
                    case XmlConfig.EWeaponPartType.Muzzle:
                    {
                        result = "枪口";
                    }
                        break;
                    case XmlConfig.EWeaponPartType.UpperRail:
                    {
                        result = "导轨";
                    }
                        break;
                    case XmlConfig.EWeaponPartType.SideRail:
                    {
                        result = "侧导轨";
                    }
                        break;
                    case XmlConfig.EWeaponPartType.LowerRail:
                    {
                        result = "握把";
                    }
                        break;
                    case XmlConfig.EWeaponPartType.Stock:
                    {
                        result = "枪托";
                    }
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
            GetPartAchiveAttachedAttributeByType(achive.Magazine, attributeType, ref attachedVal);
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
            if (partItemAttributes.ContainsKey(id))
                return partItemAttributes[id];
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
