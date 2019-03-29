using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using App.Shared.VechilePrediction;
using App.Shared.VehicleGameHandler;
using Core.GameModule.Module;
using VehicleCommon;

namespace App.Server.GameModules.Vehicle
{
    public class ServerVehicleModule : GameModule
    {
        public ServerVehicleModule(Contexts contexts, VehicleTimer vehicleTimer)
        {
            AddSystem(new VehicleHitBoxInitSystem(contexts));
            AddSystem(new ServerVehicleEntityInitSystem(contexts));
            AddSystem(new VehicleSyncPositionSystem(contexts));
            AddSystem(new VehicleEntityDeactiveSystem(contexts.vehicle));
            AddSystem(new RideOffVehicleOnPlayerDestroy(contexts));
            AddSystem(new VehicleGameStateUpdateSystem(contexts.vehicle, new ServerVehicleGameHandlerRegister(contexts)));
            //AddSystem(new VehicleRideSystem(contexts.vehicle));
            AddSystem(new ServerVehicleCmdExecuteSystem(contexts, vehicleTimer));
            AddSystem(new PlayerControlledVehicleUpdateSystem(contexts));
            AddSystem(new VehicleSoundSelectSystem(contexts.vehicle));
        }
    }
}
