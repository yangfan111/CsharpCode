using Assets.Utils.Configuration;
using Core.Compare;
using System.Collections.Generic;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace Core.Configuration
{
    public interface IWeaponConfigManager
    {
        EFireMode GetFirstAvaliableFireMode(int id);
    }

    public class WeaponDataConfigManager : AbstractConfigManager<WeaponDataConfigManager>, IWeaponConfigManager
    {
        private Dictionary<int, WeaponConfig> _configCache = new Dictionary<int, WeaponConfig>();
        private Dictionary<int, DefaultFireModeLogicConfig> _fireModeCountCache = new Dictionary<int, DefaultFireModeLogicConfig>();
        private Dictionary<int, int> _fakeConfigCache = new Dictionary<int, int>();

        private WeaponConfigs _configs = null;
        private int[] _emptyIntArray = new int[0];
        public string ConfigName;

        public WeaponConfig Configs
        {
            get
            {
                for(int i = 0; i < _configs.Weapons.Length; i++)
                {
                    if(_configs.Weapons[i].Name == ConfigName)
                    {
                        return _configs.Weapons[i];
                    }
                }
                return _configs.Weapons[0];
            }
            set
            {
                for (int i = 0; i < _configs.Weapons.Length; i++)
                {
                    if (_configs.Weapons[i].Name == ConfigName)
                    {
                        _configs.Weapons[i] = value;
                        break;
                    }
                }
            }
        }

        public void InitFakeConfig()
        {
            var cfgs = SingletonManager.Get<WeaponConfigManager>().GetConfigs();
            foreach(var cfg in cfgs)
            {
                if(_configCache.ContainsKey(cfg.Key))
                {
                    _fakeConfigCache[cfg.Value.SubType] = _configCache[cfg.Key].Id; 
                }
            }
        }

        public override void ParseConfig(string xml)
        {
            _configs = XmlConfigParser<WeaponConfigs>.Load(xml);
            foreach (var item in _configs.Weapons)
            {
                _configCache[item.Id] = item;
            }
        }

        public WeaponConfig GetConfigById(int id)
        {
            if (_configCache.ContainsKey(id))
            {
                return _configCache[id];
            }
            else
            {
                Logger.ErrorFormat("{0} does not exist in weapon config ", id);
                return null;
            }
        }

        public bool HasAutoFireMode(int id)
        {
            var cfg = GetFireModeConfig(id);
            if (null == cfg) return false;
            foreach(var mode in cfg.AvaiableModes)
            {
                if(mode == EFireMode.Auto)
                {
                    return true;
                }
            }
            return false;
        } 

        public EFireMode GetFirstAvaliableFireMode(int id)
        {
            var cfg = GetFireModeConfig(id);
            if(null == cfg || cfg.AvaiableModes.Length < 1)
            {
                return EFireMode.Manual;
            }
            return cfg.AvaiableModes[0]; 
        }

        public int GetFireModeCountById(int id)
        {
            var cfg = GetFireModeConfig(id);
            if(null == cfg)
            {
                return 1;
            }
            return cfg.AvaiableModes.Length;
        }

        public DefaultFireModeLogicConfig GetFireModeConfig(int id)
        {
            var cfg = GetConfigById(id);
            if(null == cfg)
            {
                return null;
            }
            if(_fireModeCountCache.ContainsKey(id))
            {
                return _fireModeCountCache[id];
            }
            var defCfg = cfg.WeaponLogic as DefaultWeaponLogicConfig;
            if (null != defCfg)
            {
                var fireCfg = defCfg.FireLogic as DefaultFireLogicConfig;
                if (null != fireCfg)
                {
                    var fireModeCfg = fireCfg.FireModeLogic as DefaultFireModeLogicConfig;
                    if (null != fireModeCfg)
                    {
                        _fireModeCountCache[id] = fireModeCfg;
                        return fireModeCfg;
                    }
                }
            }
            return null;
        }

        public WeaponConfigs GetConfigs()
        {
            return _configs;   
        }

        public bool IsAttachmentMatch(int id, int attachId )
        {
            if(!_configCache.ContainsKey(id))
            {
                Logger.ErrorFormat("{0} does not exist in weapon config ", id);
                return false;
            }
            var config = _configCache[id];
            var attachs = config.WeaponLogic.AttachmentConfig;
            for(int i = 0; i < attachs.Length; i++)
            {
                if(attachs[i] == attachId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
