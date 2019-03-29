using App.Client.GameModules.UserPhysics;
using App.Shared;
using App.Shared.Components.ClientSession;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using App.Shared.VechilePrediction;
using App.Shared.VehicleGameHandler;
using Core.GameModule.Module;
using Core.GameModule.System;
using VehicleCommon;


namespace App.Client.GameModules.Vehicle
{
    public class ClientVehicleModule : GameModule
    {
        public ClientVehicleModule(Contexts contexts)
        {
            AddSystem(new VehicleEntityDeactiveSystem(contexts.vehicle));
            AddSystem(new ClientVehicleEntityInitSystem(contexts));
            AddSystem(new VehicleHitBoxInitSystem(contexts));
            //AddSystem(new VehicleRideSystem(contexts.vehicle));
            AddSystem(new PlayerControlledVehicleUpdateSystem(contexts));
            AddSystem(new VehicleGameStateUpdateSystem(contexts.vehicle, new ClientVehicleGameHandlerRegister(contexts)));
            AddSystem(new ClientVehicleCmdExecuteSystem(contexts, contexts.session.clientSessionObjects.VehicleTimer));
            AddSystem(new ClientVehicleSoundUpdateSystem(contexts, contexts.session.entityFactoryObject.SoundEntityFactory, contexts.session.clientSessionObjects.SoundPlayer));
            if (SharedConfig.IsOffline)
            {
                AddSystem(new VehicleSoundSelectSystem(contexts.vehicle));
            }

            AddSystem(new SimulationTimeSyncClientSystem(contexts));
            AddSystem(new VehicleSyncPositionSystem(contexts));
            AddSystem(new ClientVehiclePlaybackSystem(contexts.player, contexts.vehicle));

            AddSystem(new ClientVehicleOwnerIdActiveSystem(contexts));
        }

    }
}
