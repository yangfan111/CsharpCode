using Entitas;
using Core.AnimatorClip;

namespace App.Shared.Components.Player
{
    [Player]
    public class AnimatorClipComponent : IComponent
    {
        public AnimatorClipManager ClipManager;
    }
}
