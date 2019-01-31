using App.Shared.GameModules.Common;
using Assets.App.Shared.GameModules.Camera;
using Core;
using Core.AssetManager;
using Core.EntitasAdpater;
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
            ISyncLatestManager syncLatestManager,
            IPlaybackManager playbackManager,
            IPredictionInitManager userPredictionInitManager,
            IUserPredictionInfoProvider predicatoinInfoProvider,
            ISimulationTimer simulationTimer,
            IVehicleCmdExecuteSystemHandler vehicleCmdExecuteSystemHandler,
            IVehicleExecutionSelector vehicleExecutionSelector,
            ICommonSessionObjects commonSessionObjects) : base(name)
        {
        
            topLevelGameModule.Init();
            
            Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            Add(new EntityCreateSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep)); 



            Add(new SyncLatestSystem(syncLatestManager).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            if(!SharedConfig.IsOffline)
                Add(new PlaybackInitSystem(playbackManager).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new PlaybackSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep));

            //添加游戏状态更新处理
            Add(new GameStateUpdateSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep));

            // 需要在playback之后，因为要根据车的位置更新人的位置
            // 要在predicte之前，因为要根据车的位置，更像摄像机位置
            Add(new PhysicsInitSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.CmdFrameStep));
            Add(new PhysicsUpdateSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.CmdFrameStep));

            
            Add(new VehicleCmdExecuteManagerSystem(vehicleExecutionSelector, topLevelGameModule, vehicleCmdExecuteSystemHandler, simulationTimer,false, SharedConfig.ServerAuthorative).WithExecFrameStep(EEcecuteStep.CmdFrameStep));
            Add(new UserPrePredictionSystem(topLevelGameModule, 
                predicatoinInfoProvider, 
                userPredictionInitManager, 
                commonSessionObjects.GameStateProcessorFactory).WithExecFrameStep(EEcecuteStep.NormalFrameStep)); //每帧执行的cmd要放在回滚之前，不然会导致多执行帧
            Add(new PhysicsPostUpdateSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.CmdFrameStep));
            Add(new PredictionInitSystem(userPredictionInitManager).WithExecFrameStep(EEcecuteStep.CmdFrameStep));           
            Add(new UserPredictionSystem(topLevelGameModule, 
                predicatoinInfoProvider, 
                userPredictionInitManager, 
                commonSessionObjects.GameStateProcessorFactory).WithExecFrameStep(EEcecuteStep.CmdFrameStep));
            
            Add(new UnityAssetManangerSystem(commonSessionObjects).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager).WithExecFrameStep(EEcecuteStep.NormalFrameStep));

            Add(new GamePlaySystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.CmdFrameStep));

            Add(new RenderSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new UiSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.UIFrameStep));
            Add(new UiHfrSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep));

            Add(new CommonLifeTimeSystem(commonSessionObjects.GameContexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new CommoTickImmutabblitySystem(commonSessionObjects.GameContexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new EntityCleanUpSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
          
            Add(new CommonDestroySystem(commonSessionObjects).WithExecFrameStep(EEcecuteStep.NormalFrameStep));


        }
    }
}