using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IBulletFireInfo" />
    /// </summary>
    public interface IBulletFireInfo
    {
        /// <summary>
        /// 视线方向的射出位置 
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireViewPosition(PlayerWeaponController controller);

        /// <summary>
        /// 枪口方向的射出位置
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireEmitPosition(PlayerWeaponController controller);

        /// <summary>
        /// 射出方向
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireDir(int seed, PlayerWeaponController controller, int userCmdSeq);
    }
    public interface IWeaponFireUpdate
    {
        void Update(EntityKey owner, IUserCmd cmd, Contexts contexts);
    }
    public interface IWeaponFireController
    {
        void OnUpdate(EntityKey owner, WeaponSideCmd cmd, Contexts contexts);
    }
    //TODO: to be destroyed
    public interface IFireEffectFactory
    {
        void CreateBulletDropEffect(PlayerWeaponController controller);

        void CreateSparkEffect(PlayerWeaponController controller);
    }
}
