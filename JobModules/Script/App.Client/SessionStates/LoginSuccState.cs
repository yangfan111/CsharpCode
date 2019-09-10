using App.Client.ClientSystems;
using App.Client.GameModules.GamePlay.Free.Entitas;
using App.Client.GameModules.Player;
using App.Client.GameModules.Terrain;
using App.Client.GameModules.UserInput;
using App.Client.SessionStates;
using App.Client.Tools;
using App.Shared;
using App.Shared.Components;
using App.Shared.GameModules;
using App.Shared.VechilePrediction;
using Core.GameModule.Module;
using Core.GameModule.Step;
using Core.GameModule.System;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SessionState;
using Core.SyncLatest;
using Core.Utils;
using Entitas;
using System;
using UnityEngine;

namespace App.Client.ClientGameModules
{

    public sealed class ClientUserCmdFeature : Feature
    {
        public ClientUserCmdFeature(string name, Contexts contexts) : base(name)
        {
            Add(new InputCollectSystem(contexts));
            Add(new MouseLockSystem(contexts));
            Add(new ClientFreeCmdGenerateSystem(contexts));
            Add(new UserCmdCollectSystem(contexts));
            Add(new PlayerInterceptCmdSystem(contexts));
            Add(new ClientPlayerPickAndDropSystem(contexts));
        }
    }

    public sealed class ClientProfileFeature : Feature
    {
        public ClientProfileFeature(string name, Contexts contexts) : base(name)
        {
            Add(new TerrainTestSystem(contexts));
            Add(new AutoTerrainNavigatorSystem(contexts));
            Add(new MinRendererSetSystem(contexts));
            Add(new WoodConflictSystem(contexts));
        }
    }
    /// <summary>
    ///
    /// 
    /// </summary>
    public class LoginSuccState : AbstractSessionState
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(LoginSuccState));

        private CompositeGameModule _gameModule;

       

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            if (Camera.main == null)
            {
                throw new Exception("Camera.main is null");
            }
            Contexts _contexts = (Contexts)contexts;
          
            _gameModule = GameModuleFactory.CreateCompositeGameModule(_contexts);
            var sessionObjects = _contexts.session.clientSessionObjects;

            SyncLastestManager netSyncManager = sessionObjects.NetSyncManager;
            PlaybackManager playbackManager = sessionObjects.PlaybackManager;
            var predictionManager = sessionObjects.UserPredictionManager;
            var predicatoinProvider = sessionObjects.UserPredictionProvider;
            ISimulationTimer simulationTimer = sessionObjects.SimulationTimer;

            var systems = new Feature("LoginSuccState");
            systems.Add(new DriveTimeSystem(_contexts));
            systems.Add(new PrepareSnapshotPairSystem(_contexts));
            systems.Add(new ClientUserCmdFeature("UserCmd", _contexts));
            systems.Add(new ClientProfileFeature("Profile", _contexts));
            if (SharedConfig.IsReplay)
            {
                systems.Add(new UserCmdReplaySystem(_contexts));
                systems.Add(new PrepareSnapshotPairSystem(_contexts));
                
            }
            systems.Add(new ClientMainFeature(
                "LoginSuccSystems",
                _gameModule,
                netSyncManager,
                playbackManager,
                predictionManager,
                predicatoinProvider,
                simulationTimer,
                sessionObjects.VehicleCmdExecuteSystemHandler,
                new ClientVehicleExecutionSelector(_contexts), 
                _contexts.session.commonSession));
            /*车辆命令走老流程*/
            systems.Add(new UserCmdSendSystem(_contexts).WithExecFrameStep(EEcecuteStep.CmdFrameStep));
            /*用户的命令*/
            systems.Add(new UserCmdUpdateSystem(_contexts).WithExecFrameStep(EEcecuteStep.CmdFrameStep));
            return systems;
        }

        public override Systems CreateOnDrawGizmos(IContexts contexts)
        {
            Systems system = new Systems();
            system.Add(new GizmosRenderSystem(_gameModule));
            return system;
        }

        public override Systems CreateLateUpdateSystems(IContexts contexts)
        {
            Systems system = new Systems();
            system.Add(new LateUpdateSystem(_gameModule));
            return system;
        }

        public override Systems CreateOnGuiSystems(IContexts contexts)
        {
            Systems system = new Systems();
            system.Add(new OnGuiSystem(_gameModule));
          
            return system;
        }

      


        public LoginSuccState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
        }

     
    }
}