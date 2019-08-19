using App.Shared.GameModules.Weapon;
using App.Shared.Player.Events;
using Assets.XmlConfig;
using Core;
using Core.EntityComponent;
using Core.Event;
using Core.Fsm;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="ModeProcessListener" />
    /// </summary>
    public class ModeProcessListener : IModeProcessListener
    {
        public virtual void OnExpend(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            AudioGrp_ShotMode shotMode;
            if (controller.HeldWeaponAgent.HasSilencerPart)
                shotMode = AudioGrp_ShotMode.Silencer;
            else
                shotMode = ((EFireMode) controller.HeldWeaponAgent.BaseComponent.RealFireModel).ToAudioGrpShotMode();

            controller.AudioController.PlayFireAudio(controller.HeldConfigId, shotMode);
        }


        public virtual void OnWeaponPickup(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            controller.AudioController.PlaySimpleAudio(EAudioUniqueId.PickupWeapon);
        }

        public virtual void OnDrop(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
        }


        public void OnSwitch(IPlayerWeaponProcessor controller, int weaponId, EInOrOff op)
        {
                var uniqueId = op == EInOrOff.In ? EAudioUniqueId.WeaponIn : EAudioUniqueId.WeaponOff;
                controller.AudioController.StopPullBoltAudio();
                controller.AudioController.PlaySimpleAudio(uniqueId, true);
        }

        public void OnItemPickup(IPlayerWeaponProcessor controller, int itemId, int category, int count)
        {
            switch ((ECategory) category)
            {
                case ECategory.Weapon:
                    break;
                default:
                    var audioUniqueId = AudioUtil.ToAudioUniqueId((ECategory) category, itemId);
                    if (audioUniqueId != EAudioUniqueId.None )
                        controller.AudioController.PlaySimpleAudio(audioUniqueId, true);
                    break;
            }
        }
    }
}