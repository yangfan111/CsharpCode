using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.Vehicle;
using Entitas;
using VehicleCommon;

namespace App.Shared.VechilePrediction
{
    public class ClientVehicleCmdExecuteSystem : VehicleCmdExecuteSystem
    {
        private PlayerContext _playerContext; 
        public ClientVehicleCmdExecuteSystem(Contexts contexts, VehicleTimer vehicleTimer):base(vehicleTimer)
        {
            _playerContext = contexts.player;
            EnableSyncFromComponent = SharedConfig.ServerAuthorative || SharedConfig.DynamicPrediction;
        }

        public override void SyncFromComponent(Entity entity)
        {
            if (EnableSyncFromComponent)
            {
                if (SharedConfig.DynamicPrediction)
                {
                    var vehicle = (VehicleEntity)entity;
                    if (vehicle.hasOwnerId &&
                        vehicle.ownerId.Value.Equals(_playerContext.flagSelfEntity.entityKey.Value))
                    {
                        return; ;
                    }
                }

                base.SyncFromComponent(entity);
            }
        }
    }
}
