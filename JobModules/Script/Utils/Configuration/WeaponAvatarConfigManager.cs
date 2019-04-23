using Core.Utils;
using System.Collections.Generic;
using Utils.AssetManager;
using Utils.CharacterState;
using XmlConfig;

namespace Utils.Configuration
{

    public class WeaponAvatarConfigManager :  AbstractConfigManager<WeaponAvatarConfigManager>, IConfigParser
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponAvatarConfigManager));
        private WeaponAvatarConfig _config;
        private Dictionary<int, WeaponAvatarConfigItem> _configItems = new Dictionary<int, WeaponAvatarConfigItem>();
        private Dictionary<AssetInfo, UnityEngine.Object> _assetPool = new Dictionary<AssetInfo, UnityEngine.Object>(AssetInfo.AssetInfoComparer.Instance);
        public HashSet<AssetInfo> AnimSubResourceAssetInfos = new HashSet<AssetInfo>(AssetInfo.AssetInfoComparer.Instance);
        public const int EmptyHand = 13;
        WeaponAvatarConfigItem _noWeaponItem;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<WeaponAvatarConfig>.Load(xml);
            foreach(var item in _config.Items)
            {
                _configItems[item.Id] = item;
                if(item.SubType == EmptyHand)
                {
                    _noWeaponItem = item;
                }
            }
            if(null == _noWeaponItem)
            {
                Logger.Error("No hand config exist in weapon avatar config ");
            }


            foreach (var item in _config.Items)
            {
                if (!string.IsNullOrEmpty(item.AnimBundle))
                {
                    if (!string.IsNullOrEmpty(item.AnimFemaleP1))
                    {
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimFemaleP1));
                    }
                    if (!string.IsNullOrEmpty(item.AnimFemaleP3))
                    {
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimFemaleP3));
                    }
                    if (!string.IsNullOrEmpty(item.AnimMaleP1))
                    {
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP1));
                    }
                    if (!string.IsNullOrEmpty(item.AnimMaleP3))
                    {
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP3));
                    }
                }
            }
        }

        public void AddToAssetPool(AssetInfo asset, UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (_assetPool.ContainsKey(asset))
                {
                    Logger.DebugFormat("duplicate WeaponAvatarConfigManager assetPool {0}, type {1}", asset, obj.GetType());
                }
                else
                {
                    Logger.DebugFormat("adding asset to WeaponAvatarConfigManager assetPool {0}, type {1}", asset, obj.GetType());
                    _assetPool[asset] = obj;
                }
            }
        }


        public UnityEngine.Object GetOrNull(AssetInfo assetInfo)
        {
            UnityEngine.Object ret;
            _assetPool.TryGetValue(assetInfo, out ret);
            return ret;
        }

        public WeaponAvatarConfigItem GetConfigById(int id)
        {
            if(_configItems.ContainsKey(id))
            {
                return _configItems[id];
            }
            Logger.ErrorFormat("avatar id {0} doesn't exist in config", id);
            return null;
        }

        public AssetInfo GetThirdPersonModel(int id)
        {
            var cfg = GetConfigById(id);
            if(null != cfg)
            {
                return new AssetInfo(cfg.ModelBundle, cfg.ResP3);
            }
            return new AssetInfo();
        }
        public List<AssetInfo> GetEffectAsset(int id)
        {
            List<AssetInfo> list = new List<AssetInfo>();
            var cfg = GetConfigById(id);
            if (null != cfg && null != cfg.EffectBundle && cfg.EffectBundle.Length > 0)
            {
                if (null != cfg.SpecialEffect && cfg.SpecialEffect.Count > 0)
                {
                    foreach (string effect in cfg.SpecialEffect)
                    {
                        list.Add(new AssetInfo(cfg.EffectBundle, effect));
                    }
                }
            }
            return list;
        }
        public AssetInfo GetFirstPersonModel(int id)
        {
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                return new AssetInfo(cfg.ModelBundle, cfg.ResP1);
            }
            return new AssetInfo();
        }

        public AssetInfo GetThirdPersonAnimation(int id, Sex sex)
        {
            var cfg = GetConfigById(id);
            if(null != cfg)
            {
                switch (sex)
                {
                    case Sex.Male:
                        return new AssetInfo(cfg.AnimBundle, cfg.AnimMaleP3);
                    case Sex.Female:
                        return new AssetInfo(cfg.AnimBundle, cfg.AnimFemaleP3);
                }
            }
            return new AssetInfo();
        }

        public AssetInfo GetFirstPersonAnimation(int id, Sex sex)
        {
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                switch (sex)
                {
                    case Sex.Male:
                        return new AssetInfo(cfg.AnimBundle, cfg.AnimFemaleP1);
                    case Sex.Female:
                        return new AssetInfo(cfg.AnimBundle, cfg.AnimMaleP1);
                }
            }
            return new AssetInfo();
        }

        public bool GetLeftHandIK(int id)
        {
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                return cfg.LeftHandIK;
            }
            return false;
        }

        public float GetSightDistance(int id)
        {
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                return cfg.SightDistance;
            }
            return 0;
        }

        public AssetInfo GetEmptyHandedFirstPersonAnim(Sex sex)
        {
            switch(sex)
            {
                case Sex.Female:
                    return new AssetInfo(_noWeaponItem.AnimBundle, _noWeaponItem.AnimFemaleP1);
                case Sex.Male:
                    return new AssetInfo(_noWeaponItem.AnimBundle, _noWeaponItem.AnimMaleP1);
            }
            return new AssetInfo();
        }

        public int GetEmptyHandId()
        {
            if(null != _noWeaponItem)
            {
                return _noWeaponItem.Id;
            }
            Logger.Error("no hand item exist ");
            return 0;
        }

        public AssetInfo GetEmptyHandedThirdPersonAnim(Sex sex)
        {
            switch (sex)
            {
                case Sex.Female:
                    return new AssetInfo(_noWeaponItem.AnimBundle, _noWeaponItem.AnimFemaleP3);
                case Sex.Male:
                    return new AssetInfo(_noWeaponItem.AnimBundle, _noWeaponItem.AnimMaleP3);
            }
            return new AssetInfo(); ;
        }

        public AssetInfo GetIcon(int id)
        {
            var cfg = GetConfigById(id);
            if (cfg != null)
            {
                return new AssetInfo(cfg.IconBundle, cfg.Icon);
            }
            return new AssetInfo();
        }

        public AssetInfo GetKillIcon(int id)
        {
            var cfg = GetConfigById(id);
            if (cfg != null)
            {
                return new AssetInfo(cfg.IconBundle, cfg.KillIcon);
            }
            return new AssetInfo();   
        }

        
    }
}
