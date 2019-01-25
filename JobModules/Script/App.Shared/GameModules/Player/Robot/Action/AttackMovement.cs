using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot.Utility;
using Assets.App.Shared.GameModules.Player.Robot.SharedVariables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;


namespace App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("目标移动的情况下,移到到合适的位置进行攻击")]
    class AttackMovement : NavMeshMovement
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The PlayerEntity to attack")] [SerializeField]
        protected SharedPlayerEntity m_Target;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "Change destinations if the target is looking at the agent within the specified number of degrees")]
        [SerializeField]
        protected SharedFloat m_EvadeAngle = 15;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "Specifies the minimum angle that the agent should move when determining a new destination")]
        [SerializeField]
        protected SharedFloat m_MinMoveAngle = 10;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "Specifies the maximum angle that the agent should move when determining a new destination")]
        [SerializeField]
        protected SharedFloat m_MaxMoveAngle = 20;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "When the agent looks for a new destination a new position is checked to determine if it's a valid position. If more than the threshold number of positions are checked " +
            "then angles which are closer to the agent's current position will be checked.")]
        [SerializeField]
        protected SharedInt m_SmallAngleCount = 10;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "Specifies the distance that the new destination check should increase upon each interation. This is only used if the iteration count is greater than the SmallMovementThreshold")]
        [SerializeField]
        protected SharedFloat m_DistanceStep = 0.01f;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Likelihood that the distance value will be randomly updated")]
        [SerializeField]
        protected SharedFloat m_DistanceUpdateLikelihood = 0.2f;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "Prevent the agent from getting stuck in the same position by multiplying the new destination location by the specified multiplier if the number of distance checks is greater " +
            "than the evade threshold")]
        [SerializeField]
        protected SharedFloat m_StuckIterationCount = 10;

        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "Prevent the agent from getting stuck in the same position by multiplying the new destination location by the specified multiplier if the number of distance checks is greater " +
            "than the evade threshold")]
        [SerializeField]
        protected SharedFloat m_StuckMultiplier = 4;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Is the target within sight?")] 
        [SharedRequired] [SerializeField]
        protected SharedBool m_TargetInSight;

        // Internal variables
        private bool m_PositiveAngle;
        private float m_Distance = float.MaxValue;
        private Vector3 m_Destination;
        private Vector3 m_LookDirection;
        private Vector3 m_EvadeDestination;
        private bool m_SmallAngles;
        private int m_EvadeCount;
        private float m_DistancePercent;
        private IRobotPlayerWapper wapper;

        private PlayerEntity m_PrevTarget;
        private PlayerEntity mEntity;
       

      
        public override void OnAwake()
        {
            base.OnAwake();

            m_PositiveAngle = Random.value < 0.5f;
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            wapper = mEntity.robot.Wapper;
        }

      
        public override void OnStart()
        {
            base.OnStart();

           
            m_Destination = m_Target.Value.position.Value;
        }

        public override TaskStatus OnUpdate()
        {
         
            if (m_Target.Value != m_PrevTarget)
            {
                m_PrevTarget = m_Target.Value;
              
            }

           
            var weaponStat = wapper.GetCurrentWeaponStat();
          
            m_LookDirection = transform.position - wapper.Position;
            m_LookDirection.y = 0;
            var distance = Mathf.Clamp(m_LookDirection.magnitude, weaponStat.MinUseDist, weaponStat.MaxUseDist);
        
            if (distance < m_Distance || Random.value < m_DistanceUpdateLikelihood.Value)
            {
                m_Distance = distance;
            }

            var setDestination = false;
        
            if (Quaternion.Angle(Quaternion.LookRotation(m_LookDirection), m_Target.Value.orientation.RotationView) <
                m_EvadeAngle.Value || !m_TargetInSight.Value)
            {
               
                if (!HasPath())
                {
                    if (!NextDestination(weaponStat, true))
                    {
                        return TaskStatus.Failure;
                    }

                    setDestination = true;
                }
            }
            else
            {
                m_EvadeCount = 0;
            }

            if (!setDestination)
            {
              
                if ((m_Destination - m_Target.Value.position.Value).sqrMagnitude < 0.01f ||
                    wapper.LineOfSight(m_Destination, m_Target.Value, true))
                {
                    if (!NextDestination(weaponStat, false))
                    {
                        return TaskStatus.Failure;
                    }

                    setDestination = true;
                }
            }

        
            var lookRotation = Quaternion.LookRotation(m_Target.Value.position.Value - transform.position);
            wapper.RobotUserCmdProvider.LookAt = lookRotation;

            if (setDestination)
            {
                UpdateRotation(false);
                SetDestination(m_Destination);
            }

            return TaskStatus.Running;
        }

        /// <summary>
        /// Determine a new destination which has the target within sight.
        /// </summary>
        /// <param name="weaponStat">The WeaponStat of the current item.</param>
        /// <param name="newAngle">Should a new angle be retrieved? Will be false on first run.</param>
        /// <returns>True if a new destination was retrieved. The destination will be stored in m_Destination.</returns>
        private bool NextDestination(IRobotWeaponStat weaponStat, bool newAngle)
        {
            m_DistancePercent = 1;
            m_SmallAngles = false;
            var count = 0;
            var direction = m_LookDirection;
            var angle = 0f;
            var flipAngle = false;
            // Keep iterating to find a position which has the target within sight.
            do
            {
                // Do not flip the angle the first time NextDestination is called so the character can continue to move in the direction that they
                // were previously moving in.
                if (flipAngle)
                {
                    m_PositiveAngle = !m_PositiveAngle;
                }

                if (newAngle)
                {
                    if (count > m_SmallAngleCount.Value)
                    {
                        m_SmallAngles = true;
                        m_DistancePercent = Mathf.Clamp01(m_DistancePercent - m_DistanceStep.Value);
                    }

                    if (m_SmallAngles)
                    {
                        angle = Random.Range(0, m_MinMoveAngle.Value);
                    }
                    else if (m_EvadeCount > m_StuckIterationCount.Value)
                    {
                        angle = Random.Range(m_MinMoveAngle.Value, m_MaxMoveAngle.Value) * m_StuckMultiplier.Value;
                    }
                    else
                    {
                        angle = Random.Range(m_MinMoveAngle.Value, m_MaxMoveAngle.Value);
                    }

                    angle *= (m_PositiveAngle ? 1 : -1);
                }

                // Add the random angle to the last direction.
                var lookRotation = Quaternion.LookRotation(direction).eulerAngles;
                lookRotation.y += angle;
                direction = Quaternion.Euler(lookRotation) * Vector3.forward;

                // Set the new destination based on the random direction.
                m_Destination = m_Target.Value.position.Value + direction.normalized *
                                Mathf.Clamp(m_Distance * m_DistancePercent, weaponStat.MinUseDist, weaponStat.MaxUseDist);

                count++;
                flipAngle = true;
                newAngle = true;
            } while (wapper.LineOfSight(m_Destination, m_Target.Value, true) &&
                     count < 100);

            // Prevent the agent from getting stuck in the same position by increasing the evade count. As soon as the evade count gets too high
            // the character will start choose a wider angle.
            if (m_EvadeCount == 0)
            {
                m_EvadeDestination = m_Destination;
                m_EvadeCount++;
            }
            else
            {
                if ((m_EvadeDestination - m_Destination).magnitude < 1)
                {
                    m_EvadeCount++;
                }
                else
                {
                    m_EvadeCount = 0;
                }
            }

            return count < 100;
        }

        /// <summary>
        /// Reset the Behavior Designer variables.
        /// </summary>
        public override void OnEnd()
        {
            UpdateRotation(true);
            m_Distance = float.MaxValue;
            m_EvadeCount = 0;
        }
    }
}