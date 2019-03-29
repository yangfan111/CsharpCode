using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    class CommonCastData
    {
        private const int EntityIdIndex = 1;
        private const int KeyIndex = 2;
        public static void Make(RayCastTarget target, int entityId, int key, string tip)
        {
            BaseCastData.Make(target, ECastDataType.Common);
            target.IdList.Add(entityId);
            target.IdList.Add(key);
            target.Data = tip;
        }

        public static int EntityId(List<int> idList)
        {
            return idList[EntityIdIndex];
        }

        public static int Key(List<int> idList)
        {
            return idList[KeyIndex];
        }

        public static string Tip(PointerData data)
        {
            return data.Data as string;
        }
    }
}
