using App.Shared.Components;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot;
using BehaviorDesigner.Runtime.Tasks;

namespace Assets.App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("跳伞")]
    class Skydiving : BehaviorDesigner.Runtime.Tasks.Action
    {
        // Component references
       
        private PlayerEntity m_entity;
        /// <summary>
        /// Cache the component references.
        /// </summary>
        public override void OnAwake()
        {
         
            m_entity = (PlayerEntity) GetComponent<EntityReference>().Reference;
        }

        /// <summary>
        /// Reset the SharedVariables.
        /// </summary>
        /// <returns>Always returns Success.</returns>
        public override TaskStatus OnUpdate()
        {
            
                m_entity.robot.Wapper.RobotUserCmdProvider.IsF = true;
                return TaskStatus.Success;

        
        }
    }
}

