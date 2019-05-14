using Core.EntityComponent;
using UnityEngine;
using WeaponConfigNs;

namespace  App.Shared.GameModules.Weapon.Behavior
{
   
    /// <summary>
    /// Defines the <see cref="IBulletFireListener" />
    /// </summary>
    public interface IBulletFireListener : IFireProcess
    {
        void OnBulletFire(PlayerWeaponController controller, WeaponSideCmd cmd);
    }

    /// <summary>
    /// Defines the <see cref="IFireTriggger" />
    /// </summary>
   

    /// <summary>
    /// PunchYaw, PunchPitch, PunchYawDirection, RunUpMax, PunchDecayCD
    /// </summary>
    public interface IFireShakeProcessor : IFrameProcess, IAfterFireProcess
    {
    }

    /// <summary>
    /// 计算Accuracy
    /// </summary>
    public interface IAccuracyCalculator : IIdleProcess, IBeforeFireProcess
    {
    }


    /// <summary>
    /// 计算Spread
    /// </summary>
    public interface ISpreadProcessor : IBeforeFireProcess,IIdleProcess
    {
    }

    /// <summary>
    /// Defines the <see cref="IFireProcessCounter" />
    /// </summary>
    public interface IFireProcessCounter : IBeforeFireProcess, IIdleProcess
    {
    }
    public interface IIdleAndAfterFireProcess : IAfterFireProcess,IIdleProcess
    {
    }
}
