using System.Collections.Generic;
using UnityEngine;

namespace Core.HitBox
{
    public interface IHitBoxTransformProvider
    {
        float BoundSphereRadius();

        Vector3 BoundSpherePosition();

        void SetActive(bool active);
        
        Dictionary<string, Collider> GetHitBoxColliders();

        Dictionary<string, Transform> GetHitBoxTransforms();
    }
}