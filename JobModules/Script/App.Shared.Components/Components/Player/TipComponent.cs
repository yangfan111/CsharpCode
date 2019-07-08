
using Core;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    public class TipComponent : IComponent 
    {
        public enum TipLocation
        {
            Bottom,
            Top,
        }
        [DontInitilize] public bool ForTest;
        [DontInitilize] public string Content;
        [DontInitilize] public ETipType TipType;
        [DontInitilize] public TipLocation Location;
    }
}