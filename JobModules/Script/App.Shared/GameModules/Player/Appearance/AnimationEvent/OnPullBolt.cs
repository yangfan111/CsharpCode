using App.Shared.GameModules.Weapon;
using Core.Utils;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class OnPullBolt : IAnimationEventCallback
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(OnPullBolt));
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
           // player.AudioController().PlayPullBoltAudio(player.WeaponController().HeldConfigId);
            //player.PlayWeaponSound(XmlConfig.EWeaponSoundType.PullBolt);
        }
    }
}
