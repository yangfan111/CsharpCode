using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using UnityEngine;
using com.wd.free.util;
using App.Shared.Configuration;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.condition
{
    [Serializable]
    public class PosTypeCondition : IParaCondition
    {
        [NonSerialized]
        public const int Water = 1;
        public const int Ground = 2;

        private string x;
        private string y;

        private int type;

        public bool Meet(IEventArgs args)
        {
            int realX = FreeUtil.ReplaceInt(x, args);
            int realY = FreeUtil.ReplaceInt(y, args);
            Ray r = new Ray(new Vector3(realX, 1000, realY), new Vector3(0, -1000, 0));

            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo);
            
            switch (type)
            {
                case Water:
                    if (hited)
                    {
                        return SingletonManager.Get<MapConfigManager>().InWater(new Vector3(hitInfo.point.x,
                            hitInfo.point.y - 0.1f, hitInfo.point.z));
                    }

                    return false;
                case Ground:
                    if (hited)
                    {
                        return !SingletonManager.Get<MapConfigManager>().InWater(new Vector3(hitInfo.point.x,
                            hitInfo.point.y - 0.1f, hitInfo.point.z));
                    }

                    return false;
            }

            return false;
        }
    }
}
