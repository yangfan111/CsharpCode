using com.wd.free.map.position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.unit;
using UnityEngine;
using App.Shared.Configuration;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.map.position
{
    [Serializable]
    public class PosGroundSelector : AbstractPosSelector
    {
        private IPosSelector pos;
        private float waterDelta;

        public override UnitPosition Select(IEventArgs args)
        {
            UnitPosition up = pos.Select(args);

            Vector3 fromV = UnityPositionUtil.ToVector3(up);

            Vector3 toV = new Vector3(up.GetX(), -10000, up.GetZ());

            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo);

            if (hited)
            {
                if (SingletonManager.Get<MapConfigManager>().InWater(new Vector3(hitInfo.point.x,
                            hitInfo.point.y - 0.1f, hitInfo.point.z)))
                {
                    hitInfo.point = new Vector3(fromV.x, hitInfo.point.y - waterDelta, fromV.z);
                }

                //Debug.LogFormat("hit {0},{1},{2}", hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);

                return UnityPositionUtil.FromVector(hitInfo.point);
            }

            return up;
        }
    }
}
