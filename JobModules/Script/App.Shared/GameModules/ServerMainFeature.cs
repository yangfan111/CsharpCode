using App.Client.ClientGameModules.System;
using App.Shared.GameModules.Common;
using App.Shared.UpdateLatest;
using Core;
using Core.AssetManager;
using Core.EntitasAdpater;
using Core.GameModule.Common;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Room;


namespace App.Shared.GameModules
{
    public sealed class ServerMainFeature : Feature
    {
        public ServerMainFeature(
            string name,
            IGameModule topLevelGameModule,
            IUserCmdExecuteSystemHandler userCmdExecuteSystemHandler,
            IVehicleCmdExecuteSystemHandler vehicleCmdExecuteSystemHandler,
            ISimulationTimer simluationTimer,
            IVehicleExecutionSelector vehicleExecutionSelector,
            ICommonSessionObjects sessionObjects,
            IRoom room) : base(name)
        {
            topLevelGameModule.Init();
            Add(new ModuleInitSystem(topLevelGameModule, sessionObjects.AssetManager));
            Add(new EntityCreateSystem(topLevelGameModule));

            Add(new GameStateUpdateSystem(topLevelGameModule));

            Add(new PhysicsInitSystem(topLevelGameModule));
            Add(new PhysicsUpdateSystem(topLevelGameModule));
            Add(new VehicleCmdExecuteManagerSystem(vehicleExecutionSelector, topLevelGameModule,
                vehicleCmdExecuteSystemHandler, simluationTimer, true, SharedConfig.ServerAuthorative));
            Add(new PhysicsPostUpdateSystem(topLevelGameModule));
#if (true)
//            Add(new UserCmdExecuteManagerSystem(topLevelGameModule, 
//                userCmdExecuteSystemHandler,
//                sessionObjects.GameStateProcessorFactory));
            Add(new UserCmdUpdateMsgExecuteManagerSystem(topLevelGameModule, userCmdExecuteSystemHandler,
                new SyncUpdateLatestMsgHandler()));
#else
            Add(new UserCmdParallelExecuteManagerSystem(topLevelGameModule,
                userCmdExecuteSystemHandler,
                sessionObjects.GameStateProcessorFactory, 4));
#endif

            Add(new UnityAssetManangerSystem(sessionObjects));
            Add(new ResourceLoadSystem(topLevelGameModule, sessionObjects.AssetManager));

            Add(new GamePlaySystem(topLevelGameModule));

            Add(new CommonLifeTimeSystem(sessionObjects.GameContexts));
            Add(new EntityCleanUpSystem(topLevelGameModule));
            Add(new CommonDestroySystem(sessionObjects));
            Add(new FreeGameRuleSystem(room));
            Add(new SendSnapshotSystem(room));
            Add(new CompensationSnapshotSystem(room));
           
        }
    }
}