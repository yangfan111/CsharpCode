using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class AddProp : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if (null == eventParam) return;
            player.appearanceInterface.Appearance.AddProp(eventParam.intParameter);
        }
    }
}
