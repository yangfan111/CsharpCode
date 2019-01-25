using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;

namespace Core.CharacterController
{
    public class UnityCharacterController:ICharacterController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UnityCharacterController));
        protected UnityEngine.CharacterController _controller;
        public UnityCharacterController(UnityEngine.CharacterController controller)
        {
            _controller = controller;
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
            _controller.Move(dist);
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
            get { return _controller.isGrounded; }
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

        public void Init()
        {
            
        }

        public CollisionFlags collisionFlags
        {
            get { return _controller.collisionFlags; }
        }

        public Vector3 GetLastGroundNormal()
        {
            var ps = _controller.gameObject.GetComponent<PlayerScript>();
            return ps.CollisionNormal;
        }

        public Vector3 GetLastGroundHitPoint()
        {
            var ps = _controller.gameObject.GetComponent<PlayerScript>();
            return ps.HitPoint;
        }

        public KeyValuePair<float, float> GetRotateBound(Quaternion prevRot, Vector3 prevPos, int frameInterval)
        {
            return new KeyValuePair<float, float>(-180f,180f);
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
    }
}
