using App.Shared.GameModules.Weapon;
using Core.Utils;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class OnSpecialPullBoltBegin : IAnimationEventCallback
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(OnSpecialPullBoltBegin));
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
           // player.AudioController().PlayPullBoltAudio(player.WeaponController().HeldConfigId);
            //player.PlayWeaponSound(XmlConfig.EWeaponSoundType.PullBolt);
            player.AudioController().PlayPullBoltAudio(player.WeaponController().HeldConfigId);
        }
    }
}
