using UnityEngine;

namespace Core.HitBox
{
    public interface IHitBoxTransformProvider
    {
        void Update(Vector3 rootPosition, Quaternion rotation);
        Transform GetTransform(Transform bone);
        Vector3 RootPosition { get; }

        Quaternion RootRotation { get; }
    }
}