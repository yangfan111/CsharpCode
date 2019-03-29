using Core.EntityComponent;
using UnityEngine;
using WeaponConfigNs;

namespace Core.Effect
{
    public interface IEffectPlayer
    {
        void PlayBulletDrop(EntityKey owner, Vector3 position, float yaw,
            EBulletCaliber caliber);

        void PlayBulletClipDrop(EntityKey owner, Vector3 position, float yaw, EBulletCaliber caliber);

    }
}
