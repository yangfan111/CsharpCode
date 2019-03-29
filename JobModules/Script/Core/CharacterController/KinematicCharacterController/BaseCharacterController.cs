using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace KinematicCharacterController
{
    public abstract class BaseCharacterController
    {
        private static readonly LoggerAdapter Log = new LoggerAdapter(typeof(BaseCharacterController));
        protected Vector3 _moveVec;
        protected Vector3 _deltaRot;

        /// <summary>
        // The KinematicCharacterMotor that will be assigned to this CharacterController via the inspector
        /// </summary>
        public KinematicCharacterMotor Motor { get; private set; }

        /// <summary>
        /// This is called by the KinematicCharacterMotor in its Awake to setup references
        /// </summary>
        public void SetupCharacterMotor(KinematicCharacterMotor motor)
        {
            Motor = motor;
            motor.CharacterController = this;
        }

        public void SetVec(Vector3 vec)
        {
            _moveVec = vec;
            //Log.InfoFormat("set vec:{0}", _moveVec.ToString("F4"));
        }

        public void SetDeltaRot(Vector3 rot)
        {
            _deltaRot = rot;
            
        }
        
        /// <summary>
        /// Asks what the character's rotation should be on this character update. 
        /// Modify the "currentRotation" to change the character's rotation.
        /// </summary>
        public abstract void UpdateRotation(ref Quaternion currentRotation, float deltaTime);

        public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime, out Vector3 deltaEuler, out bool forceRotate)
        {
            throw new Exception("UpdateRotation is not impelement!");
        }
        
        /// <summary>
        /// Asks what the character's velocity should be on this character update. 
        /// Modify the "currentVelocity" to change the character's velocity.
        /// </summary>
        public abstract void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);

        /// <summary>
        /// Gives you a callback for before the character update begins, if you 
        /// want to do anything to start off the update.
        /// </summary>
        public abstract void BeforeCharacterUpdate(float deltaTime);

        /// <summary>
        /// Gives you a callback for when the character has finished evaluating its grounding status
        /// </summary>
        public abstract void PostGroundingUpdate(float deltaTime);

        /// <summary>
        /// Gives you a callback for when the character update has reached its end, if you 
        /// want to do anything to finalize the update.
        /// </summary>
        public abstract void AfterCharacterUpdate(float deltaTime);

        /// <summary>
        /// Asks if a given collider should be considered for character collisions.
        /// Useful for ignoring specific colliders in specific situations.
        /// </summary>
        public abstract bool IsColliderValidForCollisions(Collider coll);

        /// <summary>
        /// Gives you a callback for ground probing hits
        /// </summary>
        public abstract void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

        /// <summary>
        /// Gives you a callback for character movement hits
        /// </summary>
        public abstract void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

        /// <summary>
        /// Gives you a chance to modify the HitStabilityReport of every character movement hit before it is returned to the movement code.
        /// Use this for advanced customization of character hit stability
        /// </summary>
        public abstract void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);
        
        /// <summary>
        /// <p>默认的移动策略</p>
        /// <p>1. 如果稳定地面和稳定移动(角度大于AngleLimit)，比如在斜坡上移动, result:速度会根据CharacterUp，重新改变方向，没有能量丢失</p>
        /// <p>2. 如果稳定地面和不稳定移动，比如撞到了一堵微微倾斜的墙, result:速度会保护，根据投影，重新改变方向，能量丢失</p>
        /// <p>3. 如果不稳定地面(在空中)和稳定移动(撞到稳定平面), result:速度先投影到CharacterUp平面，然后沿着切坡的方向</p>
        /// <p>4. 如果不稳定地面(在空中)和不稳定移动(撞到稳定平面), result:后沿着不稳定平面的方向</p>
        /// Allows you to override the way velocity is projected on an obstruction
        /// </summary>
        public virtual void HandleMovementProjection(ref Vector3 movement, Vector3 obstructionNormal, bool stableOnHit)
        {
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.MustUnground)
            {
                // On stable slopes, simply reorient the movement without any loss
                if (stableOnHit)
                {
                    movement = Motor.GetDirectionTangentToSurface(movement, obstructionNormal) * movement.magnitude;
                }
                // On blocking hits, project the movement on the obstruction while following the grounding plane
                else
                {
                    //这几个名字有迷惑性，其实是沿着障碍物走
                    Vector3 obstructionRightAlongGround = Vector3.Cross(obstructionNormal, Motor.GroundingStatus.GroundNormal).normalized;
                    Vector3 obstructionUpAlongGround = Vector3.Cross(obstructionRightAlongGround, obstructionNormal).normalized;
                    movement = Motor.GetDirectionTangentToSurface(movement, obstructionUpAlongGround) * movement.magnitude;
                    //丢失能量
                    movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
                }
            }
            else
            {
                if (stableOnHit)
                {
                    // Handle stable landing
                    movement = Vector3.ProjectOnPlane(movement, Motor.CharacterUp);
                    movement = Motor.GetDirectionTangentToSurface(movement, obstructionNormal) * movement.magnitude;
                }
                // Handle generic obstruction
                else
                {
                    movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
                }
            }
        }

        public virtual void SortOverlap(int nums, Collider[] colliders)
        {
            
        }
    }
}