using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot.SharedVariables;
using App.Shared.GameModules.Player.Robot.Utility;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.Conditions
{

    [TaskCategory("Voyager ")]
    [TaskDescription("Is the target in sight?")]
    public class IsTargetInSight : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The existing target GameObject")]
        [SerializeField] protected SharedGameObject m_Target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum field of view that the agent can see other targets (in degrees)")]
        [SerializeField] protected SharedFloat m_FieldOfView = 90;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum distance that the agent can see other targets")]
        [SerializeField] protected SharedFloat m_MaxTargetDistance;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject within sight")]
        [SharedRequired] [SerializeField] protected SharedGameObject m_FoundTarget;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("A set of targets that the agent should ignore")]
        [SharedRequired] [SerializeField] protected SharedGameObjectSet m_IgnoreTargets;

        private PlayerEntity mEntity;

        /// <summary>
        /// Cache the component references.
        /// </summary>
        public override void OnAwake()
        {
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
        }

        /// <summary>
        /// Returns success if the target is in sight and is alive.
        /// </summary>
        /// <returns>Success if the target is within sight and is alive.</returns>
        public override TaskStatus OnUpdate()
        {
            var target = mEntity.robot.Wapper.TargetInSight(m_FieldOfView.Value, m_MaxTargetDistance.Value, m_IgnoreTargets.Value);
            if (target != null && (m_Target.Value == null || target != m_Target.Value ))
            {
                var targetEntity = RobotUtility.GetComponentForType<EntityReference>(target).Reference as PlayerEntity;
                if (targetEntity.gamePlay.LifeState != (int) EPlayerLifeState.Dead)
                {
                    m_FoundTarget.Value = target;
                }
            }
            return target != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
