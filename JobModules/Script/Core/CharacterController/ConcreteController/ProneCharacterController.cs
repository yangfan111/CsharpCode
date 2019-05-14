using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

namespace Core.CharacterController.ConcreteController
{
    public class ProneCharacterController:KinematicCharacterController
    {
        public override void Rotate(Quaternion target, float deltaTime)
        {
            base.Rotate(target, deltaTime);
        }

        private BaseCharacterController _controller;
        private Action<Transform> _initAction;
        
        public ProneCharacterController(KinematicCharacterMotor motor, BaseCharacterController controller, Action<Transform> initAction = null) : base(motor)
        {
            _controller = controller;
            _initAction = initAction;
        }

        public override void Init()
        {
            _motor.ChangeCharacterController(_controller);
            if (_initAction != null)
            {
                _initAction.Invoke(_motor.transform);
            }
            DefaultInit();
            _motor.CapsuleDirection = 2;
            _motor.UseSphereGroundDetection = false;
            _motor.SafeMovement = false;
            _motor.OnValidate();
        }

        public override KeyValuePair<float, float> GetRotateBound(Quaternion prevRot, Vector3 prevPos, int frameInterval)
        {
            SetCharacterRotation(prevRot);
            SetCharacterPosition(prevPos);
            return KinematicCharacterSystem.MyCalcRotateBound(_motor,frameInterval);
        }
    }
}