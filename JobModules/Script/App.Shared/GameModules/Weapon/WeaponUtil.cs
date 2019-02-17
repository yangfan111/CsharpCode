using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using System.Collections.Generic;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponLogicUtil" />
    /// </summary>
    public static class WeaponLogicUtil
    {
    }

    /// <summary>
    /// Defines the <see cref="WeaponUtil" />
    /// </summary>
    public static class WeaponUtil
    {
        public static int EmptyHandId
        {
            get { return SingletonManager.Get<WeaponConfigManager>().EmptyHandId; }
        }

        /// <summary>
        /// 验证WeaponId合法
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns></returns>
        public static bool VertifyWeaponConfigId(int weaponId)
        {
            NewWeaponConfigItem config;
            return VertifyWeaponConfigId(weaponId, out config);
        }

        public static bool VertifyWeaponConfigId(int weaponId, out NewWeaponConfigItem config)
        {
            config = null;
            if (weaponId == 0)
                return false;
            config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            return config != null;
        }
        public static bool IsWeaponKeyVaild (EntityKey key)
        {
            return key != EntityKey.Default || key != EntityKey.EmptyWeapon;
        }
        public static WeaponBagContainer[] CreateEmptyBagContainers()
        {
            var containerSet = new WeaponBagContainer[GameGlobalConst.WeaponBagMaxCount];
            for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
            {
                containerSet[i] = new WeaponBagContainer();
            }
            return containerSet;
        }
        public static WeaponScanStruct CreateScan(int configId, System.Action<WeaponScanStruct> initFunc)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.Assign(configId);
            initFunc(val);
            return val;
        }
        public static WeaponScanStruct CreateScan(Components.SceneObject.WeaponObjectComponent weaponObject)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.Assign(weaponObject.WeaponKey, weaponObject.ConfigId);
            return val;
        }
        public static WeaponScanStruct CreateScan(int configId)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.Assign(configId);
            return val;
        }
        public static WeaponScanStruct CreateScan(WeaponEntity entity)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.Assign(entity.entityKey.Value, entity.weaponBasicData.ConfigId);
            return val;
        }
        public static bool VertifyEweaponSlotIndex(int index, bool ignoreNone = false)
        {
            return (index > 0 || ignoreNone) && index < (int)EWeaponSlotType.Length;
        }

        public static float GetWeaponDefaultSpeed()
        {
            var config = SingletonManager.Get<WeaponDataConfigManager>().GetConfigById(EmptyHandId);
            return config.WeaponLogic.MaxSpeed;
        }

        public static GrenadeCacheData[] CreateEmptyGrenadeCacheArrs(List<int> ids)
        {
            var containerSet = new GrenadeCacheData[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                containerSet[i] = new GrenadeCacheData();
                containerSet[i].grenadeId = ids[i];
            }
            return containerSet;
        }

        public static List<int> ForeachFilterGreandeIds()
        {
            var configs = SingletonManager.Get<WeaponConfigManager>().GetConfigs();
            var grenadeConfigIds = new List<int>();
            foreach (var config in configs)
            {
                switch ((EWeaponType)config.Value.Type)
                {
                    case EWeaponType.ThrowWeapon:
                        var subType = (EWeaponSubType)config.Value.SubType;
                        switch (subType)
                        {
                            case EWeaponSubType.BurnBomb:
                            case EWeaponSubType.FlashBomb:
                            case EWeaponSubType.FogBomb:
                            case EWeaponSubType.Grenade:
                                break;
                            default:
                                break;
                        }
                        grenadeConfigIds.Add(config.Value.Id);
                        break;
                }
            }
            return grenadeConfigIds;
        }
    }
}
