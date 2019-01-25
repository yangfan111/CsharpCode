using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("SimplePose.")]
    public class SimplePose : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IRobotPlayerWapper wapper;
        private Quaternion lookRotation = Quaternion.identity;
        private PlayerEntity mEntity;
        public SharedInt duration ;
        public SharedBool IsJump ;
        public SharedBool IsCrouch;
        public SharedBool IsProne;
        public SharedBool IsPeekLeft;
        public SharedBool IsPeekRight;
        public SharedBool IsLeftAttack;
        public SharedBool IsReload;
        private float startTime;
        public override void OnAwake()
        {
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            wapper = mEntity.robot.Wapper;
          
        }
        
        public override void OnStart()
        {
            startTime = Time.time;
            wapper.RobotUserCmdProvider.Reset();
            lookRotation = Quaternion.Euler(0, Random.Range(0,360), 0);
            base.OnStart();
           
        }

        public override TaskStatus OnUpdate()
        {
            if (Time.time > startTime + duration.Value)
            {
                wapper.RobotUserCmdProvider.Reset();
                return TaskStatus.Success;
            }
            else
            {
                wapper.RobotUserCmdProvider.HasPath = true;
                wapper.RobotUserCmdProvider.IsJump = IsJump.Value;
                wapper.RobotUserCmdProvider.IsCrouch = IsCrouch.Value;
                wapper.RobotUserCmdProvider.IsProne = IsProne.Value;
                wapper.RobotUserCmdProvider.IsCrouch = IsCrouch.Value;
                wapper.RobotUserCmdProvider.IsPeekLeft = IsPeekLeft.Value;
                wapper.RobotUserCmdProvider.IsPeekRight = IsPeekRight.Value;
                wapper.RobotUserCmdProvider.LookAt = lookRotation;
                wapper.RobotUserCmdProvider.IsLeftAttack = IsLeftAttack.Value;
                wapper.RobotUserCmdProvider.IsReload = IsReload.Value;
                return TaskStatus.Running;
            }

          
        }
    }
}