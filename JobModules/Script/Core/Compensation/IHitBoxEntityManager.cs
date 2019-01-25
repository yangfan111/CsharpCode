using Core.EntityComponent;
using UnityEngine;

namespace Core.Compensation
{
    public interface IHitBoxEntityManager
    {
     
        bool GetPositionAndRadius(IGameEntity gameEntity, out Vector3 position, out float radius);
        void UpdateHitBox(IGameEntity gameEntity);
        void EnableHitBox(IGameEntity entity, bool enable);
        bool Raycast(Ray rayRay, out RaycastHit hitInfo, float rayLength, int hitboxLayerMask);
        bool BoxCast(BoxInfo rayRay, out RaycastHit hitInfo, int hitboxLayerMask);
        void DrawHitBoxOnBullet(IGameEntity player);
        void DrawHitBoxOnFrame(IGameEntity player);
     
       
    }
}