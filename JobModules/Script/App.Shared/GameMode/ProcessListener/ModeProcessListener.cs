using App.Shared.GameModules.Weapon;
using Assets.XmlConfig;
using Core;
using Core.EntityComponent;
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
                shotMode = ((EFireMode)controller.HeldWeaponAgent.BaseComponent.RealFireModel).ToAudioGrpShotMode();
            if (controller.AudioController != null)
                controller.AudioController.PlayFireAudio(controller.HeldConfigId, shotMode);

        }


        public virtual void OnWeaponPickup(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            if (controller.AudioController != null)
                controller.AudioController.PlaySimpleAudio(EAudioUniqueId.PikcupWeapon);

        }

        public virtual void OnDrop(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
        }

      
        public void OnSwitch(IPlayerWeaponProcessor controller, int weaponId, EInOrOff op)
        {
            if (controller.AudioController != null)
            {
                var uniqueId = op == EInOrOff.In ? EAudioUniqueId.WeaponIn : EAudioUniqueId.WeaponOff;
                controller.AudioController.PlaySimpleAudio(uniqueId,true);
                
            }
        }

        public void OnItemPickup(IPlayerWeaponProcessor controller, int itemId, int category, int count)
        {
            switch ((ECategory)category)
            {
                case ECategory.Weapon:
                    break;
                default:
                    var audioUniqueId = AudioUtil.ToAudioUniqueId((ECategory)category, itemId);
                    if(audioUniqueId != EAudioUniqueId.None && controller.AudioController != null)
                        controller.AudioController.PlaySimpleAudio(audioUniqueId, true);
                    break;
            }
        }
    }
}
