using System;
using App.Client.ClientSystems;
using App.Client.GameModules.GamePlay;
using App.Client.GameModules.GamePlay.Free.Entitas;
using App.Client.GameModules.UserInput;
using App.Client.GameModules.Player;
using App.Client.GameModules.SceneObject;
using App.Client.GameModules.Sound;
using App.Client.GameModules.Terrain;
using App.Client.GameModules.Ui;
using App.Client.GameModules.Vehicle;
using App.Server;
using App.Shared;
using App.Shared.Components.ClientSession;
using App.Shared.GameModules;
using App.Shared.GameModules.Bullet;
using App.Shared.VechilePrediction;
using Assets.Sources.Free.UI;
using Core.Compensation;
using Core.Free;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SessionState;
using Core.SyncLatest;
using Entitas;
using Free.framework;
using App.Client.GameModules.Bullet;
using App.Client.GameModules.Player.Robot;
using App.Shared.Configuration;
using XmlConfig;
using App.Shared.GameModules.Attack;
using Core.Utils;
using UnityEngine;
using App.Client.GameModules.Throwing;
using App.Shared.GameModules.Configuration;
using App.Shared.GameModules.Throwing;
using App.Client.GameModules.ClientEffect;
using Assets.App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Camera.Utils;
using Assets.Sources.Free;
using Core.Configuration;
using Core.GameModule.Step;
using App.Client.Battle;
using App.Client.GameModules.GamePlay.Free.Scene;
using App.Client.GameModules.Ui.System;
using App.Client.SessionStates;
using App.Client.Tools;
using App.Shared.Components;
using Utils.Configuration;

namespace App.Client.ClientGameModules
{
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

            ISyncLatestManager syncLatestManager = sessionObjects.SyncLatestManager;
            IPlaybackManager playbackManager = sessionObjects.PlaybackManager;
            IPredictionInitManager predictionInitManager = sessionObjects.UserPredictionInitManager;
            
            IUserPredictionInfoProvider predicatoinInfoProvider = sessionObjects.UserPredictionInfoProvider;
            ISimulationTimer simulationTimer = sessionObjects.SimulationTimer;

            var systems = new Feature("LoginSuccState");
           
            systems.Add(new InputCollectSystem(_contexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            systems.Add(new MouseLockSystem(_contexts));
            systems.Add(new DriveTimeSystem(_contexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            systems.Add(new ClientFreeCmdGenerateSystem(_contexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            systems.Add(new UserCmdCollectSystem(_contexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            //  systems.Add(new ClientCameraPreUpdateSystem(_contexts.vehicle, _contexts.freeMove,_contexts.player, motors).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            systems.Add(new PlayerInterceptCmdSystem(_contexts).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            systems.Add(new UserCmdMergeSystem(_contexts).WithExecFrameStep(EEcecuteStep.CmdFrameStep));

            //Test
            systems.Add(new TerrainTestSystem(_contexts));
            //////
            systems.Add(new AutoTerrainNavigatorSystem(_contexts));
            systems.Add(new MinRendererSetSystem(_contexts));
            systems.Add(new WoodConflictSystem(_contexts));
            systems.Add(new ClientMainFeature(
                "LoginSuccSystems",
                _gameModule,
                syncLatestManager,
                playbackManager,
                predictionInitManager,
                predicatoinInfoProvider,
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