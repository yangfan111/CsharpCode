using System;
using BehaviorDesigner.Runtime.Tasks;
using Core.EntityComponent;
using JetBrains.Annotations;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public abstract class AbstractFireController : IWeaponFireController
    {
        public AbstractFireController()
        {
            CleanFireInspector = (WeaponSideCmd cmd) => !cmd.IsFire;
        }
        public void OnUpdate(EntityKey entityKey, WeaponSideCmd cmd, Contexts contexts)
        {
            var weaponController = entityKey.WeaponController();
            if (CheckInterrupt(weaponController,cmd))
                return;
            UpdateFire(weaponController, cmd,contexts);
            if (CleanFireInspector(cmd))
            {
                weaponController.RelatedThrowAction.ClearState();
                weaponController.RelatedThrowAction.ThrowingEntityKey = EntityKey.Default;
                weaponController.RelatedThrowAction.LastFireWeaponKey = 0;
            }
        }

        protected abstract void UpdateFire(PlayerWeaponController controller, WeaponSideCmd cmd,Contexts contexts);

        protected virtual bool CheckInterrupt(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            return false;
        }
        protected   Func< WeaponSideCmd,bool> CleanFireInspector;


    }
}