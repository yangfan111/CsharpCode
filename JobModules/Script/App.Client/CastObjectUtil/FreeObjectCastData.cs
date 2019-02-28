using Core.Utils;
using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    public static class FreeObjectCastData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FreeObjectCastData));
        private const int EntityIdIndex = 1;

        public static void Make(RayCastTarget target, int entityId)
        {
            if(null == target)
            {
                return;
            }
            BaseCastData.Make(target, ECastDataType.FreeObject);
            target.IdList.Add(entityId);
        }

        public static int EntityId(List<int> idList)
        {
            return idList[EntityIdIndex];
        }
    }
}
