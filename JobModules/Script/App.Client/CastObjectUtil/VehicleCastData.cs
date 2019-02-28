using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    public static class VehicleCastData
    {
        private const int EntityIdIndex = 1;
        private const int SeatIndex = 2;

        public static void Make(RayCastTarget target, int entityId)
        {
            BaseCastData.Make(target, ECastDataType.Vehicle);
            target.IdList.Add(entityId);
        }
        
        public static int EntityId(List<int> idList)
        {
            return idList[EntityIdIndex];
        }
    }
}
