using Core.WeaponLogic;
using Core.WeaponLogic.Common;
using WeaponConfigNs;
using UnityEngine;
using XmlConfig;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using Core.EntityComponent;
using Core.IFactory;

namespace App.Shared.WeaponLogic
{
    public abstract class AbstractWeaponSoundLogic<T1, T3> : AbstractAttachableWeaponLogic<T1, T3>, IWeaponSoundLogic where T1 : ICopyableConfig<T1>, new() 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractWeaponSoundLogic<T1, T3>));
        private IPlayerWeaponState _playerWeapon;
        private ISoundEntityFactory _soundEntityFactory;
        public AbstractWeaponSoundLogic(IPlayerWeaponState playerWeapon,
            ISoundEntityFactory soundEntityFactory,
            T1 config) : base(config)
        {
            _playerWeapon = playerWeapon;
            _soundEntityFactory = soundEntityFactory;
        }

        public virtual void PlaySound(EWeaponSoundType sound)
        {
            var cfg = GetSoundConfig(sound);
            if (null == cfg)
            {
                Logger.ErrorFormat("SoundType {0} is null !", sound);
                return;
            }
            var player = _playerWeapon.Owner as PlayerEntity;
            if (null == player)
            {
                Logger.Error("palyer weapon owner is not player entity");
                return;
            }
            _playerWeapon.PlaySoundOnce(cfg.Id);
        }

        public abstract SoundConfigItem GetSoundConfig(EWeaponSoundType type);
    }
}