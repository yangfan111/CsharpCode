using App.Shared.Audio;
using App.Shared.Components.Player;
using App.Shared.Components.Weapon;
using Assets.Utils.Configuration;
using Core;
using Core.Appearance;
using Core.CharacterState;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    public class WeaponAttackProxy
    {
        LoggerAdapter _loggerAdapter = new LoggerAdapter("LoggerAdapter");
        private int RefreshedTimeFrame;
        private bool PartRefreshed;

        public WeaponAttackProxy(PlayerWeaponController controller)
        {
            Owner = controller;
        }

        public WeaponRuntimeDataComponent RuntimeComponent { get; private set; }
        public WeaponBasicDataComponent   BasicComponent   { get; private set; }
        public WeaponAllConfigs           WeaponConfigAssy { get; private set; }

        public PlayerWeaponController    Owner           { get; private set; }
        public PlayerAudioControllerBase AudioController { get; private set; }

        public ICharacterState      CharacterState { get; private set; }
        public OrientationComponent Orientation    { get; private set; }
        public ICharacterAppearance Appearence     { get; private set; }

        public PlayerMoveComponent PlayerMove { get; private set; }
        
        public CameraStateNewComponent CmrNewComponent { get; private set; }

        public WeaponPartsAchive Parts
        {
            get
            {
                if (PartRefreshed)
                    return parts;
                PartRefreshed = true;
                parts = Owner.HeldWeaponAgent.SyncParts();
                return parts;
            }
        }
        private WeaponPartsAchive parts;
        public bool                IsAiming
        {
            get { return CmrNewComponent.IsAiming(); }
        }

        public bool CanFire
        {
            get { return CmrNewComponent.CanFire; }
        }

    
        public void Refresh()
        {
            RefreshedTimeFrame = Time.frameCount;
            PartRefreshed = false;
            RuntimeComponent = Owner.HeldWeaponAgent.RunTimeComponent;
            BasicComponent   = Owner.HeldWeaponAgent.BaseComponent;
            WeaponConfigAssy = Owner.HeldWeaponAgent.WeaponConfigAssy;
            AudioController    = Owner.AudioController;
            CharacterState     = Owner.RelatedCharState;
            Orientation        = Owner.RelatedOrientation;
            Appearence         = Owner.RelatedAppearence;
            CmrNewComponent = Owner.RelatedCameraSNew;
            PlayerMove         = Owner.RelatedPlayerMove;
        }

        public float GetAttachedAttributeByType(WeaponAttributeType attributeType)
        {
            int pid = 0;
            switch (attributeType)
            {
                case WeaponAttributeType.BaseDamage:
                    pid = Parts.Bore;
                    break;
                case WeaponAttributeType.DistanceDecay:
                    pid = Parts.Interlock;
                    break;
                case WeaponAttributeType.EmitVelocity:
                case WeaponAttributeType.AttackInterval:
                    pid = Parts.Feed;
                    break;
                
                    default:
                        return SingletonManager.Get<WeaponPartsConfigManager>()
                                        .GetPartAchiveAttachedAttributeByType(Parts, attributeType);
            }
            float val= SingletonManager.Get<WeaponPartsConfigManager>()
                            .GetPartAchiveAttachedAttributeByType(pid, attributeType);
            return val == GlobalConst.AttributeInvalidValue ? 0 : val;

        }

        public bool IsValid()
        {
            if (RefreshedTimeFrame != Time.frameCount)
            {
                return false;
            }

            return true;
        }

        public void InterruptPullBolt()
        {
            RuntimeComponent.InterruptPullBolt();
            AudioController.StopPullBoltAudio();
        }
    }
}