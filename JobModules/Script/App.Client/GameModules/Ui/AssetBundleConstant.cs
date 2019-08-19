using System;
using Core.Enums;
using Utils.AssetManager;

namespace Assets.App.Client.GameModules.Ui
{
    public class AssetBundleConstant
    {
        public const string UiConfigBundleName = "uiconfig";
        public const string UiCommon = "ui/client/common";
        public const string UiPrefab_Carry = "ui/client/prefab/carry";

        public const string Icon_Map = "ui/icon/map";
        public const string Icon_Carry = "ui/icon/carry";
        public const string Icon_Prop = "ui/icon/prop";
        public const string Icon_MapSnap = "ui/icon/mapsnap";
        public const string Icon_Weapon = "ui/icon/weapon";
        public const string Icon_Killinfo = "ui/icon/killinfo";
        public const string Icon_Reticle = "ui/icon/reticle";
        public const string Icon_Spray = "ui/icon/spray"; /*喷漆*/
        public const string Icon_Common = "ui/icon/common"; 

        public const string Icon_UiIcons = "ui/icons";//暂时无法分类，需要重新整理

        public const string Effect = "effect/client";
        public const string Effect_Common = "effect/common";

        public const string Prefab_Spray = "decalicious/decal";
        public const string Prefab_Common = "ui/client/prefab/common";

        public static AssetInfo GetPartsQualityAssetInfo(int quality)
        {
            return GetCommonAssetInfo("PartsQuality" + quality);
        }

        public static AssetInfo GetTipQualityAssetInfo(int quality)
        {
            if (quality < 1) quality = 1;
            return GetCommonAssetInfo("TipQuality" + quality);
        }

        public static AssetInfo GetCommonAssetInfo(string name)
        {
            return new AssetInfo(Icon_Common, name);
        }

        public static AssetInfo GetCampSmallIcon(EUICampType camp)
        {
            if (camp != EUICampType.None)
            {
                return GetCommonAssetInfo("CampSmall" + (int)camp);
            }
            return new AssetInfo();
        }

        public static AssetInfo GetSmallItemQualityLayer(int xlv)
        {
            return GetCommonAssetInfo("SmallItemQuality" + xlv);
        }
    }
}