using UnityEngine;

namespace ECM.Components
{
    public static class PhysicsCastHelper
    {
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
    }
}