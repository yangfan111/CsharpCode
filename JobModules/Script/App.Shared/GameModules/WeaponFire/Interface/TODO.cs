using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;
using WeaponConfigNs;

namespace  App.Shared.GameModules.Weapon.Behavior
{
   
  
    public interface IWeaponFireUpdate
    {
        void Update(PlayerWeaponController controller, IUserCmd cmd);
    }
    public interface IWeaponFireController
    {
        void OnUpdate(PlayerWeaponController controller, IWeaponCmd cmd);
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

 
}
