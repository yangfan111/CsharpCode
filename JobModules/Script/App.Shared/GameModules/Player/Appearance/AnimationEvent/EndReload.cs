using App.Shared.WeaponLogic;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class EndReload : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            player.appearanceInterface.Appearance.EndReload();
            player.PlayWeaponSound(EWeaponSoundType.ReloadEnd);
        }
    }
}
