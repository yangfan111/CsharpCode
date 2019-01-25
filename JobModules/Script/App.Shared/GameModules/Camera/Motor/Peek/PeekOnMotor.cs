using System.Collections.Generic;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Motor.Peek
{
    class PeekOnMotor : AbstractCameraMotor
    {
        private short _modeId;

        private readonly bool _isRight;

        private PeekCameraConfig _config;

        public PeekOnMotor(bool isright, PeekCameraConfig config)
        {
            _isRight = isright;
            _modeId = (short) (_isRight ? ECameraPeekMode.Right : ECameraPeekMode.Left);
            _config = config;
            _finalPostOffset = _isRight ? config.ThirdOffset : -config.ThirdOffset;
        }

        public override short ModeId
        {
            get { return _modeId; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            if (state.GetMainMotor().NowMode == (short) ECameraPoseMode.AirPlane) return false;
            return _isRight
                ? input.LeanState == LeanInConfig.PeekRight && state.LastPeekPercent >= 0
                : input.LeanState == LeanInConfig.PeekLeft && state.LastPeekPercent <= 0;
        }

        public override Vector3 FinalPostOffset
        {
            get { return _finalPostOffset; }
        }


        public override int Order
        {
            get { return 1; }
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PeekOnMotor));

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            if (state.ViewMode.Equals(ECameraViewMode.ThirdPerson))
            {
                _finalPostOffset = (_isRight ? 1 : -1) * _config.ThirdOffset;
            }
            else if (state.ViewMode.Equals(ECameraViewMode.FirstPerson))
            {
                _finalPostOffset = (_isRight ? 1 : -1) * _config.FirstOffset;
            }
            else if (state.ViewMode.Equals(ECameraViewMode.GunSight))
            {
                _finalPostOffset = (_isRight ? 1 : -1) * _config.GunSightOffset;
            }

            var elapsedPercent = ElapsedPercent(clientTime, subState.ModeTime, _config.TransitionTime);
            output.PostOffset = Vector3.Lerp(Vector3.zero, FinalPostOffset, elapsedPercent);
            state.LastPeekPercent = (_isRight ? 1 : -1) * elapsedPercent;
            if (elapsedPercent < 1)
            {
                _logger.DebugFormat("output.PostOffset:{0}", output.PostOffset);
            }
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
        }


        readonly HashSet<short> _rightExlcudes = new HashSet<short>() {(short) ECameraPeekMode.Left};
        readonly HashSet<short> _leftExlcudes = new HashSet<short>() {(short) ECameraPeekMode.Right};

        private Vector3 _finalPostOffset;


        public override HashSet<short> ExcludeNextMotor()
        {
            return _isRight ? _rightExlcudes : _leftExlcudes;
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
        }
    }
}