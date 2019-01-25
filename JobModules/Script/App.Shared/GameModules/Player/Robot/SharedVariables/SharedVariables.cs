
using BehaviorDesigner.Runtime;
using Core;



namespace App.Shared.GameModules.Player.Robot.SharedVariables
{
    [System.Serializable]
    public class SharedGameContext : SharedVariable<Contexts>
    {
        public static implicit operator SharedGameContext(Contexts value) { return new SharedGameContext { mValue = value }; }
       
    }
    [System.Serializable]
    public class SharedSlotType : SharedVariable<EWeaponSlotType>
    {
        public static implicit operator SharedSlotType(EWeaponSlotType value) { return new SharedSlotType { mValue = value }; }
       
    }
}
