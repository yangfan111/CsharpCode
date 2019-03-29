using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECM.Components;
using UnityEngine;
using Utils.Appearance;
using Utils.Compare;
using XmlConfig;

namespace Core.CharacterController
{
    public class 
        CharacterControllerContext: ICharacterControllerContext
    {

        private ICharacterController[] _controllers;

        private ICharacterController _currentState;
        private CharacterControllerType _currentType;

        public CharacterControllerContext(ICharacterController unityControllerState,
            ICharacterController proneKinematicControllerState,
            ICharacterController diveKinematicControllerState,
            ICharacterController swimKinematicControllerState)
        {
            _controllers = new ICharacterController[(int)CharacterControllerType.End];
            _controllers[(int) CharacterControllerType.UnityCharacterController] = unityControllerState;
            _controllers[(int) CharacterControllerType.ProneKinematicCharacterController] = proneKinematicControllerState;
            _controllers[(int) CharacterControllerType.DiveKinematicCharacterController] = diveKinematicControllerState;
            _controllers[(int) CharacterControllerType.SwimKinematicCharacterController] = swimKinematicControllerState;
            
            DisableAll();
            SetCurrentControllerType(CharacterControllerType.UnityCharacterController);
        }

        private void DisableAll()
        {
            foreach (ICharacterController characterController in _controllers)
            {
                characterController.enabled = false;
            }

            _currentType = CharacterControllerType.End;
        }


        public object RealValue
        {
            get { return _currentState.RealValue; }
        }

        public void Rotate(Quaternion target, float deltaTime)
        {
            _currentState.Rotate(target,deltaTime);
        }

        public void Move(Vector3 dist, float deltaTime = 0)
        {
            _currentState.Move(dist, deltaTime);
        }

        public Transform transform
        {
            get { return _currentState.transform; }
        }
        public GameObject gameObject
        {
            get { return _currentState.gameObject; }
        }

        public float radius
        {
            get { return _currentState.radius; }
        }

        public float height
        {
            get { return _currentState.height; }
        }
        public Vector3 center
        {
            get { return _currentState.center; }
        }

        public Vector3 direction
        {
            get { return _currentState.direction; }
        }

        public bool enabled
        {
            get { return _currentState.enabled; }
            set { _currentState.enabled = value; }
        }

        public bool isGrounded
        {
            get { return _currentState.isGrounded; }
        }

        public float slopeLimit
        {
            get { return _currentState.slopeLimit; }
        }

        public void SetCharacterPosition(Vector3 targetPos)
        {
            _currentState.SetCharacterPosition(targetPos);
            
        }

        public void SetCharacterRotation(Quaternion rot)
        {
            _currentState.SetCharacterRotation(rot);
        }

        public void SetCharacterRotation(Vector3 euler)
        {
            _currentState.SetCharacterRotation(euler);
        }

        public void Init()
        {
            _currentState.Init();
        }

        public CollisionFlags collisionFlags
        {
            get { return _currentState.collisionFlags; }
        }

        public ControllerHitInfo GetCharacterControllerHitInfo(HitType type = HitType.Down)
        {
            return _currentState.GetCharacterControllerHitInfo(type);
        }


        public KeyValuePair<float, float> GetRotateBound(Quaternion prevRot, Vector3 prevPos, int frameInterval)
        {
            return _currentState.GetRotateBound(prevRot, prevPos, frameInterval);
        }

        public GroundHit GetGroundHit
        {
            get { return _currentState.GetGroundHit; }
        }

        public Collider GetCollider()
        {
            return _currentState.GetCollider();
        }

        public void DrawBoundingBox()
        {
            _currentState.DrawBoundingBox();
        }

        public void DrawLastGroundHit()
        {
            _currentState.DrawLastGroundHit();
        }

        public void DrawGround()
        {
            _currentState.DrawGround();
        }

        public CharacterControllerType controllerType { get; set; }

        public CharacterControllerType GetCurrentControllerType()
        {
            return _currentType;
        }

        public void SetCurrentControllerType(CharacterControllerType type)
        {
            if (_currentState != null)
            {
                if (_currentType == type)
                {
                    return;
                }
                _currentState.enabled = false;
            }

            _currentState = _controllers[(int) type];
            _currentState.Init();
            _currentState.enabled = true;
            _currentType = type;
        }

        public void SetCurrentControllerType(PostureInConfig type)
        {
            if (type == PostureInConfig.Prone || type == PostureInConfig.ProneTransit)
            {
                SetCurrentControllerType(CharacterControllerType.ProneKinematicCharacterController);
            }
            else if (type == PostureInConfig.Dive)
            {
                SetCurrentControllerType(CharacterControllerType.DiveKinematicCharacterController);
            }
            else if (type == PostureInConfig.Swim)
            {
                SetCurrentControllerType(CharacterControllerType.SwimKinematicCharacterController);
            }
            else
            {
                SetCurrentControllerType(CharacterControllerType.UnityCharacterController);
            }
        }
    }
}
