using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;
using XmlConfig;

namespace Core.WeaponLogic
{
    
    public interface IWeaponLogic : IBulletContainer
    {
        void Update(IPlayerWeaponState playerWeapon, IUserCmd cmd);
        void SetAttachment(WeaponPartsStruct attachments);
        float GetFov();
        bool IsFovModified();
        bool CanCameraFocus();
        float GetFocusSpeed();
        float GetReloadSpeed();
        float GetBaseSpeed();
        
        float GetBreathFactor();
        bool EmptyHand { get; set; }
        void SetVisualConfig(ref VisualConfigGroup config);
        /// <summary>
        /// 清理状态信息
        /// </summary>
        void Reset();
     
    }

    public interface ISpeedProvider
    {
        /// <summary>
        /// 持武器时的速度，包括空手
        /// </summary>
        /// <returns></returns>
        float GetBaseSpeed();

        /// <summary>
        /// 空手速度
        /// </summary>
        /// <returns></returns>
        float GetDefaultSpeed();
    }

    public interface IBulletContainer
    {
        int GetBulletLimit();
        int GetSpecialReloadCount();
    }

    public interface IThrowingContainer
    {

    }

    public interface IWeaponSoundLogic
    {
        void PlaySound(EWeaponSoundType sound);
    }

    public interface IWeaponEffectLogic
    {
        void CreateFireEvent(IPlayerWeaponState playerState);
        void CreatePullBoltEffect(IPlayerWeaponState playerState);
        void PlayBulletDropEffect(IPlayerWeaponState playerState);
        void PlayMuzzleSparkEffect(IPlayerWeaponState playerState);
        void PlayPullBoltEffect(IPlayerWeaponState playerState);
    }
}