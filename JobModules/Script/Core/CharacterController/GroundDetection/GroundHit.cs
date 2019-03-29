using Core.CharacterController;
using Core.Utils;
using UnityEngine;

namespace ECM.Components
{
    public struct GroundHit
    {
        #region FIELDS

        private float _ledgeDistance;

        private float _stepHeight;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Is this character standing on ANY 'ground'?,碰到地面就算onground
        /// </summary>
        public bool isOnGround { get; set; }

        /// <summary>
        /// Is this character standing on VALID 'ground'?,!groundHitInfo.isOnLedgeEmptySide && Vector3.Angle(groundHitInfo.surfaceNormal, Vector3.up) < groundLimit
        /// </summary>
        public bool isValidGround { get; set; }

        public bool isOnLedgeSolidSide { get; set; }

        public bool isOnLedgeEmptySide { get; set; }

        public float ledgeDistance
        {
            get { return _ledgeDistance; }
            set { _ledgeDistance = Mathf.Max(0.0f, value); }
        }

        public bool isOnStep { get; set; }

        public float stepHeight
        {
            get { return _stepHeight; }
            set { _stepHeight = Mathf.Max(0.0f, value); }
        }

        public Vector3 groundPoint { get; set; }

        /// <summary>
        /// The normal of the 'ground' surface.
        /// </summary>
        public Vector3 groundNormal { get; set; }

        public float groundDistance { get;  set; }

        public Collider groundCollider { get;  set; }

        public Rigidbody groundRigidbody { get;  set; }

        /// <summary>
        /// The real surface normal.
        /// 
        /// This cab be different from groundNormal, because when SphereCast contacts the edge of a collider
        /// (rather than a face directly on) the hit.normal that is returned is the interpolation of the two normals
        /// of the faces that are joined to that edge.
        /// </summary>
        public Vector3 surfaceNormal { get; set; }

        #endregion

        #region METHODS

        public GroundHit(GroundHit other) : this()
        {
            isOnGround = other.isOnGround;
            isValidGround = other.isValidGround;

            isOnLedgeSolidSide = other.isOnLedgeSolidSide;
            isOnLedgeEmptySide = other.isOnLedgeEmptySide;
            ledgeDistance = other.ledgeDistance;

            isOnStep = other.isOnStep;
            stepHeight = other.stepHeight;

            groundPoint = other.groundPoint;
            groundNormal = other.groundNormal;
            groundDistance = Mathf.Max(0.0f, other.groundDistance);
            groundCollider = other.groundCollider;
            groundRigidbody = other.groundRigidbody;

            surfaceNormal = other.surfaceNormal;
        }

        public GroundHit(RaycastHit hitInfo) : this()
        {
            SetFrom(hitInfo);
        }

        public void SetFrom(RaycastHit hitInfo)
        {
            groundPoint = hitInfo.point;
            groundNormal = hitInfo.normal;
            groundDistance = Mathf.Max(0.0f, hitInfo.distance);
            groundCollider = hitInfo.collider;
            groundRigidbody = hitInfo.rigidbody;
            surfaceNormal = hitInfo.normal;
        }

        public void SetFrom(ControllerHitInfo hitInfo)
        {
            groundPoint = hitInfo.HitPoint;
            groundNormal = hitInfo.HitNormal;
            groundDistance = Mathf.Max(0.0f, hitInfo.MoveLength);
            groundCollider = hitInfo.HitCollider;
            groundRigidbody = hitInfo.Rigidbody;
            surfaceNormal = hitInfo.HitNormal;
        }

        public bool IsOnLedge()
        {
            return isOnLedgeEmptySide || isOnLedgeSolidSide;
        }

        public bool IsSlideSlopeGround(float slideAngle)
        {
            return Vector3.Angle(surfaceNormal, Vector3.up) > slideAngle &&
                   (isOnGround && !isOnLedgeEmptySide && !isOnStep) || IsOnPlayer();
        }

        /// <summary>
        /// 特殊情况，当站在角色上时候，需要下滑
        /// </summary>
        /// <returns></returns>
        private bool IsOnPlayer()
        {
            bool ret = false;
            if (groundCollider != null)
            {
                //站立的角色
                if (groundCollider is CharacterController && IsPlayerLayer())
                {
                    ret = true;
                }
                //趴下角色
                else if (groundCollider is CapsuleCollider && IsPlayerLayer())
                {
                    ret = true;
                }
            }

            return ret;
        }

        private bool IsPlayerLayer()
        {
            return groundCollider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Player);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("LedgeDistance: {0}, StepHeight: {1}, isOnGround: {2}, isValidGround: {3}, isOnLedgeSolidSide: {4}, isOnLedgeEmptySide: {5}, ledgeDistance: {6}, isOnStep: {7}, stepHeight: {8}, groundPoint: {9}, groundNormal: {10}, groundDistance: {11}, groundCollider: {12}, groundRigidbody: {13}, surfaceNormal: {14}", _ledgeDistance, _stepHeight, isOnGround, isValidGround, isOnLedgeSolidSide, isOnLedgeEmptySide, ledgeDistance, isOnStep, stepHeight, groundPoint, groundNormal, groundDistance, groundCollider, groundRigidbody, surfaceNormal);
        }
    }
}
