using Core.Free;
using System.Collections.Generic;

namespace Core.Room
{
    public class RoomInfo : IRule
    {
        public long HallRoomId;
        public int ModeId;
        public int TeamCapacity;
        public int MapId;
        public int RevivalTime;
        public bool MultiAngleStatus = true;
        public bool WatchStatus;
        public bool HelpStatus;
        public bool HasFriendHarm;
        public int WaitTimeNum;
        public int OverTime;
        public int ConditionValue;
        public int ConditionType;
        public string ChannelName = "null";
        public string RoomName = "null";
        public int RoomDisplayId;
        public int RoomCapacity;
        public string PreLoadUI = "null";
        public List<string> PreLoadAssetInfo = new List<string>(); //AssetInfo Format: "AssetBundName0/AssetName0,AssetBundName1/AssetName1,..."

        public RoomInfo()
        {
            MultiAngleStatus = true;
        }

        public RoomInfo(IHallRoom room)
        {
            CopyFrom(room);
        }

        public void CopyFrom(IHallRoom room)
        {
            HallRoomId = room.HallRoomId;
            ModeId = room.ModeId;
            TeamCapacity = room.TeamCapacity;
            MapId = room.MapId;
            RevivalTime = room.RevivalTime;
            MultiAngleStatus = room.MultiAngleStatus;
            WatchStatus = room.WatchStatus;
            HelpStatus = room.HelpStatus;
            HasFriendHarm = room.HasFriendHarm;
            WaitTimeNum = room.WaitTimeNum;
            OverTime = room.OverTime;
            ConditionType = room.ConditionType;
            ConditionValue = room.ConditionValue;
            RoomCapacity = room.RoomCapacity;
            RoomName = room.RoomName;
            RoomDisplayId = room.RoomDisplayId;
            ChannelName = room.ChannelName;
            //PreLoadAssetInfo = "";
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.RoomInfo;
        }
    }
}
