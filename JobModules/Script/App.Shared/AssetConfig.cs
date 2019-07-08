using System;
using Assets.XmlConfig;
using Utils.AssetManager;
using Core.Enums;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared
{
    public class AssetConfig
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AssetConfig));
        
        public static AssetInfo GetCharacterModelAssetInfo(int roleId)
        {
            var item = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId);
            if (null == item)
            {
                Logger.ErrorFormat("roleId:  {0}  not exit in roleXML", roleId);
                return new AssetInfo();
            }
            return new AssetInfo(item.ThirdModelAssetBundle, item.ThirdModelAssetName);
        }

        public static AssetInfo GetCharacterHandAssetInfo(int roleId)
        {
            var item = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId);
            if (null == item)
            {
                Logger.ErrorFormat("roleId:  {0}  not exit in roleXML", roleId);
                return new AssetInfo();
            }
            return new AssetInfo(item.FirstModelAssetBundle, item.FirstModelAssetName);
        }

        public static AssetInfo GetBulletAssetInfo(bool isAim)
        {
            //if(isAim)
                return  new AssetInfo("common/bullet", "dandao03");
            return new AssetInfo("common/bullet", "bolt");
            //if (isAim)
            //        return new AssetInfo("common/bullet", "dandao03");
            //    return new AssetInfo("common/bullet", "bolt");
        }
		
		public static AssetInfo GetVehicleAssetInfo(string assetBundleName, string modelName)
        {
            return new AssetInfo(assetBundleName, modelName);
        }

        public static AssetInfo GetVehicleHitboxAssetInfo(string assetBundleName)
        {
            return new AssetInfo(assetBundleName, "hitbox");
        }

        public static AssetInfo GetAnimationConfigAssetInfo()
        {
            return new AssetInfo("configuration/animation", "animation");
        }

        public static AssetInfo GetCameraPoisonEffect()
        {
            return new AssetInfo("effect/common", "pingmu_kouxue");
        }

        public static AssetInfo GetThrowingAssetInfo(EWeaponSubType type)
        {
            switch (type)
            {
                case EWeaponSubType.Grenade:
                    return new AssetInfo("weapon/grenade", "WPN_Grenade0000_P1");
                case EWeaponSubType.FlashBomb:
                    return new AssetInfo("weapon/flash", "WPN_Flash0000_P1");
                case EWeaponSubType.FogBomb:
                    return new AssetInfo("weapon/smoke", "WPN_Smoke0000_P1");
                case EWeaponSubType.BurnBomb:
                    return new AssetInfo("weapon/grenade", "G003T_bottle_P1");
                default:
                    return new AssetInfo("weapon/grenade", "WPN_Grenade0000_P1");
            }
        }

        public static AssetInfo GetBehaviorAssetInfo(String name)
        {
            return new AssetInfo("behavior",name);
        }
    }
}

