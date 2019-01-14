using System;
using Assets.XmlConfig;
using Utils.AssetManager;
using Core.Enums;

namespace App.Shared
{
    public class AssetConfig
    {
        public static AssetInfo GetCharacterModelAssetInfo(string modelName)
        {
            return new AssetInfo("character/" + modelName, "model");
        }

        public static AssetInfo GetCharacterHandAssetInfo(string modelName)
        {
            return new AssetInfo("character/" + modelName, "Hand");
        }

        public static AssetInfo GetCharacterHitboxAssetInfo(string modelName)
        {
            return new AssetInfo("character/" + modelName, "hitbox");
        }


        public static AssetInfo GetMapAssetInfo()
        {
            return new AssetInfo("map/test", "map");
        }

        public static AssetInfo GetWeaponConfigAssetInfo()
        {
            return new AssetInfo("tables", "weapon");
        }

        public static AssetInfo GetBulletAssetInfo(EBulletType type)
        {
            switch (type)
            {
                case EBulletType.Light:
                    return new AssetInfo("common/bullet", "dandao01");
                case EBulletType.Normal:
                default:
                    return new AssetInfo("common/bullet", "bolt");
            }
        }

        public static AssetInfo GetHitWallAssetInfo()
        {
            return new AssetInfo("effect/common", "explosion_asteroid");
        }
		
		public static AssetInfo GetVehicleAssetInfo(string assetBundleName, string modelName)
        {
            return new AssetInfo(assetBundleName, modelName);
        }

        public static AssetInfo GetVehicleHitboxAssetInfo(string assetBundleName)
        {
            return new AssetInfo(assetBundleName, "hitbox");
        }
        

        public static AssetInfo GetDamageHintAssetInfo()
        {
            return new AssetInfo("effect/common", "DamageHint");
        }

        public static AssetInfo GetHumanHitAssetInfo()
        {
            return new AssetInfo("effect/common", "explosion_player");
        }

        public static AssetInfo GetEquipmentBasisAssetInfo()
        {
            return new AssetInfo("configuration/equipment", "basis");
        }

        public static AssetInfo GetEquipmentLocationAssetInfo()
        {
            return new AssetInfo("configuration/equipment", "location");
        }

        public static AssetInfo GetSceneConfigAssetInfo()
        {
            return new AssetInfo("configuration/scene", "scene");
        }

        public static AssetInfo GetMapConfigInfo()
        {
            return new AssetInfo("configuration/scene", "mapConfig");
        }

        public static AssetInfo GetAnimationConfigAssetInfo()
        {
            return new AssetInfo("configuration/animation", "animation");
        }

        public static AssetInfo GetFirstPersonFemaleEmptyHandedAnimation()
        {
            return new AssetInfo("configuration/animation", "HandNwOC");
        }

        public static AssetInfo GetThirdPersonFemaleEmptyHandedAnimation()
        {
            return new AssetInfo("configuration/animation", "FemaleNwOC");
        }

        public static AssetInfo GetFirstPersonMaleEmptyHandedAnimation()
        {
            return new AssetInfo();
        }

        public static AssetInfo GetThirdPersonMaleEmptyHandedAnimation()
        {
            return new AssetInfo();
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

