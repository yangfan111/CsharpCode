using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{   
    public static class DoorCastData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DoorCastData));
        private const int EntityIdIndex = 1;

        public static void Make(RayCastTarget target, int entityId)
        {
            if(null == target)
            {
                Logger.Error("target is null");
                return;
            }

            if (!target.IdList.Contains(entityId))
            {
                BaseCastData.Make(target, ECastDataType.Door);
                target.IdList.Add(entityId);
            }
        }

        public static int EntityId(List<int> idList)
        {
            return idList[EntityIdIndex];
        }
    }
}
