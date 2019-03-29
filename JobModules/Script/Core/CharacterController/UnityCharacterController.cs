using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using ECM.Components;
using UnityEngine;

namespace Core.CharacterController
{
    public class UnityCharacterController:ICharacterController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UnityCharacterController));
        protected static readonly float CastDistance = 0.05f;
        protected UnityEngine.CharacterController _controller;
        private BaseGroundDetection _groundDetection;
        private float _referenceCastDistance;
        private bool _slideOnSteepSlope = true;
        /// <summary>
        /// Is the character sliding off a steep slope?
        /// </summary>

        public bool isSliding { get; private set; }

        public UnityCharacterController(UnityEngine.CharacterController controller)
        {
            _controller = controller;
            InitGroundDetection();
        }

        private void InitGroundDetection()
        {
            var objAdapter = new CharacterControllerAdapter(_controller.gameObject, _controller);
            _groundDetection = new GroundDetection(objAdapter);
            _groundDetection.Awake();
        }

        public object RealValue
        {
            get { return _controller; }
        }

        public void Rotate(Quaternion target, float deltaTime)
        {
            _controller.transform.rotation = target;
        }

        public virtual void Move(Vector3 dist, float deltaTime = 0)
        {
            DetectGround();
            //
            ClearHitInfos();
            _controller.Move(dist);
            PostGround();
        }

        private void ClearHitInfos()
        {
            var script = _controller.GetComponent<PlayerScript>();
            if (script != null)
            {
                script.Reset();
            }
        }

        private void PostGround()
        {
            // If we have found valid ground reset ground detection cast distance
            _groundDetection.castDistance = _referenceCastDistance;
        }

        private void ResetGroundInfo()
        {
            _groundDetection.ResetGroundInfo();

            isSliding = false;
        }
        
        private void DetectGround()
        {
            ResetGroundInfo();
            // Perform ground detection and update cast distance based on where we are
            
            _groundDetection.DetectGroundUseControllerInfo();
            _groundDetection.castDistance = _groundDetection.isGrounded ? _referenceCastDistance : 0.0f;
        }

        public Transform transform
        {
            get
            {
                return _controller.transform;
            }
        }
        public GameObject gameObject
        {
            get { return _controller.gameObject; }
        }

        public float radius
        {
            get { return _controller.radius; }
        }

        public float height
        {
            get { return _controller.height; }
        }
        public Vector3 center
        {
            get { return _controller.center; }
        }

        public Vector3 direction
        {
            get { return  Vector3.up;}
        }

        public virtual bool enabled
        {
            get { return _controller.enabled; }
            set { _controller.enabled = value; }
        }

        public bool isGrounded
        {
            //get { return _controller.isGrounded; }
            get { return _groundDetection.isOnGround; }
        }

        /// <summary>
        /// Is a valid slope to walk without slide?
        /// </summary>

        public bool isValidSlope
        {
            get { return !_slideOnSteepSlope || _groundDetection.groundAngle < slopeLimit; }
        }
        

        public float slopeLimit
        {
            get { return _controller.slopeLimit; }
        }

        public void SetCharacterPosition(Vector3 targetPos)
        {
            _controller.transform.position = targetPos;
        }

        public void SetCharacterRotation(Quaternion rot)
        {
            _controller.transform.rotation = rot;
        }

        public void SetCharacterRotation(Vector3 euler)
        {
            _controller.transform.rotation = Quaternion.Euler(euler);
        }

        public virtual void Init()
        {
            _groundDetection.groundLimit = slopeLimit;
            _groundDetection.stepOffset = _controller.stepOffset;
            _groundDetection.ledgeOffset = 0.0f;
            _groundDetection.castDistance = CastDistance;
            _groundDetection.groundMask = UnityLayers.AllCollidableLayerMask;
            //_groundDetection.groundMask = UnityLayers.SceneCollidableLayerMask;
            _referenceCastDistance = CastDistance;
            _groundDetection.OnValidate();
        }

        public CollisionFlags collisionFlags
        {
            get { return _controller.collisionFlags; }
        }

        public Vector3 GetLastGroundNormal()
        {
            //var ps = _controller.gameObject.GetComponent<PlayerScript>();
            //return ps.CollisionNormal;
            return _groundDetection.groundNormal;
        }

        public Vector3 GetLastGroundHitPoint()
        {
//            var ps = _controller.gameObject.GetComponent<PlayerScript>();
//            return ps.HitPoint;
            return _groundDetection.groundPoint;
        }

        public ControllerHitInfo GetCharacterControllerHitInfo(HitType type = HitType.Down)
        {
            var ps = _controller.gameObject.GetComponent<PlayerScript>();
            return ps.GetHitInfo(type);
        }
        
        public KeyValuePair<float, float> GetRotateBound(Quaternion prevRot, Vector3 prevPos, int frameInterval)
        {
            return new KeyValuePair<float, float>(-180f,180f);
        }

        public GroundHit GetGroundHit
        {
            get
            {
                return _groundDetection.groundHit;
            }
        }

        public Collider GetCollider()
        {
            return _controller;
        }

        public void DrawBoundingBox()
        {
            var characterTransformToCapsuleBottom = center + (-direction * (height * 0.5f));
            var characterTransformToCapsuleTop = center + (direction * (height * 0.5f));
            //DebugDraw.EditorDrawCapsule(transform.position + transform.rotation * characterTransformToCapsuleBottom, transform.position + transform.rotation * characterTransformToCapsuleTop, radius, Color.magenta);
            DebugDraw.DebugCapsule(transform.position + transform.rotation * characterTransformToCapsuleBottom, transform.position + transform.rotation * characterTransformToCapsuleTop, Color.magenta, radius);
            
        }

        public void DrawLastGroundHit()
        {
            //DebugDraw.EditorDrawArrow(GetLastGroundHitPoint(), GetLastGroundNormal(), Color.red);
            DebugDraw.DebugArrow(GetLastGroundHitPoint(), GetLastGroundNormal(), Color.red);
        }

        private void DrawLastHit()
        {
            var inof = GetCharacterControllerHitInfo();
            //DebugDraw.DebugArrow(inof.HitPoint, inof.HitNormal, Color.magenta);
        }

        public void DrawGround()
        {
            _groundDetection.TestDrawGizmos();
            //DrawLastHit();
        }
    }
}
