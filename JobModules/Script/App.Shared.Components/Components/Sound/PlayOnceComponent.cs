using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Sound
{
    [Sound, UniquePrefix("is")]
    public class PlayOnceComponent : IComponent
    {
    }
}
