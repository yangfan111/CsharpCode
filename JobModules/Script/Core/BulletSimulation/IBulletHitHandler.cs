using Core.Compensation;
using UnityEngine;

namespace Core.BulletSimulation
{
    public interface IBulletHitHandler
    {
        IBulletHitHandler SetHitLayerMask(int hitLayerMask);
        void OnHit(int cmdSeq, IBulletEntity bulletEntity, RaycastHit hit, ICompensationWorld compensationWorld);
    }
}