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
            CleanFireInspector = (IWeaponCmd cmd) => !cmd.IsFire;
        }
        public void OnUpdate(PlayerWeaponController controller, IWeaponCmd cmd, Contexts contexts)
        {
            if (CheckInterrupt(controller,cmd))
                return;
            UpdateFire(controller, cmd,contexts);
            if (CleanFireInspector(cmd))
            {
                controller.RelatedThrowAction.ClearState();
                controller.RelatedThrowAction.ThrowingEntityKey = EntityKey.Default;
                controller.RelatedThrowAction.LastFireWeaponKey = 0;
            }
        }

        protected abstract void UpdateFire(PlayerWeaponController controller, IWeaponCmd cmd,Contexts contexts);

        protected virtual bool CheckInterrupt(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            return false;
        }
        protected   Func< IWeaponCmd,bool> CleanFireInspector;


    }
}