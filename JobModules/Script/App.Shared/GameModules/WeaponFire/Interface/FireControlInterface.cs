using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{

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
