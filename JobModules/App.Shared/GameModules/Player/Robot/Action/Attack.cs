using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using Assets.App.Shared.GameModules.Player.Robot.SharedVariables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.Action
{
    
    [TaskCategory("Voyager")]
    [TaskDescription("Attacks the target. The agent will only attack if the target is within sight.")]
     public class Attack :BehaviorDesigner.Runtime.Tasks.Action
    {
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum field of view that the agent can see other targets (in degrees)")]
        [SerializeField] protected SharedFloat m_FieldOfView = 90;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject to attack")]
        [SerializeField] protected SharedPlayerEntity m_Target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Is the target within sight?")]
        [SerializeField] protected SharedBool m_TargetInSight;

        private PlayerEntity mEntity;
        private IRobotPlayerWapper wapper;

        /// <summary>
        /// Cache the component references and initialize the SharedFields.
        /// </summary>
        public override void OnAwake()
        {
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            wapper = mEntity.robot.Wapper;
        }


        /// <summary>
        /// Perform the attack.
        /// </summary>
        /// <returns>Success if the target was attacked.</returns>
        public override TaskStatus OnUpdate()
        {
           
           

            if (wapper.CanFire())
            {
              
                m_TargetInSight.Value = true;


                wapper.UserCmdGenerator.SetUserCmd((cmd) => { cmd.IsLeftAttack = true; }
                );
            }


            return  TaskStatus.Success;
        }

        /// <summary>
        /// Reset the Behavior Designer variables.
        /// </summary>
        public override void OnReset()
        {
         
            m_Target = null;
            m_TargetInSight = false;
        }
    }
}