namespace App.Shared
{
    public class RpcMessageType
    {
        public static string RegisterBattleServer = "registerBattleServer";
        public static string UpdateBattleServerStatus = "updateBattleServerStatus";
        public static string RegisterRoomServer = "registerRoomServer";
        public static string RequestCreateRoom = "requestCreateRoom";
        public static string ResponseCreateRoom = "responseCreateRoom";
        public static string SyncPlayerStatus = "syncPlayerStatus";
        public static string GameOver = "gameOver";
        public static string SingleGameOver = "singleGameOver";
        public static string LeaveRoom = "leaveRoom";

        public static string RequestJoinRoom = "requestJoinRoom";
        public static string ResponseJoinRoom = "responseJoinRoom";
        public static string UpdateRoomGameState = "updateRoomGameState";
        public static string MandateLogOut = "mandateLogout";

        public static string ServerHeartBeat = "heartBeat";
    }
}
