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
        }


        public bool IsOnLedge()
        {
            return isOnLedgeEmptySide || isOnLedgeSolidSide;
        }

        public bool IsSlideSlopeGround(float slideAngle)
        {
            return Vector3.Angle(groundNormal, Vector3.up) > slideAngle &&
                   (isOnGround && !isOnLedgeEmptySide && !isOnStep);
        }
        
        #endregion


    }
}
