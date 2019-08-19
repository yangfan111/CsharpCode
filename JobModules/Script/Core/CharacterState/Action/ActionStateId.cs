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
        CommonNull,                     
        
        Fire,                           
        SpecialFire,                    
        SpecialFireHold,                
        SpecialFireEnd,                 
        Injury,                         
        Reload,                         
        SpecialReload,                  

        Unarm,                          
        Draw,                           
        SwitchWeapon,                   
        PickUp,                         
        MeleeAttackOne,                 
        MeleeAttackTwo,                 
        MeleeAttackSpecial,             
        Grenade,                        
        OpenDoor,                       
        Props,                          
        
        Gliding,                        
        Parachuting,                    
        
        BuriedBomb,                     
        DismantleBomb,                  

        KeepNull,                       
        Drive,                          
        Sight,                          
        Rescue,                         
        RageStart,
        RageLoop,
        RageEnd,
        SuccessPose,
        
        TransfigurationNull,
        TransfigurationStart,
        TransfigurationFinish,

        EnumEnd
    }
}
