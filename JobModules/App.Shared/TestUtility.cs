using Core.Room;

namespace App.Shared
{
    public class TestUtility
    {
        public static string TestToken = "wdTest";
        public static string RobotToken = "wdTest_Robot";
        public static long PlayerId = 0;
        public static long RobotrIdBase = 100;
        public static string PlayerName = "Test";
        public static string RobotName = "Robot";
        public static int RoleModelId = 2;
        public static long TeamId = 0;
        public static string TestRobotToken = "robotTest";

        private static long TestMaxPlayerId = 1000;

        public static IPlayerInfo CreateTestPlayer()
        {
            return new PlayerInfo(TestToken,null, PlayerId, PlayerName, RoleModelId, TeamId,0,0,0,0,0,null,null,false);
        }

        public static IPlayerInfo CreateTestPlayer(IRoomId roomId)
        {
            return new PlayerInfo(TestToken, roomId, PlayerId, PlayerName, RoleModelId, TeamId,0,0,0,0,0,null,null,false);
        }

        public static long NewPlayerId
        {
            get { return ++TestMaxPlayerId; }
        }
    }
}