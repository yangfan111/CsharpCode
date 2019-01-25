using Assets.XmlConfig;
using Core;
using Core.GameModeLogic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using WeaponConfigNs;

namespace App.Shared.Components.Player
{
   
    [Player]
    public class ModeLogicComponent : IComponent
    {
        [DontInitilize] public IWeaponModeLogic ModeLogic;
     
    }
}