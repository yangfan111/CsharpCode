using UnityEngine;
using Core.EntityComponent;
using WeaponConfigNs;

namespace Core.WeaponLogic.WeaponLogicInterface
{
    #region FireLogicInterface
    public interface IForFireLogic { }

    public interface IBeforeFireBullet : IForFireLogic
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之前调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);
    }

    public interface IAfterFire : IForFireLogic
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之后调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);    
    }

    public interface IIdle : IForFireLogic
    {
        /// <summary>
        /// 没有Fire命令
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnIdle(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);
    }

    public interface IFrame : IForFireLogic
    {
        /// <summary>
        /// 每个Cmd调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnFrame(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);
    }

    public interface IBulletFire : IForFireLogic
    {
        void OnBulletFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);
    }


    public interface IFireTriggger : IForFireLogic
    {
        bool IsTrigger(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);
    }

    public interface IFireCheck : IForFireLogic
    {
        bool IsCanFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);
    }

    #endregion

    /// <summary>
    /// PunchYaw, PunchPitch, PunchYawDirection, RunUpMax, PunchDecayCD
    /// </summary>
    public interface IKickbackLogic : IFrame, IAfterFire
    {
    }


    /// <summary>
    /// 计算Accuracy
    /// </summary>
    public interface IAccuracyLogic : IIdle, IBeforeFireBullet
    {
        
    }

    /// <summary>
    /// 创建投掷物对象
    /// </summary>
    public interface IThrowingFactory
    {
        float ThrowingInitSpeed(bool isNear);
        int BombCountdownTime { get; }
        ThrowingConfig ThrowingConfig { get; }
        EntityKey CreateThrowing(PlayerEntity playerEntity, Vector3 direction, int renderTime, float initVel);
        void UpdateThrowing(EntityKey entityKey, bool isThrow, float initVel);
        void DestroyThrowing(EntityKey entityKey);
    }

    /// <summary>
    /// 创建开火相关特效
    /// </summary>
    public interface IFireEffectFactory
    {
        void CreateBulletDropEffect(PlayerEntity playerEntity);
        void CreateSparkEffect(PlayerEntity playerEntity);
    }

    /// <summary>
    /// 计算Spread
    /// </summary>
    public interface ISpreadLogic: IBeforeFireBullet
    {
        
    }

    public interface IFireBulletCounter: IBeforeFireBullet, IIdle
    {
        
    }
}