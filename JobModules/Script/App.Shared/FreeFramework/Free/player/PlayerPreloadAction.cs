using App.Shared.Components;
using App.Shared.FreeFramework.framework.unit;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Configuration;
using Core.Utils;
using System.Collections.Generic;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.FreeFramework.Free.player
{
    public class PlayerPreloadAction : AbstractPlayerAction
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerPreloadAction));

        public override void DoAction(IEventArgs args)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            _logger.InfoFormat("preloading ---- mode id: {0}", args.GameContext.session.commonSession.RoomInfo.ModeId);
            if (GameRules.IsChicken(args.GameContext.session.commonSession.RoomInfo.ModeId))
            {
                var weaponResourceConfigManager = SingletonManager.Get<WeaponResourceConfigManager>();
                var weaponAvatarConfigManager = SingletonManager.Get<WeaponAvatarConfigManager>();
                var weaponPartSurvivalConfigManager = SingletonManager.Get<WeaponPartSurvivalConfigManager>();
                var weaponPartsConfigManager = SingletonManager.Get<WeaponPartsConfigManager>();
                var vehicleAssetConfigManager = SingletonManager.Get<VehicleAssetConfigManager>();
                var roleAvatarConfigManager = SingletonManager.Get<RoleAvatarConfigManager>();
                var gameItemConfigManager = SingletonManager.Get<GameItemConfigManager>();

                foreach (DropPool drop in DropPoolConfig.current.Items)
                {
                    string assetInfo = "";
                    switch ((ECategory) int.Parse(drop.ItemType))
                    {
                        case ECategory.Weapon:
                            var weaponAvatarId = weaponResourceConfigManager.GetConfigById(int.Parse(drop.ItemId)).AvatorId;
                            var weaponAvatar = weaponAvatarConfigManager.GetConfigById(weaponAvatarId);
                            if (weaponAvatar != null && !StringUtil.IsNullOrEmpty(weaponAvatar.ModelBundle))
                            {
                                assetInfo = weaponAvatar.ModelBundle + "/" + weaponAvatar.ResP3;
                            }
                            break;
                        case ECategory.WeaponPart:
                            var partId = weaponPartSurvivalConfigManager.GetDefaultPartBySetId(int.Parse(drop.ItemId));
                            var part = weaponPartsConfigManager.GetConfigById(partId);
                            if (part != null && !StringUtil.IsNullOrEmpty(part.Bundle))
                            {
                                assetInfo = part.Bundle + "/" + part.Res;
                            }
                            break;
                        case ECategory.Vehicle:
                            var vehicle = vehicleAssetConfigManager.GetConfigItem(int.Parse(drop.ItemId));
                            if (vehicle != null && !StringUtil.IsNullOrEmpty(vehicle.BundleName))
                            {
                                assetInfo = vehicle.BundleName + "/" + vehicle.AssetName;
                            }
                            break;
                        case ECategory.Avatar:
                            var avatar = roleAvatarConfigManager.GetConfigById(int.Parse(drop.ItemId));
                            if (avatar != null && !StringUtil.IsNullOrEmpty(avatar.Bundle))
                            {
                                assetInfo = avatar.Bundle + "/" + avatar.FPrefab;
                            }
                            break;
                        case ECategory.GameItem:
                            var gameItem = gameItemConfigManager.GetConfigById(int.Parse(drop.ItemId));
                            if (gameItem != null && !StringUtil.IsNullOrEmpty(gameItem.Bundle))
                            {
                                assetInfo = gameItem.Bundle + "/" + gameItem.Prefab;
                            }
                            break;
                        default:
                            continue;
                    }

                    if (!StringUtil.IsNullOrEmpty(assetInfo))
                    {
                        if (!dict.ContainsKey(assetInfo))
                        {
                            if (int.Parse(drop.ItemType) == (int) ECategory.Vehicle)
                            {
                                dict.Add(assetInfo, 2);
                            }
                            else
                            {
                                dict.Add(assetInfo, 10);
                            }
                        }
                    }
                }

                var roleConfigManager = SingletonManager.Get<RoleConfigManager>();
                var avatarAssetConfigManager = SingletonManager.Get<AvatarAssetConfigManager>();
                RoomPlayer[] roomPlayers = (RoomPlayer[]) ((ObjectUnit) args.GetUnit("roomInfo")).GetObject;
                if (roomPlayers.Length != 0)
                {
                    foreach (RoomPlayer player in roomPlayers)
                    {
                        var model = roleConfigManager.GetRoleItemById(player.RoleModelId);
                        if (model != null && !StringUtil.IsNullOrEmpty(model.ThirdModelAssetBundle) && !StringUtil.IsNullOrEmpty(model.FirstModelAssetBundle))
                        {
                            string modelP3 = model.ThirdModelAssetBundle + "/" + model.ThirdModelAssetName;
                            string modelP1 = model.FirstModelAssetBundle + "/" + model.FirstModelAssetName;
                            if (dict.ContainsKey(modelP3))
                                dict[modelP3] = dict[modelP3] + 1;
                            else dict.Add(modelP3, 1);
                            if (!dict.ContainsKey(modelP1)) dict.Add(modelP1, 1);

                            foreach (int avatarId in player.AvatarIds)
                            {
                                var resId = roleAvatarConfigManager.GetResId(avatarId, (Sex) model.Sex);
                                var avatar = avatarAssetConfigManager.GetAvatarAssetItemById(resId);
                                if (avatar != null && !StringUtil.IsNullOrEmpty(avatar.BundleName))
                                {
                                    string asset = avatar.BundleName + "/" + avatar.AssetName;
                                    if (dict.ContainsKey(asset))
                                        dict[asset] = dict[asset] + 1;
                                    else dict.Add(asset, 1);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                RoomPlayer[] roomPlayers = (RoomPlayer[]) ((ObjectUnit) args.GetUnit("roomInfo")).GetObject;
                if (roomPlayers.Length != 0)
                {
                    var roleConfigManager = SingletonManager.Get<RoleConfigManager>();
                    var roleAvatarConfigManager = SingletonManager.Get<RoleAvatarConfigManager>();
                    var avatarAssetConfigManager = SingletonManager.Get<AvatarAssetConfigManager>();
                    var weaponResourceConfigManager = SingletonManager.Get<WeaponResourceConfigManager>();
                    var weaponAvatarConfigManager = SingletonManager.Get<WeaponAvatarConfigManager>();
                    var weaponPartsConfigManager = SingletonManager.Get<WeaponPartsConfigManager>();

                    foreach (RoomPlayer player in roomPlayers)
                    {
                        if (GameRules.ClothesType(args.GameContext.session.commonSession.RoomInfo.ModeId) != (int) EGameModeClothes.Default && player.CampInfo != null && player.CampInfo.Preset.Count > 0)
                        {
                            foreach (PresetInfo p in player.CampInfo.Preset)
                            {
                                var model = roleConfigManager.GetRoleItemById(p.RoleModelId);
                                if (model != null && !StringUtil.IsNullOrEmpty(model.ThirdModelAssetBundle) && !StringUtil.IsNullOrEmpty(model.FirstModelAssetBundle))
                                {
                                    string modelP3 = model.ThirdModelAssetBundle + "/" + model.ThirdModelAssetName;
                                    string modelP1 = model.FirstModelAssetBundle + "/" + model.FirstModelAssetName;
                                    if (dict.ContainsKey(modelP3))
                                        dict[modelP3] = dict[modelP3] + 1;
                                    else dict.Add(modelP3, 1);
                                    if (!dict.ContainsKey(modelP1)) dict.Add(modelP1, 1);

                                    foreach (int avatarId in p.AvatarIds)
                                    {
                                        var resId = roleAvatarConfigManager.GetResId(avatarId, (Sex) model.Sex);
                                        var avatar = avatarAssetConfigManager.GetAvatarAssetItemById(resId);
                                        if (avatar != null && !StringUtil.IsNullOrEmpty(avatar.BundleName))
                                        {
                                            string asset = avatar.BundleName + "/" + avatar.AssetName;
                                            if (dict.ContainsKey(asset))
                                                dict[asset] = dict[asset] + 1;
                                            else dict.Add(asset, 1);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var model = roleConfigManager.GetRoleItemById(player.RoleModelId);
                            if (model != null && !StringUtil.IsNullOrEmpty(model.ThirdModelAssetBundle) && !StringUtil.IsNullOrEmpty(model.FirstModelAssetBundle))
                            {
                                string modelP3 = model.ThirdModelAssetBundle + "/" + model.ThirdModelAssetName;
                                string modelP1 = model.FirstModelAssetBundle + "/" + model.FirstModelAssetName;
                                if (dict.ContainsKey(modelP3))
                                    dict[modelP3] = dict[modelP3] + 1;
                                else dict.Add(modelP3, 1);
                                if (!dict.ContainsKey(modelP1)) dict.Add(modelP1, 1);

                                foreach (int avatarId in player.AvatarIds)
                                {
                                    var resId = roleAvatarConfigManager.GetResId(avatarId, (Sex) model.Sex);
                                    var avatar = avatarAssetConfigManager.GetAvatarAssetItemById(resId);
                                    if (avatar != null && !StringUtil.IsNullOrEmpty(avatar.BundleName))
                                    {
                                        string asset = avatar.BundleName + "/" + avatar.AssetName;
                                        if (dict.ContainsKey(asset))
                                            dict[asset] = dict[asset] + 1;
                                        else dict.Add(asset, 1);
                                    }
                                }
                            }
                        }


                        foreach (PlayerWeaponBagData weaponBag in player.WeaponBags)
                        {
                            foreach (PlayerWeaponData weaponData in weaponBag.WeaponList)
                            {
                                var weaponAvatarId = weaponResourceConfigManager.GetConfigById(weaponData.WeaponTplId)
                                    .AvatorId;
                                var weaponAvatar = weaponAvatarConfigManager.GetConfigById(weaponAvatarId);
                                if (weaponAvatar != null && !StringUtil.IsNullOrEmpty(weaponAvatar.ModelBundle))
                                {
                                    string assetP3 = weaponAvatar.ModelBundle + "/" + weaponAvatar.ResP3;
                                    string assetP1 = weaponAvatar.ModelBundle + "/" + weaponAvatar.ResP1;
                                    if (dict.ContainsKey(assetP3))
                                        dict[assetP3] = dict[assetP3] + 1;
                                    else dict.Add(assetP3, 1);
                                    if (!dict.ContainsKey(assetP1))
                                        dict.Add(assetP1, 1);

                                    foreach (int partId in weaponData.WeaponPartTplId)
                                    {
                                        var part = weaponPartsConfigManager.GetConfigById(partId);
                                        if (part != null && !StringUtil.IsNullOrEmpty(part.Bundle))
                                        {
                                            string asset = part.Bundle + "/" + part.Res;
                                            if (dict.ContainsKey(asset))
                                                dict[asset] = dict[asset] + 1;
                                            else dict.Add(asset, 1);
                                        }
                                    }
                                }
                            }
                        }

                        
                    }
                }
            }

            int count = 0;
            foreach (string assetInfo in dict.Keys)
            {
                for (int i = 0; i < dict[assetInfo]; i++)
                {
                    args.GameContext.session.commonSession.RoomInfo.PreLoadAssetInfo.Add(assetInfo);
                }
                count += dict[assetInfo];
            }
            _logger.InfoFormat("preloading --- asset count: {0}", count);
        }
    }
}
