using App.Shared.Configuration;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using Core.Free;
using Core.Utils;
using System;
using UnityEngine;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.map.position
{
    [Serializable]
    public class PosGroundSelector : AbstractPosSelector, IRule
    {
        private IPosSelector pos;
        private float waterDelta;

        public override UnitPosition Select(IEventArgs args)
        {
            UnitPosition up = pos.Select(args);
            Vector3 position = UnityPositionUtil.ToVector3(up);

            RaycastHit hitInfo;
            if (SingletonManager.Get<MapConfigManager>().InWater(position))
            {
                Ray ray = new Ray(position, Vector3.up);
                bool hit = Physics.Raycast(ray, out hitInfo, 10000, UnityLayerManager.GetLayerMask(EUnityLayerName.WaterTrigger));
                if (hit)
                {
                    return UnityPositionUtil.FromVector(hitInfo.point - new Vector3(0, waterDelta, 0));
                }
            }
            else
            {
                Ray ray = new Ray(position, Vector3.down);
                bool hit = Physics.Raycast(ray, out hitInfo, 10000, UnityLayers.SceneCollidableLayerMask | UnityLayerManager.GetLayerMask(EUnityLayerName.WaterTrigger));
                if (hit)
                {
                    if (hitInfo.collider.transform.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger))
                    {
                        return UnityPositionUtil.FromVector(hitInfo.point - new Vector3(0, waterDelta, 0));
                    }
                    else
                    {
                        return UnityPositionUtil.FromVector(hitInfo.point);
                    }
                }
            }
            return up;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PosGroundSelector;
        }
    }
}
