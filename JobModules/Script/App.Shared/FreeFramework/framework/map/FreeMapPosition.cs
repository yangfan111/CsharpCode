using Core.Utils;
using Shared.Scripts.MapConfigPoint;
using System.Collections.Generic;

namespace App.Server.GameModules.GamePlay.Free.map
{
    public class FreeMapPosition
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FreeMapPosition));

        private static Dictionary<int, MapConfigPoints> cache = new Dictionary<int, MapConfigPoints>();
        
        public static void Init(Contexts contexts)
        {
            int map = contexts.session.commonSession.RoomInfo.MapId;
            if (!cache.ContainsKey(map))
            {
                cache.Add(map, MapConfigPoints.current);
            }
        }

        public static MapConfigPoints GetPositions(int map)
        {
            return cache[map];
        }
    }
}
