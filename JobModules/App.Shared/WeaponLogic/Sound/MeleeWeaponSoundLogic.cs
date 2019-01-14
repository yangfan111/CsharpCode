using App.Shared.WeaponLogic;
using Core.Configuration.Sound;
using Core.IFactory;
using Core.Utils;
using Core.WeaponLogic;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace Assets.App.Shared.WeaponLogic
{
    public class MeleeWeaponSoundLogic : AbstractWeaponSoundLogic<MeleeWeaponSoundConfig, int>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeWeaponSoundLogic));

        private IPlayerWeaponState _playerWeapon;

        public MeleeWeaponSoundLogic(
            IPlayerWeaponState playerWeapon,
            ISoundEntityFactory soundEntityFactory,
            MeleeWeaponSoundConfig config) : base(playerWeapon, soundEntityFactory, config)
        {
            _playerWeapon = playerWeapon;
        }

        public override SoundConfigItem GetSoundConfig(EWeaponSoundType type)
        {
            switch (type)
            {
                case EWeaponSoundType.LeftFire1:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.Left1);
                case EWeaponSoundType.RightFire1:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.Right);
                case EWeaponSoundType.LeftFire2:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.Left2);
                case EWeaponSoundType.Transform:
                    return SingletonManager.Get<SoundConfigManager>().GetSoundById(_config.Transform);
            }
            return null;
        }
        public override void Apply(MeleeWeaponSoundConfig baseConfig, MeleeWeaponSoundConfig output, int arg)
        {
           
        }
    }
}
