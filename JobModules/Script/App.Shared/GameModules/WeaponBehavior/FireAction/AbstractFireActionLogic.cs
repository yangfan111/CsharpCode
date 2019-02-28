using Assets.Utils.Configuration;
using Core.Utils;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="AbstractFireActionLogic{T1}" />
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class AbstractFireActionLogic<T1> : IFireActionLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractFireActionLogic<T1>));

        public AbstractFireActionLogic()
        {
        }

        public virtual void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            int weaponId = controller.HeldWeaponAgent.ConfigId;
            var needActionDeal = CheckNeedActionDeal(weaponId, ActionDealEnum.Fire);
            OnAfterFire(controller, needActionDeal);
        }

        protected abstract void OnAfterFire(PlayerWeaponController controller, bool needActionDeal);

        public abstract void OnIdle(PlayerWeaponController playerWeapon, IWeaponCmd cmd);

        protected bool CheckNeedActionDeal(int weaponId, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponResourceConfigManager>().NeedActionDeal(weaponId, ActionDealEnum.Reload);
        }

        protected void SpecialFire(PlayerWeaponController controller, bool needActionDeal)
        {
            if (controller.RelatedCameraSNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                if (controller.RelatedStateInterface != null)
                {
                    controller.RelatedStateInterface.SpecialSightsFire(() =>
                    {
                        if (needActionDeal)
                        {
                            controller.RelatedAppearence.RemountWeaponOnRightHand();
                        }
                    });
                }
                else
                {
                    LogError("controller has no stateInterface");
                }
            }
            else
            {
                if (controller.RelatedStateInterface != null)

                {
                    controller.RelatedStateInterface.SpecialFire(() =>
                    {
                        if (needActionDeal)
                        {
                            controller.RelatedAppearence.RemountWeaponOnRightHand();
                        }
                    });
                }
                else
                {
                    LogError("controller has no stateInterface");
                }
            }
            // controller.PlayWeaponSound(EWeaponSoundType.LeftFire1);
            if (needActionDeal)
            {
                controller.RelatedAppearence.MountWeaponOnAlternativeLocator();
            }
        }

        protected void DefaultFire(PlayerWeaponController controller)
        {
            if (controller.RelatedCameraSNew != null && controller.RelatedCameraSNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                if (controller.RelatedStateInterface != null)
                {
                    controller.RelatedStateInterface.SightsFire();
                }
                else
                {
                    LogError("player has no stateInterface");
                }
            }
            else
            {
                if (controller.RelatedStateInterface != null)
                {
                    controller.RelatedStateInterface.Fire();
                }
                else
                {
                    LogError("player has no stateInterface");
                }
            }
        }

        private void LogError(string msg)
        {
            Logger.Error(msg);
            System.Console.WriteLine(msg);
        }
    }
}
