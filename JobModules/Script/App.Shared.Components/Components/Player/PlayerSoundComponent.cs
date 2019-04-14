using Core.Sound;
using Entitas;

namespace App.Shared.Components.Player
{
    [Player]
    public class SoundManagerComponent :IComponent
    {
        public IPlayerSoundManager Value;
    }
}