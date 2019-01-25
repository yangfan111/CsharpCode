using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class RemoveProp : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            player.appearanceInterface.Appearance.RemoveProp();
        }
    }
}
