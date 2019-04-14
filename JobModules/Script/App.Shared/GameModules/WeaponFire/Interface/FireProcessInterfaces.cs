using Core.EntityComponent;
using UnityEngine;
using WeaponConfigNs;

namespace  App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IFireProcess" />
    /// </summary>
    public interface IFireProcess
    {
    }

    /// <summary>
    /// Defines the <see cref="IBeforeFireProcess" />
    /// </summary>
    public interface IBeforeFireProcess : IFireProcess
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之前调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnBeforeFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IAfterFireProcess" />
    /// </summary>
    public interface IAfterFireProcess : IFireProcess
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之后调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IIdleProcess" />
    /// </summary>
    public interface IIdleProcess : IFireProcess
    {
        /// <summary>
        /// 没有Fire命令
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFrameProcess" />
    /// </summary>
    public interface IFrameProcess : IFireProcess
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
    public interface IBulletFire : IFireProcess
    {
        void OnBulletFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFireTriggger" />
    /// </summary>
    public interface IFireTriggger : IFireProcess
    {
        bool IsTrigger(PlayerWeaponController controller, IWeaponCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFireChecker" />
    /// </summary>
    public interface IFireChecker : IFireProcess
    {
        bool IsCanFire(PlayerWeaponController controller, IWeaponCmd cmd);
    }


    
}
