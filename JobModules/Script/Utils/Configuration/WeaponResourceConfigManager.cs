using Assets.XmlConfig;
using Shared.Scripts;
using System.Collections.Generic;
using Utils.Configuration;
using WeaponConfigNs;
using XmlConfig;

namespace Assets.Utils.Configuration
{

    /// <summary>
    /// 目前使用配件和资源的配置，数值配置在WeaponResourceConfigManager
    /// </summary>
    public class WeaponResourceConfigManager : AbstractConfigManager<WeaponResourceConfigManager>, IConfigParser
    {
        private Dictionary<int, WeaponResConfigItem> _weaponDic = new Dictionary<int, WeaponResConfigItem>();
        private readonly int[] _emptyIntArray = new int[0];
        private int? C4Id;
        public int EmptyHandId { get; private set; }
        public int MeleeVariant { get; private set; }

        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<NewWeaponConfig>.Load(xml);
            foreach (var item in cfg.Items)
            {
                _weaponDic[item.Id] = item;
                switch((EWeaponSubType)item.SubType)
                {
                    case EWeaponSubType.C4:
                        C4Id = item.Id;
                        break;
                    case EWeaponSubType.Hand:
                        EmptyHandId = item.Id;
                        break;
                    case EWeaponSubType.Melee:
                        switch (item.Id) {
                            case (int)EWeaponIdType.MeleeVariant:
                                MeleeVariant = item.Id;
                                break;
                        }
                        break;
                }
            }
            if(!C4Id.HasValue)
            {
                Logger.Error("config for c4 doesn't exist");
            }
         
        }

        public Dictionary<int, WeaponResConfigItem> GetConfigs()
        {
            return _weaponDic;
        } 

        public WeaponResConfigItem GetConfigById(int id)
        {
            if(!_weaponDic.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null;
            }
            return _weaponDic[id];
        }

        public bool IsC4(int weaponId)
        {
            if(!C4Id.HasValue)
            {
                return false;
            }
            return C4Id.Value == weaponId;
        }

        public bool IsArmors(int weaponId)
        {
            var config = GetConfigById(weaponId);
            if (config != null)
            {
                return config.Type == (int) EWeaponType_Config.Armor || config.Type == (int) EWeaponType_Config.Helmet;
            }
            return false;
        }

        public bool IsArmor(int weaponId)
        {
            var config = GetConfigById(weaponId);
            if (config != null)
            {
                return config.Type == (int) EWeaponType_Config.Armor;
            }
            return false;
        }

        public bool IsHelmet(int weaponId)
        {
            var config = GetConfigById(weaponId);
            if (config != null)
            {
                return config.Type == (int) EWeaponType_Config.Helmet;
            }
            return false;
        }
        
        public int GetAvatarByWeaponId(int id)
        {
            if(!_weaponDic.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return 0;
            }
            return _weaponDic[id].AvatorId;
        }

        public bool IsSpecialType(int id, ESpecialWeaponType specialType)
        {
            var config = GetConfigById(id);
            if(null == config || config.SpecialType == null)
            {
                return false;
            }
            foreach(var type in config.SpecialType)
            {
                if(type == (int)specialType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool NeedActionDeal(int id, ActionDealEnum action)
        {
            var config = GetConfigById(id);
            if(null == config)
            {
                return false;
            }
            var actions = config.ActionDeal;
            if(null == actions)
            {
                return false;
            }
            foreach (int item in actions)
            {
                if ((int)action == item)
                {
                    return true;
                }
            }

            return false;
        }

        public float GetAimModelScale(int id)
        {
            if(FirstPersonOffsetScript.StaticAimScale != 0)
            {
                return FirstPersonOffsetScript.StaticAimScale;
            }
            var cfg = GetConfigById(id);
            if(null == cfg)
            {
                return 1;
            }
            return cfg.AimModelScale;
        }

        public int[] GetDefaultWeaponAttachments(int id)
        {
            var cfg = GetConfigById(id);
            if(null == cfg || null == cfg.Parts)
            {
                return _emptyIntArray;
            }
            return cfg.Parts;
        }

   
    }
}
