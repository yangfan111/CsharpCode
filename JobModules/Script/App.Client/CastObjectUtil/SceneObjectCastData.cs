using Core.Utils;
using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    public static class SceneObjCastData
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjCastData));
        private const int EntityIdIndex = 1;
        private const int ItemIdIndex = 2;
        private const int CountIndex = 3;
        private const int CategoryIndex = 4;

        public static void Make(RayCastTarget target, int entityId, int itemId, int count, int category)
        {
            if(null == target)
            {
                Logger.Error("target is null !!");
                return;
            }
            BaseCastData.Make(target, ECastDataType.SceneObject);
            var idList = target.IdList;
            idList.Add(entityId);
            idList.Add(itemId);
            idList.Add(count);
            idList.Add(category);
        }

        public static int EntityId(List<int> idList)
        {
            return idList[EntityIdIndex];
        }

        public static int ItemId(List<int> idList)
        {
            return idList[ItemIdIndex];
        }

        public static int Count(List<int> idList)
        {
            return idList[CountIndex];
        }

        public static int Category(List<int> idList)
        {
            return idList[CategoryIndex];
        }
    }
}
