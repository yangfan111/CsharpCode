using Entitas;
using Entitas.CodeGeneration.Attributes;
using WeaponConfigNs;

namespace App.Shared.Components.Player
{
    [Player]
    public class WeaponConfigComponent :IComponent
    {
        [DontInitilize] public VisualConfigGroup LogicConfig = new VisualConfigGroup();
    }
}
