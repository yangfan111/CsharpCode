using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using BehaviorDesigner.Runtime.Tasks;

namespace App.Shared.GameModules.Player.Robot.Conditions
{

    [TaskCategory("Voyager ")]
    [TaskDescription("I是否还在飞机上")]
    class IsSelfOnGround : Conditional
    {
        private PlayerEntity mEntity;
        public override void OnAwake()
        {
           
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
            
        }
        public override TaskStatus OnUpdate()
        {
            if (mEntity != null )
            {
               
                if (mEntity != null )
                {
                    return mEntity.playerMove.IsGround
                        ? TaskStatus.Success
                        : TaskStatus.Failure;
                }
            }

            return TaskStatus.Failure;
        }
    }
}
