using System.Collections.Generic;
using Core.Utils;
using KinematicCharacterController;
using UnityEngine;

namespace App.Shared.GameModules.Player.ConcreteCharacterController
{
    public class SwimController : BaseCharacterController
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SwimController));

        private List<Collider> IgnoredColliders = new List<Collider>();



        public SwimController()
        {
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime, out Vector3 deltaEuler, out bool forceRotate)
        {
            deltaEuler = Vector3.zero;
            forceRotate = false;
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {

        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            currentVelocity = _moveVec;
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {

        }

        public override void PostGroundingUpdate(float deltaTime)
        {

        }

        public override void AfterCharacterUpdate(float deltaTime)
        {

        }

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count >= 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void HandleMovementProjection(ref Vector3 movement, Vector3 obstructionNormal, bool stableOnHit)
        {
            Vector3 characterFoward = Quaternion.Euler(0, Motor.FlyOffsetY, 0) * Vector3.forward;
            Vector3 directionLeft;
            // On stable slopes, simply reorient the movement with loss
            if (stableOnHit)
            {
                movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
            }
            
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.MustUnground)
            {
                // On blocking hits, project the movement on the obstruction while following the grounding plane
                if (!stableOnHit)
                {
                    //这几个名字有迷惑性，其实是沿着障碍物走
                    Vector3 obstructionRightAlongGround = Vector3.Cross(obstructionNormal, Motor.GroundingStatus.GroundNormal).normalized;
                    Vector3 obstructionUpAlongGround = Vector3.Cross(obstructionRightAlongGround, obstructionNormal).normalized;
                    movement = Motor.GetDirectionTangentToSurfaceCustom(movement, obstructionUpAlongGround, characterFoward, out directionLeft) * movement.magnitude;
                    //丢失能量
                    movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
                    
                }
                
            }
            else
            {
                // Handle generic obstruction
                if (!stableOnHit)
                {
                    movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
                }
            }
        }
    }
}