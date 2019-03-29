using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Sound
{
    [Sound, UniquePrefix("flag")]
    public class PlayingComponent : IComponent
    {
    }
}
