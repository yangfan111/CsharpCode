using Core.Utils;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class OnPullBolt : IAnimationEventCallback
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(OnStep));
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            player.weaponLogic.WeaponSound.PlaySound(XmlConfig.EWeaponSoundType.PullBolt);
        }
    }
}
