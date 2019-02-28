using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.ClientSession;
using App.Shared.GameModules.Camera;
using App.Shared.GameModules.Camera.Utils;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.SessionState;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera
{
    public class ClientCameraPreUpdateSystem: AbstractStepExecuteSystem
    {
     
        private PlayerContext _playerContext;
        private Motors _motors = new Motors();
        DummyCameraMotorInput _input = new DummyCameraMotorInput();
        private DummyCameraMotorState _state ;
        private VehicleContext _vehicleContext;
        private Contexts _contexts;

        private FreeMoveContext _freeMoveContext;
        private readonly  Array _subCameraMotorTypeArray;
         
        public ClientCameraPreUpdateSystem(Contexts contexts, Motors m)
        {
            _vehicleContext = contexts.vehicle;
            _freeMoveContext = contexts.freeMove;
            _motors = m;
            _playerContext = contexts.player;
            _contexts = contexts;
            _state = new DummyCameraMotorState(m);
            _subCameraMotorTypeArray = Enum.GetValues(typeof(SubCameraMotorType));
        }

        private void PreProcessInput(PlayerEntity player, DummyCameraMotorInput input,
            Dictionary<int, ICameraNewMotor> dict, SubCameraMotorState subState, DummyCameraMotorState state)
        {
            if (!dict.ContainsKey(subState.NowMode)) return;
            if (!dict.ContainsKey(subState.LastMode)) return;
            var oldMotor = dict[subState.LastMode];
            var nowMotor = dict[subState.NowMode];
            nowMotor.PreProcessInput(player, input, state);
        }

      

        protected override void InternalExecute()
        {
          
            
            var player = _playerContext.flagSelfEntity;
            if (player == null) return;
            if (!player.hasFirstPersonModel) return;
            if (!player.hasThirdPersonModel) return;
            if (!player.hasUserCmd) return;
            if (player.userCmd.UserCmdStepList.Count ==0) return;
            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;
            
            var cmd = (UserCmd) player.userCmd.UserCmdStepList.Last();
           
            DummyCameraMotorState.Convert(player.cameraStateNew, _state);
            var archotRotation = player.cameraArchor.ArchorEulerAngle;
            _input.Generate(_contexts, player, cmd, archotRotation.y,archotRotation.x);
           
            foreach (int i in _subCameraMotorTypeArray)
            {
                var type = (SubCameraMotorType) i;
                PreProcessInput(player, _input, _motors.GetDict(type), _state.Get(type), _state);
            }
            DummyCameraMotorState.Convert(_state, player.cameraStateNew);
        }
    }
}
