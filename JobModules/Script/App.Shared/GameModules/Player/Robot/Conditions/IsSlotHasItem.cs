using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot.SharedVariables;
using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Weapon;
using BehaviorDesigner.Runtime.Tasks;
using Core;

namespace App.Shared.GameModules.Player.Robot.Conditions
{

    [TaskCategory("Voyager ")]
    [TaskDescription("某个是否有某种物品")]
    class IsSlotHasItem : Conditional
    {
        private PlayerEntity mEntity;
        public SharedSlotType SlotType;
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
                    //return !mEntity.WeaponController().GetSlotWeaponInfo(SlotType.Value).Equals(new WeaponInfo())?TaskStatus.Success:TaskStatus.Failure;
                    return SlotType.Value != EWeaponSlotType.None?TaskStatus.Success:TaskStatus.Failure;
                }
            }

            return TaskStatus.Failure;
        }
    }
}


