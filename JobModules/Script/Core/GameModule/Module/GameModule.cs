using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;

namespace Core.GameModule.Module
{
    public class GameModule : IGameModule
    {
        private List<IEntityInitSystem> _entityInitSystems = new List<IEntityInitSystem>();
        private List<IModuleInitSystem> _clientModuleInitSystems = new List<IModuleInitSystem>();
        private List<IUserCmdExecuteSystem> _userCmdExecuteSystems = new List<IUserCmdExecuteSystem>();
        private List<IVehicleCmdExecuteSystem> _vehicleCmdExecuteSystems = new List<IVehicleCmdExecuteSystem>();
        private List<IGizmosRenderSystem> _gizmosRenderSystems = new List<IGizmosRenderSystem>();
        private List<IRenderSystem> _renderSystems = new List<IRenderSystem>();
        private List<IEntityCleanUpSystem> _entityCleanUpSystems = new List<IEntityCleanUpSystem>();

        private List<IGameStateUpdateSystem> _gameStateMonitorSystems = new List<IGameStateUpdateSystem>();

        private List<IPhysicsInitSystem> _physicsInitSystems = new List<IPhysicsInitSystem>();
        private List<IPhysicsUpdateSystem> _physicsUpdateSystems = new List<IPhysicsUpdateSystem>();
        private List<IPhysicsPostUpdateSystem> _physicsPostUpdateSystems = new List<IPhysicsPostUpdateSystem>();
    
        private List<IResourceLoadSystem> _resourceLoadSystems = new List<IResourceLoadSystem>();
        private List<IPlaybackSystem> _playbackSystems = new List<IPlaybackSystem>();
        private List<ILateUpdateSystem> _lateUpdateSystems = new List<ILateUpdateSystem>();
        private List<IOnGuiSystem> _onGuiSystems = new List<IOnGuiSystem>();
        private List<IGamePlaySystem> _gamePlaySystems = new List<IGamePlaySystem>();
        private List<IUiSystem> _uiSystems = new List<IUiSystem>();
        private List<IUiHfrSystem> _hfrUiSystems = new List<IUiHfrSystem>();
        private List<IBeforeUserCmdExecuteSystem> _beforeUserCmdExecuteSystem = new List<IBeforeUserCmdExecuteSystem>();

        public virtual void Init()
        {
            
        }

        public void AddSystem(object system)
        {
            if (system is IEntityInitSystem) _entityInitSystems.Add(system as IEntityInitSystem);
            if (system is IModuleInitSystem) _clientModuleInitSystems.Add(system as IModuleInitSystem);
            if (system is IUserCmdExecuteSystem) _userCmdExecuteSystems.Add(system as IUserCmdExecuteSystem);
            if (system is IVehicleCmdExecuteSystem) _vehicleCmdExecuteSystems.Add(system as IVehicleCmdExecuteSystem);
            if (system is IGizmosRenderSystem) _gizmosRenderSystems.Add(system as IGizmosRenderSystem);
            if (system is IRenderSystem) _renderSystems.Add(system as IRenderSystem);
            if (system is IEntityCleanUpSystem) _entityCleanUpSystems.Add(system as IEntityCleanUpSystem);
            if (system is IGameStateUpdateSystem) _gameStateMonitorSystems.Add(system as IGameStateUpdateSystem);
            if (system is IPhysicsInitSystem) _physicsInitSystems.Add(system as IPhysicsInitSystem);
            if (system is IPhysicsUpdateSystem) _physicsUpdateSystems.Add(system as IPhysicsUpdateSystem);
            if (system is IPhysicsPostUpdateSystem) _physicsPostUpdateSystems.Add(system as IPhysicsPostUpdateSystem);
            if (system is IResourceLoadSystem) _resourceLoadSystems.Add(system as IResourceLoadSystem);
            if (system is IPlaybackSystem) _playbackSystems.Add(system as IPlaybackSystem);
            if (system is ILateUpdateSystem) _lateUpdateSystems.Add(system as ILateUpdateSystem);
            if (system is IOnGuiSystem) _onGuiSystems.Add(system as IOnGuiSystem);
            if (system is IGamePlaySystem) _gamePlaySystems.Add(system as IGamePlaySystem);
            if (system is IUiSystem) _uiSystems.Add(system as IUiSystem);
            if (system is IUiHfrSystem) _hfrUiSystems.Add(system as IUiHfrSystem);
            if(system is IBeforeUserCmdExecuteSystem) _beforeUserCmdExecuteSystem.Add(system as IBeforeUserCmdExecuteSystem);
        }

        public List<IEntityInitSystem> EntityInitSystems
        {
            get { return _entityInitSystems; }
        }

        public List<IModuleInitSystem> ModuleInitSystems
        {
            get { return _clientModuleInitSystems; }
        }

  
        public List<IPlaybackSystem> PlaybackSystems
        {
            get { return _playbackSystems; }
        }

        public List<IUserCmdExecuteSystem> UserCmdExecuteSystems
        {
            get { return _userCmdExecuteSystems; }
        }

        public List<IVehicleCmdExecuteSystem> VehicleCmdExecuteSystems
        {
            get { return _vehicleCmdExecuteSystems; }
        }

        public List<IGameStateUpdateSystem> GameStateUpdateSystems
        {
            get { return _gameStateMonitorSystems; }
        }

        public List<IPhysicsInitSystem> PhysicsInitSystems
        {
            get { return _physicsInitSystems; }
        }

        public List<IPhysicsUpdateSystem> PhysicsUpdateSystems
        {
            get { return _physicsUpdateSystems; }
        }

        public List<IPhysicsPostUpdateSystem> PhysicsPostUpdateSystems
        {
            get { return _physicsPostUpdateSystems; }
        }

        public List<IGizmosRenderSystem> GizmosRenderSystems
        {
            get { return _gizmosRenderSystems; }
        }

        public List<IRenderSystem> RenderSystems
        {
            get { return _renderSystems; }
        }

        public List<IEntityCleanUpSystem> EntityCleanUpSystems
        {
            get { return _entityCleanUpSystems; }
        }

        public List<ILateUpdateSystem> LateUpdateSystems
        {
            get { return _lateUpdateSystems; }
        }
        public List<IOnGuiSystem> OnGUISystems
        {
            get { return _onGuiSystems; }
        }

        public List<IUiSystem> UiSystems
        {
            get { return _uiSystems; }
        } 
        
        public List<IUiHfrSystem> UiHfrSystems
        {
            get { return _hfrUiSystems; }
        } 

        public List<IGamePlaySystem> GamePlaySystems
        {
            get { return _gamePlaySystems; }
        }

        public List<IBeforeUserCmdExecuteSystem> BeforeUserCmdExecuteSystems
        {
            get { return _beforeUserCmdExecuteSystem; }
         }


        public List<IResourceLoadSystem> ResourceLoadSystems { get { return _resourceLoadSystems; } }

    }
}