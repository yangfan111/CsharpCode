using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.SceneObject
{
    public static class SceneObjectDropUtil
    {
        public  delegate bool RaycastDel(Vector3 origin, Vector3 direction, out RaycastHit hitInfo,
                                             float maxDistance, int layerMask);
        private readonly static RaycastDel defaultRaycast = Physics.Raycast;
        
        private static readonly Vector3 dropFixedOffset = new Vector3(0, 0.1f, 0);
        private static readonly int DropMaxCastDistance = 100;
        private static readonly int dropLayer = UnityLayers.PickupObstacleLayerMask;
        /// <summary>
        /// 从玩家手上到预定drop点连线，判断中间是否有物体阻挡
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="hitInfo"></param>
        /// <param name="specifiedRaycast"></param>
        /// <returns></returns>
        public static bool RaycastPlayerDropObject(PlayerEntity playerEntity,Vector3? dropPos,out RaycastHit hitInfo,RaycastDel specifiedRaycast= null)
        {
            specifiedRaycast = specifiedRaycast ?? defaultRaycast;
            var handPosVal = SceneObjectPositionUtil.GetHandObjectPosition(playerEntity);
            var dropPosVal =  dropPos.HasValue?dropPos.Value: SceneObjectPositionUtil.GetPlayerDropPos(playerEntity);
            return (specifiedRaycast(handPosVal, dropPosVal - handPosVal, out hitInfo, Vector3.Distance(handPosVal, dropPosVal),
                dropLayer));
        }

       
        /// <summary>
        /// 原始drop位置向下100M做检测，获取落到地面位置
        /// </summary>
        /// <param name="dropPos"></param>
        /// <param name="specifiedRaycast"></param>
        /// <returns></returns>
        public static Vector3 GetFixedDropPos(Vector3 dropPos, RaycastDel specifiedRaycast = null)
        {
            specifiedRaycast = specifiedRaycast ?? defaultRaycast;
            RaycastHit vhit;
            if (specifiedRaycast(dropPos +dropFixedOffset , Vector3.down, out vhit, DropMaxCastDistance,
                dropLayer))
            {
                return vhit.point;
            }

            return dropPos;
        }


        public static Vector3 GetDropObjectPos(PlayerEntity playerEntity,RaycastDel specifiedRaycast = null)
        {
            var dropPos = SceneObjectPositionUtil.GetPlayerDropPos(playerEntity);

            RaycastHit hitInfo;
            if (RaycastPlayerDropObject(playerEntity,dropPos, out hitInfo,specifiedRaycast))
            {
                dropPos = GetFixedDropPos(hitInfo.point,specifiedRaycast);
            }
            else
            {
                dropPos = GetFixedDropPos(dropPos,specifiedRaycast);
            }

            return dropPos;
        }

    }
}