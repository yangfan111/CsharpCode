using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("Resets the variables.")]
    public class MoveRandomWithDir : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IRobotPlayerWapper wapper;
        private Quaternion lookRotation = Quaternion.identity;
        private PlayerEntity mEntity;
        public SharedInt duration ;
        private float startTime;
        public override void OnAwake()
        {
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            wapper = mEntity.robot.Wapper;
          
        }

        public override void OnStart()
        {
            startTime = Time.time;
            lookRotation = Quaternion.Euler(0, Random.Range(0,360), 0);
            wapper.RobotUserCmdProvider.Reset();
            base.OnStart();
            
        }

        public override TaskStatus OnUpdate()
        {
            if (Time.time > startTime + duration.Value)
            {
                wapper.RobotUserCmdProvider.HasPath = false;
                wapper.RobotUserCmdProvider.DesirwdVelocity =Vector3.zero;
                wapper.RobotUserCmdProvider.LookAt = lookRotation;
                return TaskStatus.Success;
            }
            else
            {
                wapper.RobotUserCmdProvider.HasPath = true;
                wapper.RobotUserCmdProvider.DesirwdVelocity = lookRotation.Forward();
            
                wapper.RobotUserCmdProvider.LookAt = lookRotation;
                return TaskStatus.Running;
            }

          
        }
    }
}