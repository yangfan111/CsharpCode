using UnityEngine;

namespace Core.CharacterController
{
    public class PlayerScript : MonoBehaviour
    {
        private Vector3 _collisionNormal = Vector3.up;
        private Vector3 _hitPoint = Vector3.zero;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _collisionNormal = hit.normal;
            _hitPoint = hit.point;
            //Debug.DrawRay(_hitPoint, _collisionNormal * 5, Color.black, 1, false);
        }

        public Vector3 CollisionNormal { get { return _collisionNormal; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
    }
}
