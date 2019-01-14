using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using Entitas;
using VehicleCommon;

namespace App.Shared.VechilePrediction
{
    public class ServerVehicleCmdExecuteSystem : VehicleCmdExecuteSystem
    {
        public ServerVehicleCmdExecuteSystem(Contexts contexts, VehicleTimer vehicleTimer) : base(vehicleTimer)
        {
            EnableSyncFromComponent = SharedConfig.ServerAuthorative;
        }

        public override void SyncFromComponent(Entity entity)
        {
            if (EnableSyncFromComponent)
            {
                base.SyncFromComponent(entity);
            }   
        }

        public override  void ExecuteVehicleCmd(Entity entity, IVehicleCmd cmd)
        {
            if (SharedConfig.ServerAuthorative)
            {
                base.ExecuteVehicleCmd(entity, cmd);
            }
            else
            {
                var vehicle = (VehicleEntity)entity;
                VehicleStateUtility.ApplyVehicleCmdAndState(vehicle, cmd);
            }
        }
    }
}
