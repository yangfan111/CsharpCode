using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Room;
using Core.Utils;
using Google.Protobuf.Collections;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Server
{
    public partial class ServerRoom
    {
        private IPlayerTokenGenerator _tokenGenerator;

        public PlayerInfo PlayerJoin(long hallRoomId, object roomPlayerInfo, out int errorCode)
        {
            _logger.InfoFormat("Player Join Game ...");

            var roomPlayer = roomPlayerInfo as RoomPlayer;
            PlayerInfo playerInfo = null;

            errorCode = 0;
            if (_hallRoom == null || _hallRoom.HallRoomId != hallRoomId)
            {
                errorCode = (int) ErrorCode.JoinRoom_HallRoom_NotFound;
            }else if (!_hallRoom.CanJoin())
            {
                errorCode = (int) ErrorCode.JoinRoom_ServerRoom_Full;
            }else if (_hallRoom.HasPlayer(roomPlayer.Id))
            {
                errorCode = (int) ErrorCode.JoinRoom_RepeatJoin;
            }
            else
            {
                playerInfo = AddPlayerToRoom(roomPlayer);
            }

            bool success = playerInfo != null;
            _logger.InfoFormat("Join  Success {6} Player({0}), Id:{1}, Name:{2}, ModelId:{3}, TeamId:{4}, Token:{5}", 0,
                roomPlayer.Id, roomPlayer.Name, roomPlayer.RoleModelId, roomPlayer.TeamId, success ?  playerInfo.Token : null, success);

            return playerInfo;
        }

        private PlayerInfo AddPlayerToRoom(RoomPlayer roomPlayer)
        {
            string token = _tokenGenerator.GenerateToken(roomPlayer.Id);
            int[] avatarIds = RepeatedField2IntArray(roomPlayer.AvatarIds);
            int[] weaponAvatarIds = RepeatedField2IntArray(roomPlayer.WeaponAvatarIds);
            int num = _hallRoom.MaxNum(roomPlayer.TeamId);

            PlayerInfo playerInfo = new PlayerInfo(token, RoomId, roomPlayer.Id, roomPlayer.Name, roomPlayer.RoleModelId, roomPlayer.TeamId, num, roomPlayer.Level, roomPlayer.BackId, roomPlayer.TitleId, roomPlayer.BadgeId, avatarIds, weaponAvatarIds, false);
            playerInfo.RankScore = roomPlayer.RankScore;
            playerInfo.IsKing = roomPlayer.IsKing;
            playerInfo.CreateTime = DateTime.Now.Ticks / 10000;
            playerInfo.GameStartTime = (int) DateTime.Now.Ticks / 10000;
            playerInfo.IsLogin = false;
            playerInfo.Camp = roomPlayer.Camp;
            if (playerInfo.Camp > 0 && playerInfo.TeamId == 0)
            {
                playerInfo.TeamId = playerInfo.Camp;
            }
            foreach (var bag in roomPlayer.WeaponBags)
            {
                playerInfo.WeaponBags[bag.BagIndex] = new Core.Room.PlayerWeaponBagData
                {
                    BagIndex = bag.BagIndex,
                    weaponList = new List<Core.Room.PlayerWeaponData>(bag.WeaponList.Count),
                };
                foreach (var weapon in bag.WeaponList)
                {
                    var weaponData = new Core.Room.PlayerWeaponData
                    {
                        Index = weapon.Index,
                        WeaponTplId = weapon.WeaponTplId,
                        WeaponAvatarTplId = weapon.WeaponAvatarTplId,
                    };
                    foreach (var part in weapon.WeaponPartTplId)
                    {
                        var type = SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(part);
                        switch (type)
                        {
                            case XmlConfig.EWeaponPartType.LowerRail:
                                weaponData.LowerRail = part;
                                break;
                            case XmlConfig.EWeaponPartType.Magazine:
                                weaponData.Magazine = part;
                                break;
                            case XmlConfig.EWeaponPartType.Muzzle:
                                weaponData.Muzzle = part;
                                break;
                            case XmlConfig.EWeaponPartType.SideRail:
                                break;
                            case XmlConfig.EWeaponPartType.Stock:
                                weaponData.Stock = part;
                                break;
                            case XmlConfig.EWeaponPartType.UpperRail:
                                weaponData.UpperRail = part;
                                break;
                        }
                    }
                    playerInfo.WeaponBags[bag.BagIndex].weaponList.Add(weaponData);
                }
            }

            _hallRoom.AddPlayer(playerInfo);
            return playerInfo;
        }

        private int[] RepeatedField2IntArray(RepeatedField<int> fields)
        {
            int[] arr = new int[fields.Count];
            for (int a = 0; a < arr.Length; a++)
            {
                arr[a] = fields[a];
            }
            return arr;
        }

    }
}
