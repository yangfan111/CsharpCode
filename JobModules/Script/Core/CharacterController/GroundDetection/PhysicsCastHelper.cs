using System;
using UnityEngine;

namespace ECM.Components
{
    public static class PhysicsCastHelper
    {
        #region CastHelper
        public const float defaultBackstepDistance = 0.05f;

        /// <summary>
        /// SphereCast helper method.
        /// </summary>
        /// <param name="origin">The center of the sphere at the start of the sweep.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="direction">The direction into which to sweep the sphere.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        /// <param name="triggerInteraction"></param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <param name="groundMask"></param>
        /// <returns>True when the intersects any 'ground' collider, otherwise false.</returns>
        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo,
            float distance, int groundMask, QueryTriggerInteraction triggerInteraction, float backstepDistance = defaultBackstepDistance)
        {
            origin = origin - direction * backstepDistance;

            var hit = Physics.SphereCast(origin, radius, direction, out hitInfo, distance + backstepDistance,
                groundMask, triggerInteraction);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }

        /// <summary>
        /// Raycast helper method.
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        /// <param name="triggerInteraction">Specifies whether casts should hit Triggers.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <param name="groundMask">Layers to be considered as 'ground' (walkables).</param>
        /// <returns>True when the intersects any 'ground' collider, otherwise false.</returns>
        public static  bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int groundMask, QueryTriggerInteraction triggerInteraction,
            float backstepDistance = defaultBackstepDistance)
        {
            origin = origin - direction * backstepDistance;

            var hit = Physics.Raycast(origin, direction, out hitInfo, distance + backstepDistance, groundMask,
                triggerInteraction);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }
        
        
        /// <summary>
        /// CapsuleCast helper method.
        /// </summary>
        /// <param name="bottom">The center of the sphere at the start of the capsule.</param>
        /// <param name="top">The center of the sphere at the end of the capsule.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="direction">The direction into which to sweep the sphere.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <param name="groundMask">Layers to be considered as 'ground' (walkables).</param>
        /// <param name="triggerInteraction">Specifies whether casts should hit Triggers.</param>
        /// <returns>True when the intersects any 'ground' collider, otherwise false.</returns>
        public static bool CapsuleCast(Vector3 bottom, Vector3 top, float radius, Vector3 direction, out RaycastHit hitInfo,
            float distance, int groundMask, QueryTriggerInteraction triggerInteraction, float backstepDistance = defaultBackstepDistance)
        {
            top = top - direction * backstepDistance;
            bottom = bottom - direction * backstepDistance;

            var hit = Physics.CapsuleCast(bottom, top, radius, direction, out hitInfo, distance + backstepDistance,
                groundMask, triggerInteraction);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }
        #endregion



        #region OverlapHelper
        private const int MaxCollisionBudget = 16;
        public static Collider[] CollidersArray = new Collider[MaxCollisionBudget];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bottomHemi">底部半球中心</param>
        /// <param name="topHemi">顶部半球中心</param>
        /// <param name="radius">半径</param>
        /// <param name="overlappedColliders"></param>
        /// <param name="layers"></param>
        /// <param name="triggerInteraction"></param>
        /// <param name="colliderFilter">过滤器，如果返回true，把这个collider从结果中删除</param>
        /// <returns></returns>
        public static int CapsuleOverlap(Vector3 bottomHemi, Vector3 topHemi, float radius, Collider[] overlappedColliders, LayerMask layers, QueryTriggerInteraction triggerInteraction,
            Func<Collider, bool> colliderFilter)
        {
            int nbHits = 0;
            int nbUnfilteredHits = Physics.OverlapCapsuleNonAlloc(
                bottomHemi,
                topHemi,
                radius,
                overlappedColliders,
                layers,
                triggerInteraction);

            // Filter out the character capsule itself
            nbHits = nbUnfilteredHits;
            for (int i = nbUnfilteredHits - 1; i >= 0; i--)
            {
                if (colliderFilter(overlappedColliders[i]))
                {
                    nbHits--;
                    if (i < nbHits)
                    {
                        overlappedColliders[i] = overlappedColliders[nbHits];
                    }
                }
            }

            return nbHits;
        }

        public static void GetCapsule(CapsuleCollider collider, Vector3 position, Quaternion rotation,
            out Vector3 point1, out Vector3 point2, out float radius)
        {
            var center = collider.center;
            radius = collider.radius;
            var height = Mathf.Clamp(collider.height, radius * 2, collider.height);
            GetCapsule(position, rotation, height, radius, center, GetCapsuleForwardDirection(collider.direction),
                out point1, out point2);
        }
        
        public static void GetCapsule(CharacterController collider, Vector3 position, Quaternion rotation,
            out Vector3 point1, out Vector3 point2, out float radius)
        {
            var center = collider.center;
            radius = collider.radius;
            var height = Mathf.Clamp(collider.height, radius * 2, collider.height);
            GetCapsule(position, rotation, height, radius, center, Vector3.up, 
                out point1, out point2);
        }

        public static void GetCapsule(Vector3 position, Quaternion rotation, float height, float radius,
            Vector3 center, Vector3 localFoward ,out Vector3 point1, out Vector3 point2)
        {
            var characterTransformToCapsuleBottomHemi = center + (-localFoward * (height * 0.5f)) + (localFoward * radius);
            var characterTransformToCapsuleTopHemi = center + (localFoward * (height * 0.5f)) + (-localFoward * radius);
//            var characterTransformToCapsuleBottom = center + (-localFoward * (height * 0.5f));
//            var characterTransformToCapsuleTop = center + (localFoward * (height * 0.5f));
            point1 = position + rotation * characterTransformToCapsuleBottomHemi;
            point2 = position + rotation * characterTransformToCapsuleTopHemi;
        }
        
        public static void GetDebugDrawTypeCapsule(Vector3 position, Quaternion rotation, float height, float radius,
            Vector3 center, Vector3 localFoward ,out Vector3 point1, out Vector3 point2)
        {
            var characterTransformToCapsuleBottom = center + (-localFoward * (height * 0.5f));
            var characterTransformToCapsuleTop = center + (localFoward * (height * 0.5f));
            point1 = position + rotation * characterTransformToCapsuleBottom;
            point2 = position + rotation * characterTransformToCapsuleTop;
        }
        
        /// <summary>
        /// The direction of the capsule.The value can be 0, 1 or 2 corresponding to the X, Y and Z axes, respectively.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 GetCapsuleForwardDirection(int direction)
        {
            Vector3 ret;
            switch (direction)
            {
                case 0:
                    ret = Vector3.right;
                    break;
                case 1:
                    ret = Vector3.up;
                    break;
                case 2:
                    ret = Vector3.forward;
                    break;
                default:
                    ret = Vector3.up;
                    break;
            }

            return ret;
        }
        #endregion
        
    }
}