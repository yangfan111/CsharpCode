using System;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using UltimateFracturing;
using UnityEngine;

namespace App.Shared.GameModules.Attack
{
    public class HitFracturedHandler
    {
        public static FracturedAbstractChunk HitFracturedObj(PlayerEntity src, RaycastHit hit, FracturedHittable fracturedHittable)
        {
            if (fracturedHittable != null)
            {
                var script = fracturedHittable.gameObject.GetComponent<FracturedHittable>();
                if (script == null) return null;

                var objtype = MapObjectUtility.GetGameObjType(script.Owner);
                if (objtype < 0) return null;

                int rawObjKey = MapObjectUtility.GetGameObjId(script.Owner);

                if (rawObjKey != Int32.MinValue && !SharedConfig.IsServer &&
                    MapObjectUtility.GetMapObjectByGameObject(script.Owner) == null)
                {
                    MapObjectUtility.SendCreateMapObjMsg(objtype, rawObjKey);
                    BulletHitHandler._logger.InfoFormat("CreateMapObjEvent: type:{0}, obj:{1}, num:{2}",
                        (ETriggerObjectType) objtype,
                        fracturedHittable.gameObject, src.uploadEvents.Events.Count);
                }

                return fracturedHittable.Hit(hit.point, hit.normal);
            }
            
            return null;
        }
    }
}