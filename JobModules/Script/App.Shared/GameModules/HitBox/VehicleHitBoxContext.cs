using System.Collections.Generic;
using App.Shared.Components.Common;
using App.Shared.Components.Player;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.Components;
using Core.EntityComponent;
using Core.HitBox;
using EVP;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.Attack
{
    public class VehicleHitBoxContext : IHitBoxContext
    {
        private VehicleContext _vehicleContext;
        public VehicleHitBoxContext(VehicleContext context)
        {
            _vehicleContext = context;
        }

        public Vector3 GetPosition(EntityKey entityKey)
        {
            var entity = _vehicleContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasPosition && entity.hasHitBox)
            {
                return entity.hitBox.HitPreliminaryGeo.position;
            }
            return Vector3.zero;
        }

        public float GetRadius(EntityKey entityKey)
        {
            var entity = _vehicleContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasPosition && entity.hasHitBox)
            {
                return entity.hitBox.HitPreliminaryGeo.radius;
            }
            return -1;
        }

        public void EnableHitBox(EntityKey entityKey, bool enalbe)
        {
            var entity = _vehicleContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasHitBox)
            {
                entity.hitBox.HitBoxGameObject.SetActive(enalbe);
            }
        }

        public List<Transform> GetCollidersTransform(EntityKey entityKey)
        {
            var result = new List<Transform>();
            var entity = _vehicleContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasHitBox)
            {
                result.Add(entity.hitBox.HitBoxGameObject.transform);
            }
            return result;
        }

        private HitBoxComponent GetHitBoxComponent(EntityKey entityKey)
        {
            var entity = _vehicleContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasPosition && entity.hasHitBox)
            {
                return entity.hitBox;
            }
            return null;
        }

        public void UpdateHitBox(IGameEntity gameEntity, int renderTime, int cmdSeq)
        {
            var position = gameEntity.Position.Value;
            var hitBoxComponent = GetHitBoxComponent(gameEntity.EntityKey);
            if (hitBoxComponent != null)
            {
                hitBoxComponent.HitBoxGameObject.transform.position = position;
            }

            var vehicle = GetVehicleEntity(gameEntity);
            vehicle.UpdateHitBoxes(gameEntity);
        }

        public void RecoverHitBox(IGameEntity gameEntity, int renderTime)
        {
           
        }

        private VehicleEntity GetVehicleEntity(IGameEntity gameEntity)
        {
            return _vehicleContext.GetEntityWithEntityKey(gameEntity.EntityKey);
        }
    }
}