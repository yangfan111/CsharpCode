
namespace Core.Room
{
    public interface IHallRoom
    {
        long HallRoomId { get; set; }

        IRoom ServerRoom { get; set; }

        ERoomGameStatus GameStatus { get; set; }
        int ModeId { get; set; }
        int TeamCapacity { get; set; }
        int MapId { get; set; }
        int RevivalTime { get; set; }         // 复活时间(单位:秒)
        bool MultiAngleStatus { get; set; }     // 切换第三人称
        bool WatchStatus { get; set; }          // 是否允许观战
        bool HelpStatus { get; set; }           // 允许救援
        bool HasFriendHarm { get; set; }        // 友军伤害
        int WaitTimeNum { get; set; }           // 等待阶段时间设置(单位:秒)
        int OverTime { get; set; }              // 游戏结束时间(单位:秒)
        int ConditionType { get; set; }
        int ConditionValue { get; set; }
        string ChannelName { get; set; }
        string RoomName { get; set; }
        int RoomDisplayId { get; set; }
        int RoomCapacity { get; set; }

        long CreateTime { get; set; }

        bool IsValid { get; set; }

        void AddPlayer(IPlayerInfo player);

        IPlayerInfo GetPlayer(long playerId);

        void RemovePlayer(long playerId);

        bool HasPlayer(long playerId);

        bool CanJoin();

        int MaxNum(long teamId);

        void UpdateRoomGameStatus(ERoomGameStatus status, ERoomEnterStatus enter);

        void UpdatePlayerStatus(long playerId, EPlayerGameStatus status);

        void PlayerLeaveRoom(long playerId);

        void PlayerLoginSucc(long playerId);

        void GameOver();

        void Dispose();

        void CheckTimeOut();
    }

    public interface ITeamInfo
    {
        long TeamId { get; set; }
        int PlayerCount { get; set; }
        int TotalRankScore { get; set; }
        float AvgRankScore { get; set; }

        int TotalKillCount { get; set; }
        int TotalSaveCount { get; set; }
        int TotalAliveMinute { get; set; }
        float TotalMemberRankScore { get; set; }    //(51 - rank)*0.6
        bool IsChicken { get; set; }
        int Rank { get; set; }
    }

}
