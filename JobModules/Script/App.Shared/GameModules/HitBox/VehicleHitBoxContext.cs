using App.Shared.Components.Common;
using App.Shared.Components.Player;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.Components;
using Core.EntityComponent;
using EVP;
using UnityEngine;

namespace App.Shared.GameModules.Bullet
{
    public class VehicleHitBoxContext : IHitBoxContext
    {
        private VehicleContext _vehicleContext;
        public VehicleHitBoxContext(VehicleContext context)
        {
            _vehicleContext = context;
        }

        public HitBoxComponent GetHitBoxComponent(EntityKey entityKey)
        {
            var entity = _vehicleContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasPosition && entity.hasHitBox)
            {
                return entity.hitBox;
            }

            return null;
        }

        public void UpdateHitBox(IGameEntity gameEntity)
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

      

        private VehicleEntity GetVehicleEntity(IGameEntity gameEntity)
        {
            return _vehicleContext.GetEntityWithEntityKey(gameEntity.EntityKey);
        }
    }
}