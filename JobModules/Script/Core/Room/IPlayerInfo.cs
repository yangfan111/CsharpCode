using Core.Statistics;
using Entitas;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Room
{
    public interface ICreatePlayerInfo
    {
        string Token { get; set; }
        int EntityId { get; set; }
        long PlayerId { get; set; }
        string PlayerName { get; set; }
        int RoleModelId { get; set; }
        long TeamId { get; set; }
        int Num { get; set; }
        int Level { get; set; }
        int BackId { get; set; }
        int TitleId { get; set; }
        int BadgeId { get; set; }
        List<int> AvatarIds { get; set; }
        List<int> WeaponAvatarIds { get; set; }
        int Camp { get; set; }
        List<int> SprayLacquers { get; set; }
        PlayerWeaponBagData[] WeaponBags { get; set; }
        Vector3 InitPosition { get; set; }
        CampInfo CampInfo { get; set; }
    }
    public interface IPlayerInfo : ICreatePlayerInfo
    {
       
        IRoomId RoomId { get; set; }  
        bool IsRobot { get; set; }
        int RankScore { get; set; }
        int Rank { get; set; }
        bool IsKing { get; set; }
        StatisticsData StatisticsData { get; set; }

        long CreateTime { get; set; }
        int GameStartTime { get; set; }
        bool IsLogin { get; set; }
        Entity PlayerEntity { get; set; }
    }
}