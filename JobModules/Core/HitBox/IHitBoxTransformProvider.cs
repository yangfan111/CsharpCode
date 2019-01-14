using UnityEngine;

namespace Core.HitBox
{
    public interface IHitBoxTransformProvider
    {
        Transform GetTransform(string boneName);
        Vector3 RootPosition { get; }

        Quaternion RootRotation { get; }
    }
}