using Core.Statistics;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Room
{
    public class PlayerInfo : IPlayerInfo
    {
        /// <summary>
        /// 战斗中的武器背包的总数,由1开始
        /// </summary>
        public static readonly int PlayerWeaponBagCount = 6;

        public PlayerInfo()
        {
            WeaponBags = new PlayerWeaponBagData[PlayerWeaponBagCount];
            AvatarIds = new List<int>();
            WeaponAvatarIds = new List<int>();
            SprayLacquers = new List<int>();
        }

        public PlayerInfo(string token, IRoomId roomId, long playerId, string playerName, int roleModelId, long teamId,
            int num, int level, int backId, int titleId, int badgeId, int[] avatarIds, int[] weaponAvatarIds,
            int[] sprayLacquers, bool hasTestWeapon, int jobAttrbute, CampInfo campInfo) : this()
        {
            Token = token;
            RoomId = roomId;
            PlayerId = playerId;
            PlayerName = playerName;
            RoleModelId = roleModelId;
            TeamId = teamId;
            Num = num;
            Level = level;
            BackId = backId;
            TitleId = titleId;
            BadgeId = badgeId;

            if (avatarIds != null)
            {
                AvatarIds.AddRange(avatarIds);
            }


            if (weaponAvatarIds != null)
            {
                WeaponAvatarIds.AddRange(weaponAvatarIds);
            }

            if (sprayLacquers != null)
            {
                SprayLacquers.AddRange(sprayLacquers);
            }

            if (hasTestWeapon)
            {
                WeaponBags[1] = new PlayerWeaponBagData
                {
                    BagIndex = 1,
                    weaponList = new List<PlayerWeaponData>
                    {
                        new PlayerWeaponData
                        {
                            Index = 1,
                            WeaponTplId = 1,
                            WeaponAvatarTplId = 1,
                        }
                    }
                };
            }

            JobAttribute = jobAttrbute;
            CampInfo = campInfo;
        }


        public int EntityId { get; set; }
        public string Token { get; set; }
        public IRoomId RoomId { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int RoleModelId { get; set; }
        public bool IsRobot { get; set; }
        public long TeamId { get; set; }
        public int Num { get; set; }
        public int Level { get; set; }
        public int BackId { get; set; }
        public int TitleId { get; set; }
        public int BadgeId { get; set; }
        public List<int> AvatarIds { get; set; }
        public List<int> WeaponAvatarIds { get; set; }
        public long CreateTime { get; set; }
        public int GameStartTime { get; set; }
        public bool IsLogin { get; set; }
        public int RankScore { get; set; }
        public int Rank { get; set; }
        public bool IsKing { get; set; }
        public int Camp { get; set; }
        public PlayerWeaponBagData[] WeaponBags { get; set; }
        public Vector3 InitPosition { get; set; }
        public StatisticsData StatisticsData { get; set; }
        public Entity PlayerEntity { get; set; }
        public int JobAttribute { get; set; }

        public int SpecialFeedbackType { get; set; }
        public List<int> SprayLacquers { get; set; }

        public CampInfo CampInfo { get; set; }
    }

    public class PlayerWeaponData
    {
        public int Index;
        public int WeaponTplId;
        public int WeaponAvatarTplId;
        
        public int LowerRail;
        public int UpperRail;
        public int SideRail;
        public int Stock;
        public int Muzzle;
        public int Magazine;
        
        public int Bore;
        public int Feed;
        public int Trigger;
        public int Interlock;
        public int Brake;

        public override string ToString()
        {
            return string.Format(
                "index {0}, weaponId {1}, avatarId {2}, lowerRail {3}, upRail {4}, stock {5}, muzzle {6} magazine {7}",
                Index, WeaponTplId, WeaponAvatarTplId, LowerRail, UpperRail, Stock, Muzzle, Magazine);
        }

        public static WeaponScanStruct Explicit(PlayerWeaponData weaponData)
        {
            WeaponScanStruct scan = new WeaponScanStruct();
            scan.ConfigId = weaponData.WeaponTplId;

            scan.AvatarId = weaponData.WeaponAvatarTplId;
            scan.Muzzle = weaponData.Muzzle;
            scan.Stock = weaponData.Stock;
            scan.Magazine = weaponData.Magazine;
            scan.UpperRail = weaponData.UpperRail;
            scan.LowerRail = weaponData.LowerRail;
            scan.SideRail = weaponData.SideRail;
            scan.Bore = weaponData.Bore;
            scan.Feed = weaponData.Feed;
            scan.Trigger = weaponData.Trigger;
            scan.Interlock = weaponData.Interlock;
            scan.Brake = weaponData.Brake;
            return scan;
        }
    }

    public class PlayerWeaponBagData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponBagData));
        public int BagIndex;
        public List<PlayerWeaponData> weaponList;

        private static readonly System.Text.StringBuilder OuputStringBuilder = new System.Text.StringBuilder();

        public override string ToString()
        {
            OuputStringBuilder.Length = 0;
            OuputStringBuilder.Append(BagIndex);
            OuputStringBuilder.Append(":\n");
            for (int i = 0; i < weaponList.Count; i++)
            {
                OuputStringBuilder.Append(weaponList[i].Index);
                OuputStringBuilder.Append(":");
                OuputStringBuilder.Append(weaponList[i].WeaponTplId);
                OuputStringBuilder.Append(",");
                OuputStringBuilder.Append(weaponList[i].WeaponAvatarTplId);
                OuputStringBuilder.AppendLine();
            }

            return OuputStringBuilder.ToString();
        }

        public void CopyTo(PlayerWeaponBagData playerWeaponBagData)
        {
            if (null == playerWeaponBagData)
            {
                Logger.Error("Target to copy is null");
                return;
            }

            playerWeaponBagData.BagIndex = BagIndex;
            if (playerWeaponBagData.weaponList == null)
            {
                playerWeaponBagData.weaponList = new List<PlayerWeaponData>();
            }

            playerWeaponBagData.weaponList.Clear();
            foreach (var weapon in weaponList)
            {
                playerWeaponBagData.weaponList.Add(weapon);
            }
        }

        public static int Slot2Index(EWeaponSlotType slotType)
        {
            switch (slotType)
            {
                case EWeaponSlotType.None:
                    return 0;
                case EWeaponSlotType.PrimeWeapon:
                case EWeaponSlotType.SecondaryWeapon:
                    return 1;
                case EWeaponSlotType.PistolWeapon:
                    return 2;
                case EWeaponSlotType.MeleeWeapon:
                    return 3;
                case EWeaponSlotType.ThrowingWeapon:
                    return 4;
                case EWeaponSlotType.TacticWeapon:
                    return 7;
                default:
                    Logger.ErrorFormat("slot {0} is illegal", slotType);
                    return 0;
            }
        }

        public static EWeaponSlotType Index2Slot(int index)
        {
            switch (index)
            {
                case 0:
                    return EWeaponSlotType.None;
                case 1:
                    return EWeaponSlotType.PrimeWeapon;
                case 2:
                    return EWeaponSlotType.PistolWeapon;
                case 3:
                    return EWeaponSlotType.MeleeWeapon;
                case 4:
                case 5:
                case 6:
                    return EWeaponSlotType.ThrowingWeapon;
                case 7:
                    return EWeaponSlotType.TacticWeapon;
                default:
                    Logger.ErrorFormat("index {0} is illegal for slot", index);
                    return EWeaponSlotType.None;
            }
        }
    }

    public class CampInfo
    {
        public int CurrCamp { get; set; }
        public List<Preset> Preset { get; set; }

        public CampInfo(){}

        public CampInfo(int currCamp, List<Preset> preset)
        {
            CurrCamp = currCamp;
            Preset = preset;
        }

        public void Reset()
        {
            CurrCamp = 0;
            Preset.Clear();
        }
    }

    public class Preset
    {
        public int camp;
        public int roleModelId;
        public List<int> avatarIds;
        public int pose;

        public Preset(int camp, int roleModelId, List<int> avatarIds, int pose)
        {
            this.camp = camp;
            this.roleModelId = roleModelId;
            this.avatarIds = avatarIds;
            this.pose = pose;
        }
    }
}
