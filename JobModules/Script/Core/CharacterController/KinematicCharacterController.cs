using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CameraControl;
using Core.Utils;
using ECM.Components;
using KinematicCharacterController;
using UnityEngine;
using Utils.Compare;

namespace Core.CharacterController
{
    public class KinematicCharacterController:ICharacterController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(KinematicCharacterController));
        protected static readonly float EPS = 0.001f;
        protected KinematicCharacterMotor _motor;
        public KinematicCharacterController(KinematicCharacterMotor motor)
        {
            _motor = motor;
        }

        public object RealValue
        {
            get { return _motor; }
        }

        public virtual void Rotate(Quaternion target, float deltaTime)
        {
            _motor.CharacterController.SetDeltaRot(new Vector3(0, YawPitchUtility.CalcDeltaAngle(_motor.transform.rotation.eulerAngles.y, target.eulerAngles.y), 0));
        }

        private float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float ret = Vector3.Angle(from, to);
            
            return ret * Mathf.Sign(Vector3.Dot(Vector3.Cross(from, to), axis));
        }

        public virtual void Move(Vector3 dist, float deltaTime = 0)
        {
            _motor.CharacterController.SetVec(dist / deltaTime);
            KinematicCharacterSystem.MySimulate(deltaTime, _motor);
        }

        public Transform transform
        {
            get { return _motor.transform; }
        }

        public GameObject gameObject
        {
            get { return _motor.gameObject; }
        }

        public float radius
        {
            get { return _motor.CapsuleRadius; }
        }

        public float height
        {
            get { return _motor.CapsuleHeight; }
        }

        public Vector3 center
        {
            get { return new Vector3(0f, _motor.CapsuleYOffset, 0f); }
        }

        public Vector3 direction
        {
            get
            {
                return PhysicsCastHelper.GetCapsuleForwardDirection(_motor.CapsuleDirection);
            }
        }

        public bool enabled
        {
            get { return _motor.EnableMotor; }
            set
            {
                _motor.EnableMotor = value;
                if (_motor.EnableMotor)
                {
                    _motor.SetPositionAndRotation(transform.position, transform.rotation);
//                    _motor.SetCharacterPosition(transform.position);
//                    _motor.SetCharacterRotation(transform.rotation);
                }
            }
        }

        public bool isGrounded
        {
            get { return _motor.GroundingStatus.IsOnGround; }
        }

        public float slopeLimit
        {
            get { return _motor.MaxStableSlopeAngle; }
        }

        public virtual void SetCharacterPosition(Vector3 targetPos)
        {
            if (!CompareUtility.IsApproximatelyEqual(_motor.TransientPosition, targetPos, EPS))
            {
                _motor.SetPosition(targetPos);
            }
            //Logger.InfoFormat("set target pos:{0}", targetPos);
        }

        public virtual void SetCharacterRotation(Quaternion rot)
        {
			//这边是需要传进来旋转之前的状态，现在是由相机输入的，已经加过新的旋转了
            if (!CompareUtility.IsApproximatelyEqual(_motor.TransientRotation.eulerAngles, rot.eulerAngles,EPS))
            {
                _motor.SetRotation(rot);
            }
        }

        public virtual void SetCharacterRotation(Vector3 euler)
        {
            if (!CompareUtility.IsApproximatelyEqual(YawPitchUtility.Normalize(_motor.TransientRotation.eulerAngles), euler,EPS))
            {
                _motor.SetRotation(Quaternion.Euler(euler));
            }
        }

        protected void DefaultInit()
        {
            _motor.CapsuleRadius = 0.4f;
            _motor.CapsuleHeight = 1.75f;
            _motor.CapsuleYOffset = 0.4f;
            _motor.CapsuleDirection = 1;
            _motor.GroundDetectionExtraDistance = 0;
            _motor.MaxStepHeight = 0.5f;
            _motor.MinRequiredStepDepth = 0.1f;
            _motor.MaxStableSlopeAngle = 45f;
            _motor.MaxStableDistanceFromLedge = 0.5f;
            _motor.PreventSnappingOnLedges = false;
            _motor.MaxStableDenivelationAngle = 180f;
            _motor.SimulatedMass = 0.2f;
            _motor.RigidbodyInteractionType = RigidbodyInteractionType.SimulatedDynamic;
            _motor.PreserveAttachedRigidbodyMomentum = true;
            _motor.HasPlanarConstraint = false;
            _motor.PlanarConstraintAxis = Vector3.forward;
            _motor.StepHandling = StepHandlingMethod.Extra;
            _motor.LedgeHandling = true;
            _motor.InteractiveRigidbodyHandling = false;
            _motor.SafeMovement = false;
            _motor.SafeRotate = false;
            _motor.FlyMode = false;
            _motor.FlyModePosYOffset = 0f;
            _motor.FlyModeAngleXMin = -35f;
            _motor.FlyModeAngleXMax = 89f;
        }

        public virtual void Init()
        {
            DefaultInit();
            _motor.OnValidate();
        }

        public CollisionFlags collisionFlags
        {
            get
            {
                if (isGrounded)
                {
                    return CollisionFlags.Below;
                }
                else
                {
                    return CollisionFlags.None;
                }
            }
        }

        public Vector3 GetLastGroundNormal()
        {
            return _motor.GroundingStatus.GroundNormal;
        }

        public Vector3 GetLastGroundHitPoint()
        {
            return _motor.GroundingStatus.GroundPoint;
        }

        public ControllerHitInfo GetCharacterControllerHitInfo(HitType type = HitType.Down)
        {
            return new ControllerHitInfo(_motor.GroundingStatus.GroundNormal, _motor.GroundingStatus.GroundPoint, _motor.GroundingStatus.GroundCollider, 0f, null ,_motor.GroundingStatus.IsOnGround);
        }

        public virtual KeyValuePair<float, float> GetRotateBound(Quaternion prevRot, Vector3 prevPos, int frameInterval)
        {
            return new KeyValuePair<float, float>(-180f,180f);
        }

        public GroundHit GetGroundHit
        {
            get
            {
                //throw new Exception("not implement!!!");
                return _motor.GroundingStatus.ToGroundHit();
            }
        }

        public Collider GetCollider()
        {
            return _motor.Capsule;
        }

        public virtual void DrawBoundingBox()
        {
            var characterTransformToCapsuleBottom = _motor.CharacterTransformToCapsuleBottom;
            var characterTransformToCapsuleTop = _motor.CharacterTransformToCapsuleTop;
            //DebugDraw.EditorDrawCapsule(transform.position + transform.rotation * characterTransformToCapsuleBottom, transform.position + transform.rotation * characterTransformToCapsuleTop, radius, Color.magenta);
            DebugDraw.DebugCapsule(transform.position + transform.rotation * characterTransformToCapsuleBottom, transform.position + transform.rotation * characterTransformToCapsuleTop, Color.magenta, radius);
        }

        public void DrawLastGroundHit()
        {
            //DebugDraw.EditorDrawArrow(GetLastGroundHitPoint(), GetLastGroundNormal(), Color.red);
            DebugDraw.DebugArrow(GetLastGroundHitPoint(), GetLastGroundNormal(), Color.red);
        }

        public void DrawGround()
        {
            
        }
    }
}
