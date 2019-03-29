using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot;
using App.Shared.GameModules.Player.Robot.Utility;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Assets.App.Shared.GameModules.Player.Robot.Conditions
{
    [TaskCategory("Voyager ")]
    [TaskDescription("I是否还在飞机上")]
    class IsOnAirPlanee : Conditional
    {
        private PlayerEntity mEntity;

       

        /// <summary>
        /// Cache the component references.
        /// </summary>
        public override void OnAwake()
        {
            
         
            
        }

        public override void OnStart()
        {
            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
        }

        /// <summary>
        /// Returns success if the target is in sight and is alive.
        /// </summary>
        /// <returns>Success if the target is within sight and is alive.</returns>
        public override TaskStatus OnUpdate()
        {
            if (mEntity != null )
            {
               
                if (mEntity != null && mEntity.gamePlay.LifeState != (int) EPlayerLifeState.Dead)
                {
                    return mEntity.gamePlay.GameState == (int) GameState.AirPlane
                        ? TaskStatus.Success
                        : TaskStatus.Failure;
                }
            }

            return TaskStatus.Failure;
        }
    }
}