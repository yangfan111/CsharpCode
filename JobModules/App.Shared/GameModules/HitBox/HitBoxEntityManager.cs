using App.Shared.Components.Common;
using App.Shared.Components.Player;
using Core.Compensation;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.HitBox;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Bullet
{
    public interface IHitBoxContext
    {
        HitBoxComponent GetHitBoxComponent(EntityKey entityKey);
        void UpdateHitBox(IGameEntity gameEntity);
       
    }

    public class HitBoxEntityManager : IHitBoxEntityManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxEntityManager));

        private IHitBoxContext[] _subManagers;

        public HitBoxEntityManager(Contexts contexts, bool isServer)
        {
            _subManagers = new IHitBoxContext[(int)EEntityType.End];
            if (isServer)
                _subManagers[(int) EEntityType.Player] = new ServerPlayerHitBoxContext(contexts.player);
            else 
                _subManagers[(int) EEntityType.Player] = new ClientPlayerHitBoxContext(contexts.player);
            _subManagers[(int)EEntityType.Vehicle] = new VehicleHitBoxContext(contexts.vehicle);
        }

        private Vector3 GetPosition(IGameEntity gameEntity)
        {
            var pos = gameEntity.Position.Value;
            var comp = GetHitBoxComponent(gameEntity);
            if (comp != null)
                return comp.HitPreliminaryGeo.position + pos;
            return pos;
        }

        private float GetRadius(IGameEntity gameEntity)
        {
            var comp = GetHitBoxComponent(gameEntity);
            if (comp != null)
                return comp.HitPreliminaryGeo.radius;
            return 0;
        }

        public bool GetPositionAndRadius(IGameEntity gameEntity, out Vector3 position, out float radius)
        {
            var comp = GetHitBoxComponent(gameEntity);
            var pos = gameEntity.Position.Value;
            if (comp != null)
            {
                position = comp.HitPreliminaryGeo.position + pos;
                radius= comp.HitPreliminaryGeo.radius;
                return true;
            }
            else
            {
                position = Vector3.zero;
                radius = 0;
                return false;
            }
        }

        private IHitBoxContext GetSubManager(int type)
        {
            if (_subManagers.Length > type)
            {
                return _subManagers[type];
            }

            return null;
        }

        public HitBoxComponent GetHitBoxComponent(IGameEntity gameEntity)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
                return subManager.GetHitBoxComponent(gameEntity.EntityKey);
            return null;
        }

        public void UpdateHitBox(IGameEntity gameEntity)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
            {
                subManager.UpdateHitBox(gameEntity);
            }

            
        }
   

        public void DrawHitBoxOnBullet(IGameEntity gameEntity)
        {
            if (DebugConfig.DrawHitBoxOnBullet)
            {
                DrawHitBox(gameEntity, DebugConfig.DrawHitBoxDuration);
            }
        }

        public void DrawHitBoxOnFrame(IGameEntity gameEntity)
        {
            if (DebugConfig.DrawHitBoxOnFrame)
            {
                DrawHitBox(gameEntity, 0);
            }
        }

      

        public void DrawHitBox(IGameEntity gameEntity, float time)
        {

            var hitBoxComponent = GetHitBoxComponent(gameEntity);
            if (hitBoxComponent != null)
            {
                var position = gameEntity.Position.Value;
                DebugDraw.DebugWireSphere(GetPosition(gameEntity),
                    GetRadius(gameEntity), time);
                HitBoxGameObjectUpdater.DrawBoundBox(hitBoxComponent.HitBoxGameObject.transform, time);
            }

        }



        public void EnableHitBox(IGameEntity gameEntity, bool enable)
        {
            var comp = GetHitBoxComponent(gameEntity);
            if (comp != null)
                comp.HitBoxGameObject.SetActive(enable);
        }

        public bool Raycast(Ray rayRay, out RaycastHit hitInfo, float rayLength, int hitboxLayerMask)
        {
            return Physics.Raycast(rayRay, out hitInfo, rayLength, hitboxLayerMask);
        }

        public bool BoxCast(BoxInfo box, out RaycastHit hitInfo, int hitboxLayerMask)
        {
            return Physics.BoxCast(box.Origin, box.HalfExtens, box.Direction, out hitInfo, box.Orientation, box.Length, hitboxLayerMask); 
        }
    }
}