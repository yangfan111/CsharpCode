using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class EndReload : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            player.appearanceInterface.Appearance.EndReload();
            player.weaponLogic.WeaponSound.PlaySound(EWeaponSoundType.ReloadEnd);
        }
    }
}
