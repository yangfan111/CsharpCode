using UnityEngine;

namespace Core.CharacterController
{   
    public struct ControllerHitInfo
    {
        public Vector3 HitNormal;
        public Vector3 HitPoint;
        public Collider HitCollider;
        public float MoveLength;
        public Rigidbody Rigidbody;
        public bool Valid;

        public ControllerHitInfo(Vector3 hitNormal, Vector3 hitPoint, Collider hitCollider, float moveLength, Rigidbody rigidbody, bool valid)
        {
            HitNormal = hitNormal;
            HitPoint = hitPoint;
            HitCollider = hitCollider;
            MoveLength = moveLength;
            Rigidbody = rigidbody;
            Valid = valid;
        }

        public void Reset()
        {
            HitNormal = Vector3.zero;
            HitPoint = Vector3.zero;
            HitCollider = null;
            MoveLength = 0f;
            Rigidbody = null;
            Valid = false;
        }
    }
    
    public class PlayerScript : MonoBehaviour
    {

        private ControllerHitInfo DownHit = new ControllerHitInfo();
        private ControllerHitInfo ForwardHit = new ControllerHitInfo();
        private ControllerHitInfo UpHit = new ControllerHitInfo();
        const float MoveDirectionLen = 0.9999f;
        public bool IsOnGround { get; private set; }

        public void Reset()
        {
            DownHit.Reset();
            ForwardHit.Reset();
            UpHit.Reset();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.moveDirection.y < -MoveDirectionLen)
            {
                SetHitInfo(hit, ref DownHit);
                IsOnGround = true;
                //Debug.DrawRay(hit.point, hit.normal * 5, Color.green, 1, false);

            }
            else if (hit.moveDirection.y > MoveDirectionLen)
            {
                SetHitInfo(hit, ref UpHit);
                //Debug.DrawRay(hit.point, hit.normal * 5, Color.black, 1, false);

            }
            else
            {
                SetHitInfo(hit, ref ForwardHit);
                //Debug.DrawRay(hit.point, hit.normal * 5, Color.magenta, 1, false);
            }
        }

        private void SetHitInfo(ControllerColliderHit hit, ref ControllerHitInfo hitInfo)
        {
            hitInfo.Valid = true;
            hitInfo.HitNormal = hit.normal;
            hitInfo.HitPoint = hit.point;
            hitInfo.HitCollider = hit.collider;
            hitInfo.MoveLength = hit.moveLength;
            hitInfo.Rigidbody = hit.rigidbody;
        }

        public ControllerHitInfo GetHitInfo(HitType type = HitType.Down)
        {
            switch (type)
            {
                case HitType.Down:
                    return DownHit;
                    break;
                case HitType.Forward:
                    return ForwardHit;
                    break;
                case HitType.Up:
                    return UpHit;
                    break;
                default:
                    return DownHit;
                    break;
            }
        }
    }

    public enum HitType
    {
        Down,
        Forward,
        Up,
    }
}
