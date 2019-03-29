using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using KinematicCharacterController;
using UnityEngine;

namespace App.Shared.GameModules.Player.ConcreteCharacterController
{
    public class ProneController: BaseCharacterController
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ProneController));

        private List<Collider> IgnoredColliders = new List<Collider>();

        

        public ProneController()
        {
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime, out Vector3 deltaEuler, out bool forceRotate)
        {
            //deltaEuler = _deltaRot;
            deltaEuler = _deltaRot;
            forceRotate = true;
            //Logger.InfoFormat("deltaEuler:{0}", deltaEuler);
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {

        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 targetMovementVelocity = Vector3.zero;
            // Ground movement
            if (Motor.GroundingStatus.IsStableOnGround)
            {
                Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
                var moveVec = _moveVec;
                // Calculate target velocity
                // 左手系的话，应该是左, 前×上，逆时针，左
                var _moveInputVector = Vector3.ProjectOnPlane(moveVec, Vector3.up);
                Vector3 inputLeft = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                // 上×左，顺时针，前
                Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputLeft).normalized *
                                          _moveInputVector.magnitude;
                targetMovementVelocity = reorientedInput;
                currentVelocity = targetMovementVelocity;
                //currentVelocity = _moveInputVector;
                //Logger.InfoFormat("currentVelocity:{0}!!!", currentVelocity.ToString("F4"));
            }
            //Air move
            else
            {
                var moveVec = _moveVec;
                targetMovementVelocity = moveVec;
//                // Add move input
//                if (moveVec.sqrMagnitude > 0f)
//                {
//                    targetMovementVelocity = moveVec;
//
//                    // Prevent climbing on un-stable slopes with air movement
//                    if (Motor.GroundingStatus.FoundAnyGround)
//                    {
//                        Vector3 perpenticularObstructionNormal = Vector3
//                            .Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal),
//                                Motor.CharacterUp).normalized;
//                        targetMovementVelocity =
//                            Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
//                    }
//                }

                currentVelocity = targetMovementVelocity;
            }
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

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            //DebugDraw.DrawArrow(hitPoint, hitNormal * 2.0f, Color.black);
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            //DebugDraw.DrawArrow(hitPoint, hitNormal * 5.0f, Color.green);
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
//            DebugDraw.DrawArrow(hitPoint, hitNormal, Color.yellow);
//            if (hitStabilityReport.LedgeDetected)
//            {
//                DebugDraw.DrawArrow(hitPoint, hitStabilityReport.LedgeGroundNormal, Color.green);
//                DebugDraw.DrawArrow(hitPoint, hitStabilityReport.LedgeRightDirection, Color.red);
//                DebugDraw.DrawArrow(hitPoint, hitStabilityReport.LedgeFacingDirection, Color.blue);
//            }
        }

        public override void SortOverlap(int nums, Collider[] colliders)
        {
			return;
            if (nums <= 1)
            {
                return;
            }

            int start = 0;
            int end = nums - 1;
            bool flag = false;
            while (start < end)
            {
                if (colliders[end] is CharacterController)
                {
                    while ((colliders[start] is CharacterController) && start < end )
                    {
                        start++;
                    }

                    if (start != end)
                    {
                        var tmp = colliders[end];
                        colliders[end] = colliders[start];
                        colliders[start] = tmp;
                        flag = true;
                    }
                }
                
                end--;
            }

            for (int i = 0; i < nums; i++)
            {
                Logger.InfoFormat("{0}, collider:{1}\n", i, colliders[i].name);
            }
            

        }
    }
}
