using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using XmlConfig;

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
            _state = new DummyCameraMotorState();
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = owner.OwnerEntity as PlayerEntity;

            if (!playerEntity.hasCameraStateNew) return;
            if (!playerEntity.hasCameraStateOutputNew) return;

            CopyClientOutputToComponent(playerEntity);
        }

        private void CopyClientOutputToComponent(PlayerEntity player)
        {
            var input = player.cameraStateUpload;
            var output = player.cameraFinalOutputNew;
            
            output.Position = player.GetOrigCameraPos(input.Position);
            output.EulerAngle = input.EulerAngle;
            output.Fov = input.Fov;
            output.Far = input.Far;
            output.Near = input.Near;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new ServerPostCameraUpdateSystem(_context);
        }

        public void OnRender()
        {

        }
    }
}
