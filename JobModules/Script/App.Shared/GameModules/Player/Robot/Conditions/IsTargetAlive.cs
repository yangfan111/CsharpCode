using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot;
using App.Shared.GameModules.Player.Robot.Utility;
using Assets.App.Shared.GameModules.Player.Robot.SharedVariables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Assets.App.Shared.GameModules.Player.Robot.Conditions
{
    [TaskCategory("Voyager ")]
    [TaskDescription("Is the target Alive?")]
    public class IsTargetAlive : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The existing target PlayEntity")] [SerializeField]
        protected SharedPlayerEntity m_Target;


        /// <summary>
        /// Cache the component references.
        /// </summary>
        public override void OnAwake()
        {
           
        }

        /// <summary>
        /// Returns success if the target is in sight and is alive.
        /// </summary>
        /// <returns>Success if the target is within sight and is alive.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_Target != null && m_Target.Value == null)
            {
                var targetEntity = m_Target.Value;
                if (targetEntity.gamePlay.LifeState != (int) EPlayerLifeState.Dead)
                {
                    return targetEntity.gamePlay.LifeState != (int) EPlayerLifeState.Dead
                        ? TaskStatus.Success
                        : TaskStatus.Failure;
                }
            }

            return TaskStatus.Failure;
        }
    }
}