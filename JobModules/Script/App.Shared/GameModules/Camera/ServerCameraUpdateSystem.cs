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
            _state = new DummyCameraMotorState(_motors);

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
            player.cameraArchor.ArchorType = (ECameraArchorType)player.cameraStateUpload.ArchorType;

            CameraActionManager.CopyActionCode(CameraActionType.Enter, player.cameraStateUpload.EnterActionCode);
            CameraActionManager.CopyActionCode(CameraActionType.Leave, player.cameraStateUpload.LeaveActionCode);
            CameraActionManager.OnAction(player,_state);
        }

        private void CopyClientStateToComponent(CameraStateUploadComponent input, CameraStateNewComponent output)
        {
            output.MainNowMode = input.MainNowMode;
            output.MainLastMode = input.MainLastMode;
            output.MainModeTime = input.MainModeTime;
            output.ViewNowMode = input.ViewNowMode;
            output.ViewLastMode = input.ViewLastMode;
            output.ViewModeTime = input.ViewModeTime;
            output.PeekNowMode = input.PeekNowMode;
            output.PeekLastMode = input.PeekLastMode;
            output.PeekModeTime = input.PeekModeTime;
            output.FreeNowMode = input.FreeNowMode;
            output.FreeLastMode = input.FreeLastMode;
            output.FreeModeTime = input.FreeModeTime;
            output.FreeYaw = input.FreeYaw;
            output.FreePitch = input.FreePitch;
            output.LastFreeYaw = input.LastFreeYaw;
            output.LastFreePitch = input.LastFreePitch;
            output.LastPeekPercent = input.LastPeekPercent;
            output.CanFire = input.CanFire;
        }

        private void CopyClientOutputToComponent(CameraStateUploadComponent input,
             CameraFinalOutputNewComponent output)
        {
            output.Position = input.Position;
            output.EulerAngle = input.EulerAngle;
            output.Fov = input.Fov;
            output.Far = input.Far;
            output.Near = input.Near;
            output.PlayerFocusPosition = input.PlayerFocusPosition;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new ServerCameraUpdateSystem(_context, _motors);
        }

        public void OnRender()
        {
            throw new NotImplementedException();
        }
    }
}
