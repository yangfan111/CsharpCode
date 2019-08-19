using System.Collections.Generic;
using App.Protobuf;
using Core.Room;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;
using PlayerWeaponBagData = App.Protobuf.PlayerWeaponBagData;
using PlayerWeaponData = App.Protobuf.PlayerWeaponData;

namespace App.Shared.Player
{
    public static class PlayerInfoExtensions
    {
        public static void ConvertFrom(this PlayerInfoMessage message, ICreatePlayerInfo info)
        {
            message.Camp        = info.Camp;
            message.Level       = info.Level;
            message.Num         = info.Num;
            message.BackId      = info.BackId;
            message.BadgeId     = info.BadgeId;
            message.EntityId    = info.EntityId;
            message.PlayerId    = info.PlayerId;
            message.PlayerName  = info.PlayerName;
            message.TeamId      = info.TeamId;
            message.TitleId     = info.TitleId;
            message.RoleModelId = info.RoleModelId;
            foreach (var id in info.AvatarIds)
            {
                message.AvatarIds.Add(id);
            }

            foreach (var id in info.WeaponAvatarIds)
            {
                message.WeaponAvatarIds.Add(id);
            }

            foreach (var id in info.SprayLacquers)
            {
                message.SprayLacquers.Add(id);
            }

            foreach (var bag in info.WeaponBags)
            {
                if (null == bag)
                {
                    continue;
                }

                PlayerWeaponBagData bagData = PlayerWeaponBagData.Allocate();
                bagData.BagIndex = bag.BagIndex;
                foreach (var weapon in bag.weaponList)
                {
                    var weaponData = PlayerWeaponData.Allocate();
                    weaponData.Index             = weapon.Index;
                    weaponData.WeaponTplId       = weapon.WeaponTplId;
                    weaponData.WeaponAvatarTplId = weapon.WeaponAvatarTplId;
                    if (weapon.LowerRail > 0)
                        weaponData.WeaponPartTplId.Add(weapon.LowerRail);
                    if (weapon.Magazine > 0)
                        weaponData.WeaponPartTplId.Add(weapon.Magazine);
                    if (weapon.UpperRail > 0)
                        weaponData.WeaponPartTplId.Add(weapon.UpperRail);
                    if (weapon.Muzzle > 0)
                        weaponData.WeaponPartTplId.Add(weapon.Muzzle);
                    if (weapon.Stock > 0)
                        weaponData.WeaponPartTplId.Add(weapon.Stock);

                    bagData.WeaponList.Add(weaponData);
                }

                message.WeaponBags.Add(bagData);
            }

            message.InitPosition   = Vector3.Allocate();
            message.InitPosition.X = info.InitPosition.x;
            message.InitPosition.Y = info.InitPosition.y;
            message.InitPosition.Z = info.InitPosition.z;
        }

        public static void ConvertFrom(this ICreatePlayerInfo info, PlayerInfoMessage message)
        {
            info.Token       = "local";
            info.Camp        = message.Camp;
            info.Level       = message.Level;
            info.Num         = message.Num;
            info.BackId      = message.BackId;
            info.BadgeId     = message.BadgeId;
            info.EntityId    = message.EntityId;
            info.PlayerId    = message.PlayerId;
            info.PlayerName  = message.PlayerName;
            info.TeamId      = message.TeamId;
            info.TitleId     = message.TitleId;
            info.RoleModelId = message.RoleModelId;
            info.AvatarIds   = new List<int>();
            info.AvatarIds.AddRange(message.AvatarIds);
            info.WeaponAvatarIds = new List<int>();
            info.WeaponAvatarIds.AddRange(message.WeaponAvatarIds);
            info.SprayLacquers = new List<int>();
            info.SprayLacquers.AddRange(message.SprayLacquers);
            info.WeaponBags = new Core.Room.PlayerWeaponBagData[message.WeaponBags.Count];
            for (var i = message.WeaponBags.Count - 1; i >= 0; i--)
            {
                info.WeaponBags[i]            = new Core.Room.PlayerWeaponBagData();
                info.WeaponBags[i].BagIndex   = message.WeaponBags[i].BagIndex;
                info.WeaponBags[i].weaponList = new List<Core.Room.PlayerWeaponData>();
                Core.Room.PlayerWeaponData weaponData;
                foreach (var playerWeaponData in message.WeaponBags[i].WeaponList)
                {
                    //info.WeaponBags[i].weaponList.Add(
                    weaponData = new Core.Room.PlayerWeaponData
                    {
                                    Index             = playerWeaponData.Index,
                                    WeaponTplId       = playerWeaponData.WeaponTplId,
                                    WeaponAvatarTplId = playerWeaponData.WeaponAvatarTplId
                    };
                    foreach (var part in playerWeaponData.WeaponPartTplId)
                    {
                        var type = SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(part);
                        switch (type)
                        {
                            case EWeaponPartType.LowerRail:
                                weaponData.LowerRail = part;
                                break;
                            case EWeaponPartType.Magazine:
                                weaponData.Magazine = part;
                                break;
                            case EWeaponPartType.Muzzle:
                                weaponData.Muzzle = part;
                                break;
                            case EWeaponPartType.SideRail:
                                break;
                            case EWeaponPartType.Stock:
                                weaponData.Stock = part;
                                break;
                            case EWeaponPartType.UpperRail:
                                weaponData.UpperRail = part;
                                break;
                        }
                    }

                    info.WeaponBags[i].weaponList.Add(weaponData);
                }
            }

            info.InitPosition =
                            new UnityEngine.Vector3(message.InitPosition.X, message.InitPosition.Y,
                                message.InitPosition.Z);
        }
    }
}