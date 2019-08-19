using System.Linq;
using App.Client.Cmd;
using App.Client.GameModules.Vehicle;
using App.Protobuf;
using App.Server;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.GameModules.Vehicle;
using App.Shared.Network;
using App.Shared.Player;
using Core.CharacterState;
using Core.GameTime;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using XmlConfig;
using Vector3 = UnityEngine.Vector3;

namespace App.Client.ClientSystems
{
    public class UserCmdCollectSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UserCmdCollectSystem));
        private IUserCmdGenerator _generator;
        private IVehicleCmdGenerator _vehicleCmdGenerator;
  

        private ITimeManager _timeManager;
        private ClientSessionObjectsComponent _sessionObjects;
        private PlayerContext _playerContext;
        private ISimulationTimer _simulationTimer;

        private Contexts _contexts;
        private bool IsSwitchAutoRun = false;

        public UserCmdCollectSystem(Contexts contexts)
        {
            _sessionObjects = contexts.session.clientSessionObjects;
            
            _timeManager = _sessionObjects.TimeManager;
            _generator = _sessionObjects.UserCmdGenerator;
            _vehicleCmdGenerator = new UnityVehicleCmdGenerator(contexts.userInput.userInputManager.Mgr);
            _playerContext = contexts.player;

            _simulationTimer = _sessionObjects.SimulationTimer;

            _contexts = contexts;
        }

        protected override void InternalExecute()
        {
        
            //if (_elapse < _interval)
            //{
            //    _elapse++;
            //    return;
            //}

            //_elapse = 0;
         
            
            var player = _playerContext.flagSelfEntity;
            if (player != null && player.hasFirstPersonModel && player.hasThirdPersonModel && player.hasUserCmd)
            {
                CreateUserCmd(player);

                CreateVehicleCmd(player);
            }
            

        }

        private void CreateVehicleCmd(PlayerEntity player)
        {
            var controlledVehicle = PlayerVehicleUtility.GetControlledVehicle(player, _contexts.vehicle);
            if (controlledVehicle != null)
            {
                var vehicleCmd = VehicleEntityUtility.CreateVehicleCmd(_vehicleCmdGenerator, _contexts.vehicle, player,
                    _simulationTimer.CurrentTime);

                if (vehicleCmd != null)
                {
                    controlledVehicle.vehicleCmd.AddLatest(vehicleCmd);

                    
                }
            }
        }

        class DummyPlayerEntityUserCmdOwnApapter : IUserCmdOwnAdapter
        {
            private PlayerEntity playerEntity;

            public DummyPlayerEntityUserCmdOwnApapter(PlayerEntity playerEntity)
            {
                this.playerEntity = playerEntity;
            }

            public float Yaw
            {
                get { return playerEntity.orientation.Yaw; }
            }

            public float Pitch   {
                get { return playerEntity.orientation.Pitch; }
            }
            public Vector3 Position{
                get { return playerEntity.position.Value; }
            }

            public Transform PlayerTransform
            {
                get { return playerEntity.RootGo().transform; }
            }
        }
        private void CreateUserCmd(PlayerEntity player)
        {
            UserCmd cmd = _generator.GenerateUserCmd(new DummyPlayerEntityUserCmdOwnApapter(player), _timeManager.FrameInterval);
            cmd.RenderTime = _timeManager.RenderTime;
            cmd.ClientTime = _timeManager.ClientTime;
            if (_sessionObjects.SnapshotSelctor.LatestSnapshot != null)
            {
                cmd.SnapshotId = _sessionObjects.SnapshotSelctor.LatestSnapshot.SnapshotSeq;
            }
            else
            {
                cmd.SnapshotId = 0;
            }

            _generator.SetLastUserCmd(cmd);
            player.userCmd.AddLatest(cmd);

            
        }
    }
}