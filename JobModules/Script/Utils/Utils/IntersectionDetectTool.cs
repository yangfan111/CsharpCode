using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Utils.Utils
{
    public static class IntersectionDetectTool
    {
        public static Collider[] _internalColliders = new Collider[16];
        public static RaycastHit[] _internalCharacterHits = new RaycastHit[16];
        public static List<Collider> _colliderList = new List<Collider>(128);
        
        public static List<bool> ColliderEnableState = new List<bool>(128);
        /// <summary>
        /// Raycasts to detect collision hits
        /// </summary>
        /// <returns> Returns the number of hits </returns>
        public static int CharacterCollisionsRaycast(Vector3 position, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits)
        {
            direction.Normalize();

            // Raycast
            int nbHits = 0;
            int nbUnfilteredHits = Physics.RaycastNonAlloc(
                position,
                direction,
                hits,
                distance,
                UnityLayers.AllCollidableLayerMask,
                QueryTriggerInteraction.Ignore);

            // Hits filter
            closestHit = new RaycastHit();
            float closestDistance = Mathf.Infinity;
            nbHits = nbUnfilteredHits;
            for (int i = nbUnfilteredHits - 1; i >= 0; i--)
            {
                // Filter out the invalid hits
                if (hits[i].distance <= 0f)
                {
                    nbHits--;
                    if (i < nbHits)
                    {
                        hits[i] = hits[nbHits];
                    }
                }
                else
                {
                    // Remember closest valid hit
                    if (hits[i].distance < closestDistance)
                    {
                        closestHit = hits[i];
                        closestDistance = hits[i].distance;
                    }
                }
            }

            return nbHits;
        }
        
        public static void SetColliderLayer(GameObject gameObject, int layer)
        {
            _colliderList.Clear();
            gameObject.GetComponentsInChildren<Collider>(_colliderList);
            foreach (var v in _colliderList)
            {
                v.gameObject.layer = layer;
            }
            _colliderList.Clear();
        }
        
        public static void SetColliderDisable(GameObject gameObject, List<bool> ret)
        {
            _colliderList.Clear();
            gameObject.GetComponentsInChildren<Collider>(_colliderList);
            foreach (var v in _colliderList)
            {
                ret.Add(v.enabled);
                v.enabled = false;
            }
            _colliderList.Clear();
        }
        
        public static void RestoreCollider(GameObject gameObject, List<bool> restoreValue)
        {
            _colliderList.Clear();
            gameObject.GetComponentsInChildren<Collider>(_colliderList);
            AssertUtility.Assert(restoreValue.Count == _colliderList.Count);
            for (int i = 0; i < restoreValue.Count; ++i)
            {
                _colliderList[i].enabled = restoreValue[i];
            }
            _colliderList.Clear();
            restoreValue.Clear();
        }
    }
}