using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IWeaponLogic" />
    /// </summary>
    public interface IWeaponLogic
    {
        void Update(PlayerWeaponController controller, IUserCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="ISpeedProvider" />
    /// </summary>
    public interface ISpeedProvider
    {
        /// <summary>
        /// 空手速度
        /// </summary>
        /// <returns></returns>
        float GetDefaultSpeed();
    }

    /// <summary>
    /// Defines the <see cref="IThrowingContainer" />
    /// </summary>
    public interface IThrowingContainer
    {
    }

    /// <summary>
    /// Defines the <see cref="IWeaponSoundLogic" />
    /// </summary>
    public interface IWeaponSoundLogic
    {
        void PlaySound(PlayerWeaponController controller, EWeaponSoundType sound);
    }

    /// <summary>
    /// Defines the <see cref="IWeaponEffectLogic" />
    /// </summary>
    public interface IWeaponEffectLogic : IAfterFire
    {
        void CreateFireEvent(PlayerWeaponController controller);

        void CreatePullBoltEffect(PlayerWeaponController controller);

        void PlayBulletDropEffect(PlayerWeaponController controller);

        void PlayMuzzleSparkEffect(PlayerWeaponController controller);

        void PlayPullBoltEffect(PlayerWeaponController controller);
    }
}
