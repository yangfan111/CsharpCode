using App.Protobuf;
using Core.Room;

namespace App.Client.GameModules.Room
{
    public static class RoomInfoEx
    {
        public static void FromLoginSuccMsg(this RoomInfo roomInfo, LoginSuccMessage message)
        {
            roomInfo.MapId = message.MapId;
            roomInfo.ModeId = message.GameMode;
            roomInfo.TeamCapacity = message.TeamCapacity; ;
            roomInfo.RevivalTime = message.RevivalTime;
            roomInfo.MultiAngleStatus = message.MultiAngleStatus;
            roomInfo.WatchStatus = message.WatchStatus;
            roomInfo.HelpStatus = message.HelpStatus;
            roomInfo.HasFriendHarm = message.HasFriendHarm;
            roomInfo.WaitTimeNum = message.WaitTimeNum;
            roomInfo.OverTime = message.OverTime;
            roomInfo.ChannelName = message.ChannelName;
            roomInfo.RoomName = message.RoomName;
            roomInfo.RoomDisplayId = message.RoomDisplayId;
            roomInfo.RoomCapacity = message.RoomCapacity;
            roomInfo.PreLoadAssetInfo = message.PreLoadAssetInfo;
            roomInfo.PreLoadUI = message.PreLoadUI;
        }

        public static void ToLoginSuccMsg(this RoomInfo roomInfo, LoginSuccMessage message)
        {
            message.MapId = roomInfo.MapId;
            message.GameMode = roomInfo.ModeId;
            message.TeamCapacity = roomInfo.TeamCapacity; ;
            message.RevivalTime = roomInfo.RevivalTime;
            message.MultiAngleStatus = roomInfo.MultiAngleStatus;
            message.WatchStatus = roomInfo.WatchStatus;
            message.HelpStatus = roomInfo.HelpStatus;
            message.HasFriendHarm = roomInfo.HasFriendHarm;
            message.WaitTimeNum = roomInfo.WaitTimeNum;
            message.OverTime = roomInfo.OverTime;
            message.ChannelName = roomInfo.ChannelName == null ? "": roomInfo.ChannelName;
            message.RoomName = roomInfo.RoomName == null ? "" : roomInfo.RoomName;
            message.RoomDisplayId = roomInfo.RoomDisplayId;
            message.RoomCapacity = roomInfo.RoomCapacity;
            message.PreLoadAssetInfo = roomInfo.PreLoadAssetInfo;
            message.PreLoadUI = roomInfo.PreLoadUI;
        }
    }
}
