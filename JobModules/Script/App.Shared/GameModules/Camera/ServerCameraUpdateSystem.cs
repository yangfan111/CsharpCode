using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Camera
{
    class ServerCameraUpdateSystem : IUserCmdExecuteSystem, ISimpleParallelUserCmdExecuteSystem, IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerCameraUpdateSystem));

        private VehicleContext _vehicleContext;
        private FreeMoveContext _freeMoveContext;
        private PlayerContext _playerContext;
        private Contexts _context;
        private Motors _motors;
        DummyCameraMotorState _state;
        private readonly List<SubCameraMotorType> _subCameraMotorTypeArray = new List<SubCameraMotorType>();

        public ServerCameraUpdateSystem(Contexts context, Motors m)
        {
            _context = context;
            _vehicleContext = context.vehicle;
            _freeMoveContext = context.freeMove;
            _playerContext = context.player;
            _motors = m;
            _state = new DummyCameraMotorState();

            foreach (SubCameraMotorType value in System.Enum.GetValues(typeof(SubCameraMotorType)))
            {
                _subCameraMotorTypeArray.Add(value);
            }
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;

            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;

            CopyClientStateToComponent(player.cameraStateUpload, player.cameraStateNew);

            HandleAction(player);
        }

        private void HandleAction(PlayerEntity player)
        {
            _motors.ActionManager.CopyActionCode(CameraActionType.Enter, player.cameraStateUpload.EnterActionCode);
            _motors.ActionManager.CopyActionCode(CameraActionType.Leave, player.cameraStateUpload.LeaveActionCode);
            _motors.ActionManager.OnAction(player, _state);
        }

        private void CopyClientStateToComponent(CameraStateUploadComponent input, CameraStateNewComponent output)
        {
            output.MainNowMode = input.MainNowMode;
            output.ViewNowMode = input.ViewNowMode;
            output.PeekNowMode = input.PeekNowMode;
            output.FreeNowMode = input.FreeNowMode;
            output.FreeYaw = input.FreeYaw;
            output.FreePitch = input.FreePitch;
            output.CanFire = input.CanFire;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new ServerCameraUpdateSystem(_context, _motors);
        }

        public void OnRender()
        {
        }
    }
}
