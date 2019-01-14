using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot;
using App.Shared.GameModules.Player.Robot.SharedVariables;

namespace Assets.App.Shared.GameModules.Player.Robot.Action
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;


    [TaskCategory("Voyager")]
    [TaskDescription("Resets the variables.")]
    public class Reset : Action
    {
        [Tooltip("The GameObject to attack")] public SharedGameObject m_Target;

        [Tooltip("The possible GameObject to attack")]
        public SharedGameObject m_PossibleTarget;

        [Tooltip("A set of targets that the agent should ignore")]
        public SharedGameObjectSet m_IgnoreTargets;

        [Tooltip("The GameObject which the agent is using as cover")]
        public SharedGameObject m_BackupRequestor;

        [Tooltip("The target of the teammate who is requesting backup")]
        public SharedGameObject m_BackupTarget;

        [Tooltip("A bool representing if the agent should search for the target")]
        public SharedBool m_Search;

        [Tooltip("A bool representing if the agent can attack the target")]
        public SharedBool m_CanAttack;

        [Tooltip("Should the cancel backup request be sent?")]
        public SharedBool m_CancelBackupRequest;

        private PlayerEntity mEntity;
        private IRobotPlayerWapper wapper;

        /// <summary>
        /// Cache the component references.
        /// </summary>
        public override void OnAwake()
        {
            base.OnAwake();
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            wapper = mEntity.robot.Wapper;
        }

        /// <summary>
        /// Reset the SharedVariables.
        /// </summary>
        /// <returns>Always returns Success.</returns>
        public override TaskStatus OnUpdate()
        {
            if (!m_Target.IsNone)
            {
                m_Target.Value = null;
            }

            if (!m_PossibleTarget.IsNone)
            {
                m_PossibleTarget.Value = null;
            }

            if (!m_IgnoreTargets.IsNone)
            {
                m_IgnoreTargets.Value.Clear();
            }

            if (!m_BackupRequestor.IsNone)
            {
                m_BackupRequestor.Value = null;
            }

            if (!m_BackupTarget.IsNone)
            {
                m_BackupTarget.Value = null;
            }

            if (!m_Search.IsNone)
            {
                m_Search.Value = false;
            }

            if (!m_CanAttack.IsNone)
            {
                m_CanAttack.Value = true;
            }

            wapper.StopNavMeshAgent();
            return TaskStatus.Success;
        }
    }
}