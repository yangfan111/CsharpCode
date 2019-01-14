



using System;
using System.Collections.Generic;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Room;
using Sharpen;
using Utils.Configuration;

namespace App.Shared.Player
{
    public static class PlayerInfoExtensions
    {
        public static void ConvertFrom(this App.Protobuf.PlayerInfoMessage message,  Core.Room.ICreatePlayerInfo info )
        {
            message.Camp = info.Camp;
            message.Level = info.Level;
            message.Num = info.Num;
            message.BackId = info.BackId;
            message.BadgeId = info.BadgeId;
            message.EntityId = info.EntityId;
            message.PlayerId = info.PlayerId;
            message.PlayerName = info.PlayerName;
            message.TeamId = info.TeamId;
            message.TitleId = info.TitleId;
            message.RoleModelId = info.RoleModelId;
            foreach (var id in info.AvatarIds)
            {
                message.AvatarIds.Add(id);
            }
            foreach (var id in info.WeaponAvatarIds)
            {
                message.WeaponAvatarIds.Add(id);
            }
          
            foreach (var bag in info.WeaponBags)
            {
                if (null == bag)
                {
                    continue;
                }
                var bagData = Protobuf.PlayerWeaponBagData.Allocate();
                bagData.BagIndex = bag.BagIndex;
                foreach (var weapon in bag.weaponList)
                {
                    var weaponData = Protobuf.PlayerWeaponData.Allocate();
                    weaponData.Index = weapon.Index;
                    weaponData.WeaponTplId = weapon.WeaponTplId;
                    weaponData.WeaponAvatarTplId = weapon.WeaponAvatarTplId;
                    bagData.WeaponList.Add(weaponData);
                }
                message.WeaponBags.Add(bagData);
            }
           
        }

        public static void ConvertFrom(this Core.Room.ICreatePlayerInfo info , App.Protobuf.PlayerInfoMessage message )
        {
            info.Camp = message.Camp;
            info.Level = message.Level;
            info.Num = message.Num;
            info.BackId = message.BackId;
            info.BadgeId = message.BadgeId;
            info.EntityId = message.EntityId;
            info.PlayerId = message.PlayerId;
            info.PlayerName = message.PlayerName;
            info.TeamId = message.TeamId;
            info.TitleId = message.TitleId;
            info.RoleModelId = message.RoleModelId;
            info.AvatarIds=new List<int>();
            info.AvatarIds.AddRange(message.AvatarIds);
            info.WeaponAvatarIds =new List<int>();
            info.WeaponAvatarIds.AddRange(message.WeaponAvatarIds);
            info.WeaponBags = new Core.Room.PlayerWeaponBagData[message.WeaponBags.Count];
            for (var i = message.WeaponBags.Count - 1; i >= 0; i--)
            {
                info.WeaponBags[i] = new  Core.Room.PlayerWeaponBagData();
                info.WeaponBags[i].BagIndex = message.WeaponBags[i].BagIndex;
                info.WeaponBags[i].weaponList = new List<Core.Room.PlayerWeaponData>();
                foreach (var playerWeaponData in message.WeaponBags[i].WeaponList)
                {
                    info.WeaponBags[i].weaponList.Add(new Core.Room.PlayerWeaponData
                    {
                        Index = playerWeaponData.Index,
                        WeaponTplId = playerWeaponData.WeaponTplId,
                        WeaponAvatarTplId = playerWeaponData.WeaponAvatarTplId,
                    });
                }
            }
        }
    }
}