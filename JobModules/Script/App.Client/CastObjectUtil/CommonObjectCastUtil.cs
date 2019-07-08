//#define DEBUG_OBSTACLE
using App.Client.GameModules.SceneObject;
using App.Shared.Player;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Bone;

namespace App.Client.CastObjectUtil
{
    public static class CommonObjectCastUtil
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonObjectCastUtil));
        private const int _maxCastCount = 10;
        private static int _castCount = 0;

        private static GameObject _startGo;
        private static GameObject _targetGo;
        public static bool HasObstacleBetweenPlayerAndItem(PlayerEntity player, Vector3 targetCenter, GameObject item)
        {
            //  Logger.Debug("HasObstacleBeteenPlayerAndItem");
            if(null == player)
            {
                Logger.Error("player is null");
                return true;
            }
            _castCount = 0;
            /*var startPoint = player.cameraStateOutputNew.FinalArchorPosition;
            int mask = ~(UnityLayerManager.GetLayerMask(EUnityLayerName.UserInputRaycast) | UnityLayerManager.GetLayerMask(EUnityLayerName.Player)
                       | UnityLayerManager.GetLayerMask(EUnityLayerName.Vehicle) | UnityLayerManager.GetLayerMask(EUnityLayerName.NoCollisionWithBullet)
                       | UnityLayerManager.GetLayerMask(EUnityLayerName.NoCollisionWithEntity));*/
            var startPoint = BoneMount.FindChildBoneFromCache(player.RootGo(), BoneName.CharacterHeadBoneName).position;
            var dir = startPoint - targetCenter;
            var ray = new Ray(targetCenter, dir.normalized);
            var inRangeOffset = -0.01f;
            var hits = Physics.RaycastAll(ray, dir.magnitude - inRangeOffset, UnityLayers.PickupObstacleLayerMask);
            foreach(var hit in hits)
            {
                Logger.DebugFormat("hit {0}", hit.collider.name);
                if(Ignore(hit, player, item))
                {
                    continue;
                }
                else
                {
                    return true;
                }
            }
            return false;
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
