using App.Shared.Components.Player;
using App.Shared.Util;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.Module
{
    public class NavMeshBridgeSystem : AbstractUserCmdExecuteSystem
    {
        private PlayerContext _playerContext;
        private IGroup<PlayerEntity> _group;
        LoggerAdapter _logger = new LoggerAdapter(typeof(NavMeshBridgeSystem));
    

        public NavMeshBridgeSystem(PlayerContext playerContext)
        {
            _playerContext = playerContext;
            _group = _playerContext.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Robot,
                PlayerMatcher.ThirdPersonModel));
        }

        


        protected void OnGrounded(IRobotPlayerWapper robot, bool grounded)
        {
            if (grounded)
            {
                // The agent is no longer on an off mesh link if they just landed.
                if (robot.NavMeshAgent.isOnOffMeshLink)
                {
                    robot.NavMeshAgent.CompleteOffMeshLink();
                }

                // Warp the NavMeshAgent just in case the navmesh position doesn't match the transform position.
                var destination = robot.NavMeshAgent.destination;
                robot.NavMeshAgent.Warp(robot.Entity.position.Value);
                // Warp can change the destination so make sure that doesn't happen.
                if (robot.NavMeshAgent.destination != destination)
                {
                    robot.NavMeshAgent.SetDestination(destination);
                }
                robot.NavMeshAgent.updateRotation = true;
                robot.NavMeshAgent.updatePosition = true;
                robot.NavMeshAgent.velocity=Vector3.zero;
            }
        }


        public void OnRespawn(IRobotPlayerWapper robot)
        {
            // Reset the NavMeshAgent to the new position.
            robot.NavMeshAgent.Warp(robot.Entity.position.Value);
            if (robot.NavMeshAgent.isOnOffMeshLink)
            {
                robot.NavMeshAgent.ActivateCurrentOffMeshLink(false);
            }

            robot.NavMeshAgent.updateRotation = true;
            robot.NavMeshAgent.updatePosition = true;
            robot.NavMeshAgent.velocity=Vector3.zero;
        }

       

        protected override bool filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasRobot && playerEntity.robot.Wapper != null;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var wapper = playerEntity.robot.Wapper;
             
            if (wapper.IsOnGround != wapper.LastIsOnGround)
            {
                wapper.LastIsOnGround = wapper.IsOnGround;
                OnGrounded(wapper, wapper.IsOnGround);
            }
            if(wapper.LifeState != wapper.LastLifeState && wapper.LifeState == (int)EPlayerLifeState.Alive)
            {
                OnRespawn(wapper);
            }
        }
    }
}