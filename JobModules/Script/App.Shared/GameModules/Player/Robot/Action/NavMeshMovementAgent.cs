using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot.SharedVariables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace App.Shared.GameModules.Player.Robot.Action
{
    public class NavMeshMovementAgent : NavMeshMovement
    {
        
        protected bool SamplePosition(Vector3 position, out Vector3 hitPosition, float dist)
        {
            NavMeshHit hit;
            var b= NavMesh.SamplePosition(position, out hit, dist, NavMesh.AllAreas);
            hitPosition = hit.position;
            return b;
        }
        public override void OnAwake()
        {
            base.OnAwake();
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            wapper = mEntity.robot.Wapper;
            navMeshAgent.updatePosition = false;
            
        }

        private IRobotPlayerWapper wapper;
        private Quaternion lookRotation;
        private PlayerEntity mEntity;

        private float GetTimeByJumpDist(IRobotPlayerWapper robot, float dist)
        {
            var delta = Mathf.Sqrt(robot.RobotSpeedInfo.JumpVo * robot.RobotSpeedInfo.JumpVo +
                                   dist * robot.RobotSpeedInfo.JumpAcceleration *
                                   robot.RobotSpeedInfo.JumpAcceleration);
            var t = (delta - robot.RobotSpeedInfo.JumpVo) / robot.RobotSpeedInfo.JumpAcceleration;
            return t;
        }

        private bool UpdateOffMeshLink(IRobotPlayerWapper robot, ref Vector3 velocity, ref Quaternion lookRotation)
        {
            // Ignore the y difference when determining a look direction and velocity.
            // This will give XZ distances a greater impact when normalized.
            var direction = robot.NavMeshAgent.currentOffMeshLinkData.endPos - robot.Entity.position.Value;
            var jumpY = direction.y;
            direction.y = 0;
            lookRotation = Quaternion.LookRotation(direction);

            if (direction.sqrMagnitude > 0.1f || robot.Entity.playerMove.IsGround)
            {
                if (jumpY > robot.RobotSpeedInfo.JumpMaxHeight)
                {
                    return false;
                }
                else
                {
                    var l = 2 * robot.RobotSpeedInfo.JumpMaxHeight - jumpY;
                    velocity = (direction.magnitude + 0.5f) / GetTimeByJumpDist(robot, l) * direction.normalized;
                    velocity.y = 0;
                    return true;
                }
            }

            return false;
        }

        private void Run(IRobotPlayerWapper robot)
        {
            var velocity = Vector3.zero;

            robot.RobotUserCmdProvider.HasPath = false;
            robot.RobotUserCmdProvider.IsJump = false;
            robot.RobotUserCmdProvider.DesirwdVelocity = Vector3.zero;

            if (robot.NavMeshAgent.isOnOffMeshLink && robot.Entity.playerMove.IsGround &&
                UpdateOffMeshLink(robot, ref velocity, ref lookRotation))
            {
                
                robot.RobotUserCmdProvider.HasPath = true;
                lookRotation.SetLookRotation(velocity);
                robot.RobotUserCmdProvider.DesirwdVelocity = velocity;

                robot.RobotUserCmdProvider.IsJump = true;
            }
            else
            {
                // Only move if a path exists.
                if (!HasArrived() && robot.NavMeshAgent.path.corners.Length > 1)
                {
                    var forward =robot.NavMeshAgent.path.corners[1] - robot.NavMeshAgent.path.corners[0];
                    forward.y = 0;
                    lookRotation = Quaternion.LookRotation(forward);

                    robot.RobotUserCmdProvider.HasPath = true;
                    robot.RobotUserCmdProvider.DesirwdVelocity = forward;
                }
            }

            // Don't let the NavMeshAgent move the character - the robot.RobotUserCmdProvider can move it.
           
         
            robot.RobotUserCmdProvider.LookAt = lookRotation;
          
            //robot.NavMeshAgent.nextPosition = robot.Position;
        }

        public override TaskStatus OnUpdate()
        {
            Run(wapper);
            
            return base.OnUpdate();
        }
    }
}