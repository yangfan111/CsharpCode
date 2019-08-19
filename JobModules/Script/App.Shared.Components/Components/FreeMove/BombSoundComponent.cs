using Entitas;

namespace App.Shared.Components.FreeMove
{
    [FreeMove, SceneObject]
    public class BombSoundComponent : IComponent
    {
        public long CreateTime;
        public long LastSoundTime;
    }
}
