using App.Shared.GameModules.Weapon;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class StartReload : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            player.appearanceInterface.Appearance.StartReload();
            player.PlayWeaponSound(EWeaponSoundType.ReloadStart);
        }
    }
}
