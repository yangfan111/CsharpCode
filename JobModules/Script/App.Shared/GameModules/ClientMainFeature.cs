using App.Client.StartUp;
using App.Shared.GameModules.Common;
using Assets.App.Shared.GameModules.Camera;
using Core;

using Core.EntityComponent;
using Core.GameModule.Common;
using Core.GameModule.Module;
using Core.GameModule.Step;
using Core.GameModule.System;

using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SyncLatest;

namespace App.Shared.GameModules
{


    public sealed class ClientMainFeature : Feature
    {
        
        public ClientMainFeature(string name,
            IGameModule topLevelGameModule,
            SyncLastestManager netSyncManager,
            IPlaybackManager playbackManager,
            PredictionManager userPredictionManager,
            AbstractPredictionProvider predicatoinProvider,
            ISimulationTimer simulationTimer,
            IVehicleCmdExecuteSystemHandler vehicleCmdExecuteSystemHandler,
            IVehicleExecutionSelector vehicleExecutionSelector,
            ICommonSessionObjects commonSessionObjects) : base(name)
        {
        
            topLevelGameModule.Init();
            
            Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            Add(new EntityCreateSystem(topLevelGameModule)); 



            Add(new SyncLatestSystem(netSyncManager));
            if(!SharedConfig.IsOffline)
                Add(new PlaybackInitSystem(playbackManager));
            Add(new PlaybackSystem(topLevelGameModule));

            //添加游戏状态更新处理
            Add(new GameStateUpdateSystem(topLevelGameModule));

            // 需要在playback之后，因为要根据车的位置更新人的位置
            // 要在predicte之前，因为要根据车的位置，更像摄像机位置
            Add(new PhysicsInitSystem(topLevelGameModule));
            Add(new PhysicsUpdateSystem(topLevelGameModule));

            
            Add(new VehicleCmdExecuteManagerSystem(vehicleExecutionSelector, topLevelGameModule, vehicleCmdExecuteSystemHandler, simulationTimer,false, SharedConfig.ServerAuthorative));
          
            
            Add(new PhysicsPostUpdateSystem(topLevelGameModule));
            Add(new PredictionInitSystem(userPredictionManager));           
            Add(new UserPredictionSystem(topLevelGameModule, 
                predicatoinProvider, 
                userPredictionManager));
            
          
            Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));

            Add(new GamePlaySystem(topLevelGameModule));

            Add(new RenderSystem(topLevelGameModule));
            Add(new UiSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.UIFrameStep));
            Add(new UiHfrSystem(topLevelGameModule));

            Add(new CommonLifeTimeSystem(commonSessionObjects.GameContexts));
            Add(new CommoTickImmutabblitySystem(commonSessionObjects.GameContexts));
            Add(new EntityCleanUpSystem(topLevelGameModule));
          
            Add(new CommonDestroySystem(commonSessionObjects));


        }
    }
}