using Core.Configuration.Sound;
using Core.Utils;
using WeaponConfigNs;
using XmlConfig;
using Core.WeaponLogic.Common;
using Core.WeaponLogic;
using Core.EntityComponent;
using Core.IFactory;
using Utils.Singleton;

namespace App.Shared.GameModules.Weapon
{
    public class DefaultWeaponSoundLogic : AbstractAttachableWeaponLogic<DefaultWeaponSoundConfig,  int>, IWeaponSoundLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultWeaponSoundLogic));

        private IPlayerWeaponState _playerWeapon;
        private ISoundEntityFactory _soundEntityFactory;

        public DefaultWeaponSoundLogic(
            ISoundEntityFactory soundEntityFactory,
            IPlayerWeaponState playerWeapon,
            DefaultWeaponSoundConfig config) : base(config)
        {
            _playerWeapon = playerWeapon;
            _soundEntityFactory = soundEntityFactory;
        }

        public override void Apply(DefaultWeaponSoundConfig baseConfig, DefaultWeaponSoundConfig output, int arg)
        {
            output.Fire = arg != 0 ? arg : baseConfig.Fire;
        }
        public void PlaySound( EWeaponSoundType soundType)
        {
            var cfg = GetSoundConfig(soundType);
            if (null == cfg)
            {
                Logger.ErrorFormat("SoundType {0} is null !", soundType);
                return;
            }
            _playerWeapon.PlaySoundOnce(cfg.Id);
        }

        public SoundConfigItem GetSoundConfig(EWeaponSoundType type)
        {
            switch (type)
            {
                case EWeaponSoundType.LeftFire1:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.Fire);
                case EWeaponSoundType.OnShoulder:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.OnShoulder);
                case EWeaponSoundType.PullBolt:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.PullBolt);
                case EWeaponSoundType.ReloadEnd:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.ReloadEnd);
                case EWeaponSoundType.ReloadStart:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.ReloadStart);
                case EWeaponSoundType.SwitchFireMode:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.SwitchFireMode);
                case EWeaponSoundType.SwitchIn:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.SwitchIn);
                case EWeaponSoundType.ClipDrop:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.ClipDrop);
            }
            return null;
        }
    }
}
