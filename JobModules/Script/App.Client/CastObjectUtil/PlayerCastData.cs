using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    public static class PlayerCastData
    {
        private const int EntityIdIndex = 1;

        public static void Make(RayCastTarget target, int entityId)
        {
            BaseCastData.Make(target, ECastDataType.Player);
            target.IdList.Add(entityId);
        }

        public static int EntityId(List<int> idList)
        {
            return idList[EntityIdIndex];
        }
    }
}
