using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic.WeaponLogicInterface;
using XmlConfig;

namespace Core.WeaponLogic
{
    public interface IWeaponLogic
    {
        void Update(PlayerEntity playerEntity, WeaponEntity weaponEntity, IUserCmd cmd);
    }

    public interface ISpeedProvider
    {
        /// <summary>
        /// 空手速度
        /// </summary>
        /// <returns></returns>
        float GetDefaultSpeed();
    }

    public interface IThrowingContainer
    {

    }

    public interface IWeaponSoundLogic
    {
        void PlaySound(PlayerEntity playerEntity, EWeaponSoundType sound);
    }

    public interface IWeaponEffectLogic : IAfterFire
    {
        void CreateFireEvent(PlayerEntity playerState);
        void CreatePullBoltEffect(PlayerEntity playerState);
        void PlayBulletDropEffect(PlayerEntity playerState);
        void PlayMuzzleSparkEffect(PlayerEntity playerState);
        void PlayPullBoltEffect(PlayerEntity playerState);
    }
}