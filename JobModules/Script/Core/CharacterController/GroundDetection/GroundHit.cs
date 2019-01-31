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

        public bool isOnGround { get; set; }

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

        public Vector3 groundNormal { get; set; }

        public float groundDistance { get;  set; }

        public Collider groundCollider { get;  set; }

        public Rigidbody groundRigidbody { get;  set; }

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
