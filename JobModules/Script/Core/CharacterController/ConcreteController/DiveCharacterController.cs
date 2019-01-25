using Core.CameraControl;
using Core.Utils;
using KinematicCharacterController;
using UnityEngine;
using Utils.Compare;

namespace Core.CharacterController.ConcreteController
{
    public class DiveCharacterController:KinematicCharacterController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DiveCharacterController));
        private BaseCharacterController _controller;
        
        public static readonly float DiveFowrard = 90f;
        public static readonly float DiveBack = -35f;
        //dive z offset
        public static readonly float DiveLeft = 0f;
        public static readonly float DiveRight = 0f;
        //
        public static readonly float DiveUp = 0f;
        public static readonly float DiveDown = 160f;
        public static readonly float FlyModePosYOffset = 0.5f;
        
        public DiveCharacterController(KinematicCharacterMotor motor, BaseCharacterController controller) : base(motor)
        {
            _controller = controller;
        }

        public override void Init()
        {
            _motor.ChangeCharacterController(_controller);
            DefaultInit();
            _motor.CapsuleDirection = 1;
            _motor.CapsuleYOffset = _motor.CapsuleHeight * 0.5f;
            _motor.FlyMode = true;
            _motor.FlyModePosYOffset = FlyModePosYOffset;
            _motor.FlyModeAngleXMin = DiveBack;
            _motor.FlyModeAngleXMax = DiveFowrard;
            _motor.OnValidate();
        }

        public override void DrawBoundingBox()
        {
            KinematicCharacterSystem.DebugShowFlyBoundingBox = true;
        }

        public override void SetCharacterPosition(Vector3 targetPos)
        {
            if (!CompareUtility.IsApproximatelyEqual(_motor.TransientPosition, targetPos, EPS))
            {
                //Logger.InfoFormat("rewind Pos from:{0} to:{1}",_motor.Transform.position ,targetPos);
                _motor.SetFlyPosition(targetPos);
            }
            
        }

        public override void SetCharacterRotation(Quaternion rot)
        {
            var newAngle = rot.eulerAngles;
            _motor.SetFlyRotation(Quaternion.Euler(0, newAngle.y, 0), newAngle.x, 0);
        }

        public override void Move(Vector3 dist, float deltaTime = 0)
        {
            _motor.CharacterController.SetVec(dist / deltaTime);
            KinematicCharacterSystem.FlySimulate(deltaTime, _motor);
        }

        public override void SetCharacterRotation(Vector3 euler)
        {
            _motor.SetFlyRotation(Quaternion.Euler(0, euler.y, 0), euler.x, euler.z);
        }
    }
}