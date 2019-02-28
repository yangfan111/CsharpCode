using Core.EntityComponent;
using UnityEngine;
using WeaponConfigNs;

namespace  App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IForFireLogic" />
    /// </summary>
    public interface IForFireLogic
    {
    }

    /// <summary>
    /// Defines the <see cref="IBeforeFireBullet" />
    /// </summary>
    public interface IBeforeFireBullet : IForFireLogic
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之前调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void BeforeFireBullet(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IAfterFire" />
    /// </summary>
    public interface IAfterFire : IForFireLogic
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之后调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IIdle" />
    /// </summary>
    public interface IIdle : IForFireLogic
    {
        /// <summary>
        /// 没有Fire命令
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFrame" />
    /// </summary>
    public interface IFrame : IForFireLogic
    {
        /// <summary>
        /// 每个Cmd调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IBulletFire" />
    /// </summary>
    public interface IBulletFire : IForFireLogic
    {
        void OnBulletFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFireTriggger" />
    /// </summary>
    public interface IFireTriggger : IForFireLogic
    {
        bool IsTrigger(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFireCheck" />
    /// </summary>
    public interface IFireCheck : IForFireLogic
    {
        bool IsCanFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }

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

        EntityKey CreateThrowing(PlayerWeaponController controller, Vector3 direction, int renderTime, float initVel);

        void UpdateThrowing(EntityKey entityKey, bool isThrow, float initVel);

        void DestroyThrowing(EntityKey entityKey);
    }

    /// <summary>
    /// 创建开火相关特效
    /// </summary>
    public interface IFireEffectFactory
    {
        void CreateBulletDropEffect(PlayerWeaponController controller);

        void CreateSparkEffect(PlayerWeaponController controller);
    }

    /// <summary>
    /// 计算Spread
    /// </summary>
    public interface ISpreadLogic : IBeforeFireBullet
    {
    }

    /// <summary>
    /// Defines the <see cref="IFireBulletCounter" />
    /// </summary>
    public interface IFireBulletCounter : IBeforeFireBullet, IIdle
    {
    }
}
