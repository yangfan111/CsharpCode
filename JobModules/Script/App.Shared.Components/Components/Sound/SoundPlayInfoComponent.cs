using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Sound
{
    [Sound]
    public class SoundPlayInfoComponent : IComponent 
    {
        [DontInitilize] public bool Loop;
        [DontInitilize] public float Volume;
        [DontInitilize] public bool Pause;
    }
}
