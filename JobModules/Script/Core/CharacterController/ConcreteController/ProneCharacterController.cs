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
        
        public ProneCharacterController(KinematicCharacterMotor motor, BaseCharacterController controller) : base(motor)
        {
            _controller = controller;
        }

        public override void Init()
        {
            _motor.ChangeCharacterController(_controller);
            DefaultInit();
            _motor.CapsuleDirection = 2;
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