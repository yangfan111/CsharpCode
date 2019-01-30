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

namespace App.Shared.GameModules.Camera
{
    class ServerPostCameraUpdateSystem : IUserCmdExecuteSystem, ISimpleParallelUserCmdExecuteSystem, IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerPostCameraUpdateSystem));

        private VehicleContext _vehicleContext;
        private FreeMoveContext _freeMoveContext;
        private PlayerContext _playerContext;
        private Contexts _context;
        private Motors _motors;
        DummyCameraMotorState _state;
        private readonly List<SubCameraMotorType> _subCameraMotorTypeArray = new List<SubCameraMotorType>();

        public ServerPostCameraUpdateSystem(Contexts context)
        {
            _context = context;
            _playerContext = context.player;
            _state = new DummyCameraMotorState(_motors);

        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = owner.OwnerEntity as PlayerEntity;

            if (!playerEntity.hasCameraStateNew) return;
            if (!playerEntity.hasCameraStateOutputNew) return;

            CopyClientOutputToComponent(playerEntity.cameraStateUpload, playerEntity.cameraFinalOutputNew);

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
            return new ServerPostCameraUpdateSystem(_context);
        }

        public void OnRender()
        {
            throw new NotImplementedException();
        }
    }
}
