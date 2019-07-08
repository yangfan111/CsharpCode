using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
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
        
        private const string DualWeaponSuffix = "D(Clone)";
        private const string SubRightWeaponSuffix = "Right";
        private const string SubLeftWeaponSuffix = "Left";
        private const string OverrideControllerTransitionSuffix = "_Transition";

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
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimFemaleP3 + OverrideControllerTransitionSuffix));
                    }
                    if (!string.IsNullOrEmpty(item.AnimMaleP1))
                    {
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP1));
                    }
                    if (!string.IsNullOrEmpty(item.AnimMaleP3))
                    {
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP3));
                        AnimSubResourceAssetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP3 + OverrideControllerTransitionSuffix));
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
            WeaponAvatarConfigItem configItem;
            _configItems.TryGetValue(id, out configItem);
            return configItem;
        }

        public bool HaveLeftWeapon(int id)
        {
            var cfg = GetConfigById(id);
            if (null == cfg) return false;
            
            var suffixName = Match(cfg.ResP3);
            return suffixName.Equals(DualWeaponSuffix);
        }
        
        public bool HaveLeftWeapon(string name)
        {
            var suffixName = Match(name);
            return suffixName.Equals(DualWeaponSuffix);
        }
        
        public GameObject GetRightWeaponGameObject(UnityObject weapon)
        {
            if (null == weapon || null == weapon.AsGameObject) return null;
            var weaponObj = weapon.AsGameObject;

            for (var i = 0; i < weaponObj.transform.childCount; ++i)
            {
                var child = weaponObj.transform.GetChild(i);
                if (child.gameObject.name.Contains(SubRightWeaponSuffix))
                    return child.gameObject;
            }

            Logger.ErrorFormat("weapon  {0}  do not have rightWeapon", weapon.AsGameObject.name);
            return null;
        }

        public GameObject GetLeftWeaponGameObject(UnityObject weapon)
        {
            if (null == weapon || null == weapon.AsGameObject) return null;
            var weaponObj = weapon.AsGameObject;

            for (var i = 0; i < weaponObj.transform.childCount; ++i)
            {
                var child = weaponObj.transform.GetChild(i);
                if (child.gameObject.name.Contains(SubLeftWeaponSuffix))
                    return child.gameObject;
            }

            Logger.ErrorFormat("weapon  {0}  do not have leftWeapon", weapon.AsGameObject.name);
            return null;
        }

        public AssetInfo GetThirdPersonWeaponModel(int id)
        {
            var cfg = GetConfigById(id);
            if(null != cfg)
            {
                return new AssetInfo(cfg.ModelBundle, cfg.ResP3);
            }
            return AssetInfo.EmptyInstance;
        }
        public AssetInfo GetTexWeapoonAsset(int id)
        {
            var cfg = GetConfigById(id);
            if(null != cfg)
            {
                return new AssetInfo(cfg.ModelBundle, cfg.Texture);
            }
            return AssetInfo.EmptyInstance;
        }
        
        public AssetInfo GetFirstPersonWeaponModel(int id)
        {
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                return new AssetInfo(cfg.ModelBundle, cfg.ResP1);
            }
            return AssetInfo.EmptyInstance;

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
        
        public AssetInfo GetThirdPersonAnimationTransition(int id, Sex sex)
        {
            var cfg = GetConfigById(id);
            if(null != cfg)
            {
                switch (sex)
                {
                    case Sex.Male:
                        return new AssetInfo(cfg.AnimBundle, cfg.AnimMaleP3 + OverrideControllerTransitionSuffix);
                    case Sex.Female:
                        return new AssetInfo(cfg.AnimBundle, cfg.AnimFemaleP3 + OverrideControllerTransitionSuffix);
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
            return new AssetInfo();
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

        private static readonly Dictionary<string,string> Cache = new Dictionary<string, string>();
        private static string Match(string name)
        {
            if (Cache.ContainsKey(name)) return Cache[name];
            var match = name.Split('_');
            var ret = string.Empty;
            if (match.Length > 0)
            {
                ret = match[match.Length - 1];
            }

            Cache[name] = ret;
            return ret;
        }
    }
}
