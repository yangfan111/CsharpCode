using System.Collections.Generic;

namespace Assets.Sources.Free.Auto
{
    public class PlayerStatus
    {
        private static IDictionary<int, int> pingMap = new Dictionary<int, int>();

        public PlayerStatus()
        {
        }

        public static int GetPing(int id)
        {
            if (pingMap.ContainsKey(id))
            {
                return pingMap[id];
            }

            return 0;
        }

//        public static void SetPing(PingListData pingList)
//        {
//            for (var i = 0; i < pingList.List.Count; i++)
//            {
//                var playerPingData = pingList.List[i];
//                if (playerPingData != null)
//                {
//                    var ce = Contexts.sharedInstance.player.GetEntityWithEntityId(playerPingData.Id);
//                    if (ce != null)
//                    {
//                        pingMap[playerPingData.Id] = playerPingData.Ping;
//                    }
//                }
//            }
//        }
    }
}
