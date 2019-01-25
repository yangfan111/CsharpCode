using Core.Common;
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

        [DontInitilize] public string Content;
        [DontInitilize] public ETipType TipType;
        [DontInitilize] public TipLocation Location;
    }
}