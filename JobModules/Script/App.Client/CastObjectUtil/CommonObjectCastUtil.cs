//#define DEBUG_OBSTACLE
using Core.Utils;
using UnityEngine;
using System;
using App.Client.GameModules.SceneObject;

namespace App.Client.CastObjectUtil
{
    public static class CommonObjectCastUtil
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonObjectCastUtil));
        private const int _maxCastCount = 10;
        private static int _castCount = 0;

        private static GameObject _startGo;
        private static GameObject _targetGo;
        public static bool HasObstacleBeteenPlayerAndItem(PlayerEntity player, Vector3 targetCenter, GameObject item)
        {
            //  Logger.Debug("HasObstacleBeteenPlayerAndItem");
            if(null == player)
            {
                Logger.Error("player is null");
                return true;
            }
            _castCount = 0;
            var startPoint = player.cameraStateOutputNew.FinalArchorPosition;
            int mask = ~(UnityLayerManager.GetLayerMask(EUnityLayerName.UserInputRaycast) | UnityLayerManager.GetLayerMask(EUnityLayerName.Player) | UnityLayerManager.GetLayerMask(EUnityLayerName.Vehicle) | UnityLayerManager.GetLayerMask(EUnityLayerName.NoCollisionWithBullet));
#if DEBUG_OBSTACLE
            DebugDraw.DebugArrow(startPoint, targetCenter - startPoint);
            if(null == _startGo)
            {
                _startGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _startGo.GetComponent<Collider>().enabled = false;
                _targetGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _targetGo.GetComponent<Collider>().enabled = false;
                _startGo.transform.localScale = Vector3.one * 0.1f;
                _targetGo.transform.localScale = Vector3.one * 0.1f;
            }
            _startGo.transform.position = startPoint;
            _targetGo.transform.position = targetCenter;
#endif
            var dir = startPoint - targetCenter;
            var ray = new Ray(targetCenter, dir.normalized);
            var inRangeOffset = -0.01f;
            var hits = Physics.RaycastAll(ray, dir.magnitude - inRangeOffset, mask);
            foreach(var hit in hits)
            {
                Logger.DebugFormat("hit {0}", hit.collider.name);
                if(Ignore(hit, player, item))
                {
                    continue;
                }
                else
                {
                    OnObstacle(true);
        //            Logger.Debug("onObstacle");
                    return true;
                }
            }
        //    Logger.Debug("onObstacle");
            OnObstacle(false);
            return false;
        }

        private static void OnObstacle(bool obstacle)
        {
#if DEBUG_OBSTACLE
            _startGo.GetComponent<Renderer>().sharedMaterial.color = obstacle ? Color.red : Color.white;
#endif
        }

        private static bool Ignore(RaycastHit hitInfo, PlayerEntity player, GameObject item)
        {
            if(IsPlayerSelf(hitInfo, player))
            {
                return true;
            }

            if(IsTrigger(hitInfo))
            {
                return true;
            }

            if(IsTerrian(hitInfo))
            {
                return true;
            }

            if(IsVehicleTrigger(hitInfo))
            {
                return true;
            }

            if(IsItemSelf(hitInfo, item))
            {
                return true;
            }

            return false;
        }

        private static bool IsItemSelf(RaycastHit hitInfo, GameObject item)
        {
            if(null == item)
            {
                return false;
            }
            var parent = hitInfo.transform.parent;
            while(null != parent)
            {
                if(parent == item.transform)
                {
                    return true;
                }
                parent = parent.parent;
            }
            return false;
        }
        
        private static bool IsPlayerSelf(RaycastHit hitInfo, PlayerEntity player)
        {
            if(hitInfo.transform== player.characterContoller.Value.transform)
            {
                return true;
            }
            return false;
        }
        
        private static bool IsTrigger(RaycastHit hitInfo)
        {
            var parent = hitInfo.collider.transform.parent;
            if(null == parent)
            {
                return false;
            }
            var triggerCol = parent.Find(SceneObjectConstant.NormalColliderName);
            if(null != triggerCol)
            {
                return true;
            }
            return false;
        }

        private static bool IsVehicleTrigger(RaycastHit hitInfo)
        {
            var isTrigger = hitInfo.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.VehicleTrigger);
            var isHitBox = hitInfo.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Hitbox);
            return isTrigger;
        }

        private static bool IsTerrian(RaycastHit hitInfo)
        {
            var terrain = hitInfo.collider.gameObject.GetComponent<Terrain>();
            var isTerrainLayer = hitInfo.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Terrain);
            return terrain != null || isTerrainLayer;
        }
    }
}
