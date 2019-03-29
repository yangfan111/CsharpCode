using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.CharacterState.Action
{
    public class ActionStateIdComparer : IEqualityComparer<ActionStateId>
    {
        public bool Equals(ActionStateId x, ActionStateId y)
        {
            return x == y;
        }

        public int GetHashCode(ActionStateId obj)
        {
            return (int)obj;
        }

        private static ActionStateIdComparer _instance = new ActionStateIdComparer();
        public static ActionStateIdComparer Instance
        {
            get
            {
                return _instance;
            }
        }



    }

    /// <summary>
    /// ActionStateId 会转换成short，ActionStateId不要超过32767
    /// </summary>
    public enum ActionStateId
    {
        CommonNull,                     // 0
        
        Fire,                           // 1
        SpecialFire,                    // 2 
        SpecialFireHold,                // 3
        SpecialFireEnd,                 // 4
        Injury,                         // 5
        Reload,                         // 6
        SpecialReload,                  // 7

        Unarm,                          // 8
        Draw,                           // 9
        SwitchWeapon,                   // 10
        PickUp,                         // 11
        MeleeAttack,                    // 12
        Grenade,                        // 13
        OpenDoor,                       // 14
        Props,                          // 15
        
        Gliding,                        // 16
        Parachuting,                    // 17
        
        BuriedBomb,                     // 18
        DismantleBomb,                  // 19

        KeepNull,                       // 20
        Drive,                          // 21
        Sight,                          // 22
        Rescue,                         // 23

        EnumEnd
    }
}
